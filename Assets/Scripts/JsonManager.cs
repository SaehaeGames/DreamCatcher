using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonManager
{
    /// <summary>여러 저장 파일을 한 작업으로 확정하기 위한 변경 후 JSON 묶음입니다.</summary>
    [Serializable]
    public class SaveEntry
    {
        public string fileName;
        public string json;
    }

    [Serializable]
    private class SaveTransaction
    {
        public List<SaveEntry> entries;
    }

    public static SaveEntry CreateSaveEntry<T>(string fileName, T data)
    {
        return new SaveEntry { fileName = fileName, json = JsonUtility.ToJson(data, true) };
    }

    /// <summary>
    /// 변경 후 데이터를 먼저 기록합니다. 기록 저장 성공이 구매 확정 시점입니다.
    /// 개별 파일 적용 도중 종료되면 다음 로드 전에 같은 내용을 다시 적용합니다.
    /// </summary>
    public bool TrySaveBatch(List<SaveEntry> entries)
    {
        if (!RecoverPendingTransaction() || !ValidateEntries(entries)) return false;
        string journalPath = GetTransactionPath();
        if (!WriteJsonSafely(journalPath, JsonUtility.ToJson(new SaveTransaction { entries = entries })))
            return false;

        // 확정 기록이 있으므로 적용 실패를 구매 실패로 알려 재구매하게 하지 않습니다.
        // 이후 일반 저장과 새 구매는 이 기록을 먼저 복구해야 실행될 수 있습니다.
        if (!RecoverPendingTransaction())
            Debug.LogWarning("[JsonManager] 확정된 구매는 다음 저장/실행 시 복구합니다.");
        return true;
    }

    /// <summary>확정된 저장 작업을 모두 재적용한 뒤 기록을 제거합니다. 같은 값의 재적용은 중복 지급을 하지 않습니다.</summary>
    public bool RecoverPendingTransaction()
    {
        try { return RecoverPendingTransactionCore(); }
        catch (IOException exception)
        {
            Debug.LogError("[JsonManager] 저장 경로 접근 실패: " + exception.Message);
            return false;
        }
        catch (UnauthorizedAccessException exception)
        {
            Debug.LogError("[JsonManager] 저장 경로 접근 권한이 없습니다: " + exception.Message);
            return false;
        }
    }

    private bool RecoverPendingTransactionCore()
    {
        string journalPath = GetTransactionPath();
        if (!File.Exists(journalPath)) return true;
        if (!TryLoadJson(journalPath, out SaveTransaction transaction) || !ValidateEntries(transaction.entries))
        {
            Debug.LogError("[JsonManager] 저장 복구 기록이 손상되었습니다. 기존 파일을 덮어쓰지 않습니다.");
            return false;
        }

        foreach (SaveEntry entry in transaction.entries)
        {
            if (!WriteJsonSafely(GetPath(entry.fileName), entry.json)) return false;
        }

        try
        {
            File.Delete(journalPath);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError("[JsonManager] 저장 복구 기록 정리 실패: " + exception.Message);
            return false;
        }
    }

    private static bool ValidateEntries(List<SaveEntry> entries)
    {
        if (entries == null || entries.Count == 0) return false;
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (SaveEntry entry in entries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.fileName)
                || entry.fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                || entry.fileName.Contains("/") || entry.fileName.Contains("\\")
                || string.IsNullOrWhiteSpace(entry.json) || !names.Add(entry.fileName)) return false;
        }
        return true;
    }

    private static string GetTransactionPath()
    {
        // 점으로 시작하는 임시 파일은 Unity 에셋으로 가져오지 않습니다.
        return Path.Combine(GetSaveFolderPath(), ".save-transaction");
    }

    private void EnsurePendingTransactionRecovered()
    {
        if (!RecoverPendingTransaction())
            throw new IOException("확정된 저장 작업을 복구할 수 없어 데이터 로드를 중단합니다.");
    }

    public T LoadDefaultData<T>(string fileName) where T : new()
    {
        TextAsset defaultJson = Resources.Load<TextAsset>("DefaultJsonData/" + fileName);
        if (defaultJson == null)
        {
            Debug.LogWarning($"[JsonManager] Default data not found: {fileName}");
            return new T();
        }

        try
        {
            T data = JsonUtility.FromJson<T>(defaultJson.text);
            return ReferenceEquals(data, null) ? new T() : data;
        }
        catch (Exception exception)
        {
            Debug.LogError($"[JsonManager] Default data parse failed: {fileName}\n{exception.Message}");
            return new T();
        }
    }

    public T LoadData<T>(string fileName) where T : new()
    {
        return LoadData<T>(fileName, null);
    }

    /// <summary>
    /// JSON 문법과 데이터별 유효성을 함께 확인하여 원본, 백업, 기본 데이터 순서로 읽습니다.
    /// 검증 함수를 생략하는 기존 호출은 빈 목록을 포함한 종전 로드 규칙을 유지합니다.
    /// </summary>
    public T LoadData<T>(string fileName, Func<T, bool> validateData) where T : new()
    {
        EnsurePendingTransactionRecovered();
        string savedPath = GetPath(fileName);
        if (TryLoadJson(savedPath, out T data, validateData)) return data;

        string backupPath = GetBackupPath(savedPath);
        if (TryLoadJson(backupPath, out data, validateData))
        {
            Debug.LogWarning($"[JsonManager] 백업 저장 데이터를 복구합니다: {fileName}");
            RestorePrimaryWithoutRotatingBackup(savedPath, JsonUtility.ToJson(data, true));
            return data;
        }

        data = LoadDefaultData<T>(fileName);
        // 기본 데이터까지 손상된 경우에는 기존 원본과 백업을 새 빈 데이터로 덮어쓰지 않습니다.
        if (!IsValidLoadedData(data, validateData))
        {
            Debug.LogError($"[JsonManager] 유효한 기본 저장 데이터가 없어 파일을 유지합니다: {fileName}");
            return data;
        }
        SaveData(fileName, data);
        return data;
    }

    public List<T> LoadDataList<T>(string fileName) where T : new()
    {
        EnsurePendingTransactionRecovered();
        string savedPath = GetPath(fileName);
        if (TryLoadJson(savedPath, out Wrapper<T> wrapper) && wrapper.datalist != null)
        {
            return wrapper.datalist;
        }

        string backupPath = GetBackupPath(savedPath);
        if (TryLoadJson(backupPath, out wrapper) && wrapper.datalist != null)
        {
            Debug.LogWarning($"[JsonManager] 백업 저장 데이터 목록을 복구합니다: {fileName}");
            RestorePrimaryWithoutRotatingBackup(savedPath, JsonUtility.ToJson(wrapper, true));
            return wrapper.datalist;
        }

        TextAsset defaultJson = Resources.Load<TextAsset>("DefaultJsonData/" + fileName);
        if (defaultJson != null)
        {
            try
            {
                wrapper = JsonUtility.FromJson<Wrapper<T>>(defaultJson.text);
                if (wrapper != null && wrapper.datalist != null)
                {
                    SaveDataList(fileName, wrapper.datalist);
                    return wrapper.datalist;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError($"[JsonManager] Default data list parse failed: {fileName}\n{exception.Message}");
            }
        }

        List<T> emptyList = new List<T>();
        SaveDataList(fileName, emptyList);
        return emptyList;
    }

    public void SaveData<T>(string fileName, T data)
    {
        TrySaveData(fileName, data);
    }

    /// <summary>저장 성공 여부가 필요한 데이터 변경에서 사용합니다.</summary>
    public bool TrySaveData<T>(string fileName, T data)
    {
        if (!RecoverPendingTransaction()) return false;
        return WriteJsonSafely(GetPath(fileName), JsonUtility.ToJson(data, true));
    }

    public void SaveDataList<T>(string fileName, List<T> dataList)
    {
        Wrapper<T> wrapper = new Wrapper<T> { datalist = dataList ?? new List<T>() };
        TrySaveData(fileName, wrapper);
    }

    private static bool TryLoadJson<T>(string path, out T data, Func<T, bool> validateData = null)
    {
        data = default(T);
        if (!File.Exists(path)) return false;

        try
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json)) return false;

            data = JsonUtility.FromJson<T>(json);
            return IsValidLoadedData(data, validateData);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[JsonManager] 저장 데이터 읽기 실패: {path}\n{exception.Message}");
            return false;
        }
    }

    /// <summary>데이터 매니저가 제공한 검사만 적용하며, 검사 오류도 잘못된 파일로 처리해 다음 복구 후보를 확인합니다.</summary>
    private static bool IsValidLoadedData<T>(T data, Func<T, bool> validateData)
    {
        if (ReferenceEquals(data, null)) return false;
        if (validateData == null) return true;
        try { return validateData(data); }
        catch (Exception exception)
        {
            Debug.LogWarning("[JsonManager] 저장 데이터 검증 실패: " + exception.Message);
            return false;
        }
    }

    private static bool WriteJsonSafely(string savedPath, string jsonData)
    {
        string tempPath = savedPath + ".tmp";
        string backupPath = GetBackupPath(savedPath);

        try
        {
            // 디스크에 내보낸 뒤 교체하여 불완전한 JSON이 원본이 되지 않게 합니다.
            using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream, new System.Text.UTF8Encoding(false)))
            {
                writer.Write(jsonData);
                writer.Flush();
                stream.Flush(true);
            }
            if (File.Exists(savedPath))
            {
                try
                {
                    File.Replace(tempPath, savedPath, backupPath, true);
                }
                catch (PlatformNotSupportedException)
                {
                    File.Copy(savedPath, backupPath, true);
                    File.Copy(tempPath, savedPath, true);
                    File.Delete(tempPath);
                }
            }
            else
            {
                File.Move(tempPath, savedPath);
            }
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"[JsonManager] 저장 실패: {savedPath}\n{exception.Message}");
            try { if (File.Exists(tempPath)) File.Delete(tempPath); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            return false;
        }
    }

    /// <summary>
    /// 정상 백업에서 복구할 때는 기존 원본 파일만 교체하고 .bak 파일은 그대로 보존합니다.
    /// 일반 저장 경로를 사용하면 손상된 원본이 정상 백업을 덮을 수 있으므로 복구 전용 경로를 사용합니다.
    /// </summary>
    private static void RestorePrimaryWithoutRotatingBackup(string savedPath, string jsonData)
    {
        string tempPath = savedPath + ".restore.tmp";

        try
        {
            File.WriteAllText(tempPath, jsonData);
            if (File.Exists(savedPath))
            {
                try
                {
                    File.Replace(tempPath, savedPath, null, true);
                }
                catch (PlatformNotSupportedException)
                {
                    File.Copy(tempPath, savedPath, true);
                    File.Delete(tempPath);
                }
            }
            else
            {
                File.Move(tempPath, savedPath);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError($"[JsonManager] 백업 복구 파일 쓰기 실패: {savedPath}\n{exception.Message}");
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    private static string GetPath(string fileName)
    {
        return Path.Combine(GetSaveFolderPath(), fileName + ".json");
    }

    private static string GetBackupPath(string savedPath)
    {
        return savedPath + ".bak";
    }

    public static string GetSaveFolderPath()
    {
#if UNITY_EDITOR
        string rootPath = Application.dataPath;
#else
        string rootPath = Application.persistentDataPath;
#endif
        string folderPath = Path.Combine(rootPath, "Saves");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        return folderPath;
    }

    public static void ClearSavedData()
    {
        string folderPath = GetSaveFolderPath();
        if (!Directory.Exists(folderPath)) return;

        foreach (string file in Directory.GetFiles(folderPath))
        {
            File.Delete(file);
        }

        Debug.Log("모든 저장 데이터를 초기화했습니다.");
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> datalist;
    }
}

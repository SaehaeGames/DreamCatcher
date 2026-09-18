using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonManager
{
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
        string savedPath = GetPath(fileName);
        if (TryLoadJson(savedPath, out T data)) return data;

        string backupPath = GetBackupPath(savedPath);
        if (TryLoadJson(backupPath, out data))
        {
            Debug.LogWarning($"[JsonManager] 백업 저장 데이터를 복구합니다: {fileName}");
            RestorePrimaryWithoutRotatingBackup(savedPath, JsonUtility.ToJson(data, true));
            return data;
        }

        data = LoadDefaultData<T>(fileName);
        SaveData(fileName, data);
        return data;
    }

    public List<T> LoadDataList<T>(string fileName) where T : new()
    {
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
        WriteJsonSafely(GetPath(fileName), JsonUtility.ToJson(data, true));
    }

    public void SaveDataList<T>(string fileName, List<T> dataList)
    {
        Wrapper<T> wrapper = new Wrapper<T> { datalist = dataList ?? new List<T>() };
        WriteJsonSafely(GetPath(fileName), JsonUtility.ToJson(wrapper, true));
    }

    private static bool TryLoadJson<T>(string path, out T data)
    {
        data = default(T);
        if (!File.Exists(path)) return false;

        try
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json)) return false;

            data = JsonUtility.FromJson<T>(json);
            return !ReferenceEquals(data, null);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[JsonManager] 저장 데이터 읽기 실패: {path}\n{exception.Message}");
            return false;
        }
    }

    private static void WriteJsonSafely(string savedPath, string jsonData)
    {
        string tempPath = savedPath + ".tmp";
        string backupPath = GetBackupPath(savedPath);

        try
        {
            File.WriteAllText(tempPath, jsonData);
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
        }
        catch (Exception exception)
        {
            Debug.LogError($"[JsonManager] 저장 실패: {savedPath}\n{exception.Message}");
            if (File.Exists(tempPath)) File.Delete(tempPath);
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

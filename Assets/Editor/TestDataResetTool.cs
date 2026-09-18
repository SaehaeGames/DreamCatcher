using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class TestDataResetTool
{
    [MenuItem("DreamCatcher/데이터 테이블 갱신")]
    public static void UpdateGameDataTables()
    {
        EditorDataTableUpdateService.UpdateAll();
    }

    [MenuItem("DreamCatcher/테스트 데이터 초기화/튜토리얼 초기화")]
    public static void ResetTutorial()
    {
        if (!EditorUtility.DisplayDialog("튜토리얼 초기화",
            "튜토리얼 진행도, 먹이 해금, 새 도감, 깃털과 드림캐쳐 인벤토리를 초기화하고\n멧비둘기 깃털 2개를 지급합니다.\n계속하시겠습니까?", "초기화", "취소"))
            return;

        PlayerDataManager playerDataManager = new PlayerDataManager();
        playerDataManager.Load();
        playerDataManager.ResetTutorialProgress();

        FeatherDataManager featherDataManager = new FeatherDataManager();
        featherDataManager.ResetData(new Dictionary<int, int> { { 0, 2 } });

        DreamCatcherInventoryDataManager inventoryDataManager = new DreamCatcherInventoryDataManager();
        inventoryDataManager.ResetData();

        DreamCatcherDataManager dreamCatcherDataManager = new DreamCatcherDataManager(inventoryDataManager);
        dreamCatcherDataManager.ResetData();

        RackDataManager rackDataManager = new RackDataManager();
        rackDataManager.ResetData();

        AssetDatabase.Refresh();
        Debug.Log("[TestDataReset] 튜토리얼 진행도, 해금, 새 도감과 깃털 초기화 완료 (멧비둘기 깃털 2개 지급)");
    }

    [MenuItem("DreamCatcher/테스트 데이터 초기화/상점 + 인테리어 전체 초기화")]
    public static void ResetAll()
    {
        if (!EditorUtility.DisplayDialog("데이터 초기화",
            "상점 구매 데이터와 인테리어 데이터를 모두 초기화합니다.\n계속하시겠습니까?", "초기화", "취소"))
            return;

        ResetGoodsData();
        ResetInteriorData();
        AssetDatabase.Refresh();
        Debug.Log("[TestDataReset] 상점 + 인테리어 데이터 초기화 완료");
    }

    [MenuItem("DreamCatcher/테스트 데이터 초기화/상점 구매 데이터만 초기화")]
    public static void ResetGoodsOnly()
    {
        ResetGoodsData();
        AssetDatabase.Refresh();
        Debug.Log("[TestDataReset] 상점 구매 데이터 초기화 완료");
    }

    [MenuItem("DreamCatcher/테스트 데이터 초기화/인테리어 데이터만 초기화")]
    public static void ResetInteriorOnly()
    {
        ResetInteriorData();
        AssetDatabase.Refresh();
        Debug.Log("[TestDataReset] 인테리어 데이터 초기화 완료");
    }

    [MenuItem("DreamCatcher/테스트 데이터 초기화/먹이 + 타이머 데이터 초기화")]
    public static void ResetRackDataOnly()
    {
        if (!EditorUtility.DisplayDialog("데이터 초기화",
            "횃대 먹이 및 타이머 데이터를 초기화합니다.\n계속하시겠습니까?", "초기화", "취소"))
            return;

        RackDataManager rackDataManager = new RackDataManager();
        rackDataManager.ResetData();
        AssetDatabase.Refresh();
        Debug.Log("[TestDataReset] 먹이 + 타이머 데이터 초기화 완료");
    }

    private static void ResetGoodsData()
    {
        GoodsDataManager goodsDataManager = new GoodsDataManager();
        goodsDataManager.ResetData();
    }

    private static void ResetInteriorData()
    {
        InteriorDataManager interiorDataManager = new InteriorDataManager();
        interiorDataManager.ResetData();
    }
}

/// <summary>
/// Play Mode와 GameManager에 의존하지 않고 에디터에서 데이터 테이블 자산을 갱신합니다.
/// 각 ScriptableObject가 가진 시트 설정을 읽어 TSV를 내려받고 dataList에 반영합니다.
/// </summary>
internal static class EditorDataTableUpdateService
{
    private static readonly Type[] DataAssetTypes =
    {
        typeof(BirdInfo_Data),
        typeof(DreamInfo_Data),
        typeof(StoreInfo_Data),
        typeof(InteriorInfo_Data),
        typeof(QuestInfo_Data),
        typeof(StoryScriptInfo_Data),
        typeof(StorySceneInfo_Data)
    };

    private static readonly Queue<UpdateJob> PendingJobs = new Queue<UpdateJob>();
    private static UnityWebRequest activeRequest;
    private static UnityWebRequestAsyncOperation activeOperation;
    private static UpdateJob activeJob;
    private static int totalJobCount;
    private static int completedJobCount;
    private static int failedJobCount;
    private static bool isUpdating;

    private sealed class UpdateJob
    {
        public ScriptableObject Asset { get; }
        public string AssetPath { get; }
        public string Address { get; }
        public string Range { get; }
        public long Worksheet { get; }

        public UpdateJob(ScriptableObject asset, string assetPath, string address, string range, long worksheet)
        {
            Asset = asset;
            AssetPath = assetPath;
            Address = address;
            Range = range;
            Worksheet = worksheet;
        }
    }

    public static void UpdateAll()
    {
        if (isUpdating)
        {
            EditorUtility.DisplayDialog("데이터 테이블 갱신", "이미 데이터 테이블을 갱신하고 있습니다.", "확인");
            return;
        }

        PendingJobs.Clear();
        completedJobCount = 0;
        failedJobCount = 0;

        foreach (Type assetType in DataAssetTypes)
        {
            if (TryCreateJob(assetType, out UpdateJob job))
            {
                PendingJobs.Enqueue(job);
            }
            else
            {
                failedJobCount++;
            }
        }

        totalJobCount = DataAssetTypes.Length;
        if (PendingJobs.Count == 0)
        {
            Debug.LogError("[DataTable] 갱신할 ScriptableObject 자산을 찾지 못했습니다.");
            return;
        }

        isUpdating = true;
        EditorApplication.update += PollRequest;
        AssemblyReloadEvents.beforeAssemblyReload += CancelUpdate;
        EditorApplication.quitting += CancelUpdate;
        Debug.Log($"[DataTable] 에디터 데이터 테이블 갱신을 시작합니다. 대상: {PendingJobs.Count}개");
        StartNextRequest();
    }

    private static bool TryCreateJob(Type assetType, out UpdateJob job)
    {
        job = null;
        string[] guids = AssetDatabase.FindAssets($"t:{assetType.Name}");
        if (guids.Length == 0)
        {
            Debug.LogError($"[DataTable] {assetType.Name} 자산을 찾지 못했습니다.");
            return false;
        }

        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        ScriptableObject asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType) as ScriptableObject;
        if (asset == null)
        {
            Debug.LogError($"[DataTable] {assetPath} 자산을 불러오지 못했습니다.");
            return false;
        }

        BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
        FieldInfo addressField = assetType.GetField("spreadSheetAddress", flags);
        FieldInfo rangeField = assetType.GetField("spreadSheetRange", flags);
        FieldInfo worksheetField = assetType.GetField("spreadSheetWorksheet", flags);
        if (addressField == null || rangeField == null || worksheetField == null)
        {
            Debug.LogError($"[DataTable] {assetType.Name}의 스프레드시트 설정 필드를 찾지 못했습니다.");
            return false;
        }

        string address = addressField.GetValue(null) as string;
        string range = rangeField.GetValue(null) as string;
        long worksheet = Convert.ToInt64(worksheetField.GetValue(null), CultureInfo.InvariantCulture);
        if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(range))
        {
            Debug.LogError($"[DataTable] {assetType.Name}의 스프레드시트 설정이 비어 있습니다.");
            return false;
        }

        job = new UpdateJob(asset, assetPath, address, range, worksheet);
        return true;
    }

    private static void StartNextRequest()
    {
        if (PendingJobs.Count == 0)
        {
            FinishUpdate();
            return;
        }

        activeJob = PendingJobs.Dequeue();
        string url = SpreadSheetManager.GetTSVAddress(
            activeJob.Address, activeJob.Range, activeJob.Worksheet);
        activeRequest = UnityWebRequest.Get(url);
        activeOperation = activeRequest.SendWebRequest();
        UpdateProgressBar();
    }

    private static void PollRequest()
    {
        if (!isUpdating || activeOperation == null || !activeOperation.isDone) return;

        try
        {
            if (activeRequest.result != UnityWebRequest.Result.Success)
            {
                failedJobCount++;
                Debug.LogError($"[DataTable] {activeJob.Asset.name} 갱신 실패: {activeRequest.error}");
            }
            else if (TryApplyDownloadedData(activeJob, activeRequest.downloadHandler.text))
            {
                completedJobCount++;
                Debug.Log($"[DataTable] {activeJob.Asset.name} 갱신 완료 ({activeJob.AssetPath})");
            }
            else
            {
                failedJobCount++;
            }
        }
        catch (Exception exception)
        {
            failedJobCount++;
            Debug.LogError($"[DataTable] {activeJob.Asset.name} 갱신 처리 중 예외가 발생했습니다.\n{exception}");
        }
        finally
        {
            activeRequest.Dispose();
            activeRequest = null;
            activeOperation = null;
            activeJob = null;
        }

        StartNextRequest();
    }

    private static bool TryApplyDownloadedData(UpdateJob job, string tsv)
    {
        FieldInfo dataListField = job.Asset.GetType().GetField(
            "dataList", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (dataListField == null || !dataListField.FieldType.IsGenericType)
        {
            Debug.LogError($"[DataTable] {job.Asset.name}의 dataList 필드를 찾지 못했습니다.");
            return false;
        }

        Type rowType = dataListField.FieldType.GetGenericArguments()[0];
        IList parsedRows = Activator.CreateInstance(typeof(List<>).MakeGenericType(rowType)) as IList;
        if (parsedRows == null)
        {
            Debug.LogError($"[DataTable] {rowType.Name} 목록을 생성하지 못했습니다.");
            return false;
        }

        string[] lines = tsv.Split('\n');
        foreach (string rawLine in lines)
        {
            string line = rawLine.TrimEnd('\r');
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (!TryParseRow(rowType, line.Split('\t'), out object row))
            {
                Debug.LogError($"[DataTable] {job.Asset.name} 파싱을 중단했습니다. 행: {line}");
                return false;
            }
            parsedRows.Add(row);
        }

        if (parsedRows.Count == 0)
        {
            Debug.LogError($"[DataTable] {job.Asset.name}에서 유효한 데이터 행을 받지 못했습니다.");
            return false;
        }

        dataListField.SetValue(job.Asset, parsedRows);
        EditorUtility.SetDirty(job.Asset);
        return true;
    }

    private static bool TryParseRow(Type rowType, string[] values, out object row)
    {
        row = Activator.CreateInstance(rowType);
        FieldInfo[] fields = rowType
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .OrderBy(field => field.MetadataToken)
            .ToArray();

        if (values.Length > fields.Length)
        {
            Debug.LogError($"[DataTable] {rowType.Name} 필드 수({fields.Length})보다 열 수({values.Length})가 많습니다.");
            return false;
        }

        for (int i = 0; i < values.Length; i++)
        {
            string value = values[i];
            if (string.IsNullOrEmpty(value)) continue;

            try
            {
                fields[i].SetValue(row, ParseValue(fields[i].FieldType, value));
            }
            catch (Exception exception)
            {
                Debug.LogError($"[DataTable] {rowType.Name}.{fields[i].Name} 파싱 실패: '{value}'\n{exception.Message}");
                return false;
            }
        }

        return true;
    }

    private static object ParseValue(Type type, string value)
    {
        if (type == typeof(string)) return value;
        if (type == typeof(int)) return int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
        if (type == typeof(float)) return float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        if (type == typeof(bool)) return bool.Parse(value);
        if (type.IsEnum) return Enum.Parse(type, value, true);
        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }

    private static void UpdateProgressBar()
    {
        int processedCount = completedJobCount + failedJobCount;
        float progress = totalJobCount == 0 ? 1f : (float)processedCount / totalJobCount;
        string tableName = activeJob == null ? "완료 처리" : activeJob.Asset.name;
        EditorUtility.DisplayProgressBar("데이터 테이블 갱신", tableName, progress);
    }

    private static void FinishUpdate()
    {
        EditorApplication.update -= PollRequest;
        AssemblyReloadEvents.beforeAssemblyReload -= CancelUpdate;
        EditorApplication.quitting -= CancelUpdate;
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        isUpdating = false;

        string message = $"성공 {completedJobCount}개, 실패 {failedJobCount}개";
        Debug.Log($"[DataTable] 에디터 데이터 테이블 갱신 종료: {message}");
        EditorUtility.DisplayDialog("데이터 테이블 갱신 완료", message, "확인");
    }

    private static void CancelUpdate()
    {
        EditorApplication.update -= PollRequest;
        AssemblyReloadEvents.beforeAssemblyReload -= CancelUpdate;
        EditorApplication.quitting -= CancelUpdate;
        activeRequest?.Dispose();
        activeRequest = null;
        activeOperation = null;
        activeJob = null;
        PendingJobs.Clear();
        EditorUtility.ClearProgressBar();
        isUpdating = false;
    }
}

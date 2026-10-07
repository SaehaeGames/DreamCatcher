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
        if (!CanResetStoreData()) return;
        if (!EditorUtility.DisplayDialog("데이터 초기화",
            "상점 구매 데이터와 인테리어 데이터를 모두 초기화합니다.\n계속하시겠습니까?", "초기화", "취소"))
            return;

        ResetStoreData(StoreResetMode.All);
    }

    [MenuItem("DreamCatcher/테스트 데이터 초기화/상점 구매 데이터만 초기화")]
    public static void ResetGoodsOnly()
    {
        if (CanResetStoreData()) ResetStoreData(StoreResetMode.Goods);
    }

    [MenuItem("DreamCatcher/테스트 데이터 초기화/인테리어 데이터만 초기화")]
    public static void ResetInteriorOnly()
    {
        if (CanResetStoreData()) ResetStoreData(StoreResetMode.Interior);
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

    /// <summary>상점 초기화의 범위입니다. 공통 저장 흐름은 같고 기본값을 적용하는 대상만 달라집니다.</summary>
    private enum StoreResetMode
    {
        All,
        Goods,
        Interior
    }

    [MenuItem("DreamCatcher/테스트 데이터 초기화/상점 + 인테리어 전체 초기화", true)]
    [MenuItem("DreamCatcher/테스트 데이터 초기화/상점 구매 데이터만 초기화", true)]
    [MenuItem("DreamCatcher/테스트 데이터 초기화/인테리어 데이터만 초기화", true)]
    private static bool ValidateStoreResetMenus()
    {
        // 실행 중인 GameManager의 메모리와 저장 파일이 서로 달라지지 않도록 실행/전환 중에는 차단합니다.
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    /// <summary>메뉴 검증을 거치지 않는 직접 호출에서도 실행 중 초기화를 차단합니다.</summary>
    private static bool CanResetStoreData()
    {
        if (ValidateStoreResetMenus()) return true;
        Debug.LogWarning("[TestDataReset] 상점/인테리어 초기화는 Play Mode를 종료한 뒤 실행해 주세요.");
        return false;
    }

    /// <summary>매니저가 기본값을 구성하고, 보유/배치 정합성을 맞춘 두 저장 파일을 함께 확정합니다.</summary>
    private static void ResetStoreData(StoreResetMode mode)
    {
        if (!CanResetStoreData()) return;
        try
        {
            // 1. 이전에 확정된 저장을 먼저 복구하고 사용할 상점 테이블 자산을 검증합니다.
            JsonManager jsonManager = new JsonManager();
            if (!jsonManager.RecoverPendingTransaction())
                throw new InvalidOperationException("이전 저장 작업을 복구할 수 없어 초기화를 중단합니다.");

            string[] tableGuids = AssetDatabase.FindAssets("t:StoreInfo_Data");
            if (tableGuids.Length != 1)
                throw new InvalidOperationException("상점 테이블(StoreInfo_Data) 자산이 정확히 하나 있어야 합니다.");
            StoreInfo_Data table = AssetDatabase.LoadAssetAtPath<StoreInfo_Data>(AssetDatabase.GUIDToAssetPath(tableGuids[0]));
            if (table == null || table.dataList == null || table.dataList.Count == 0)
                throw new InvalidOperationException("상점 테이블 데이터가 비어 있습니다.");

            // 2. 실행 중인 GameManager와 별개인 스냅샷을 만듭니다. 기본값 설정은 데이터 매니저가 담당합니다.
            GoodsDataManager goods = new GoodsDataManager();
            if (mode == StoreResetMode.Interior) goods.Load();
            else goods.ResetData(false);

            InteriorDataManager interior = new InteriorDataManager();
            if (mode == StoreResetMode.Goods)
            {
                interior.Load();
                InteriorDataManager defaults = new InteriorDataManager();
                defaults.ResetData(false);
                ResetDefaultDevelopmentInterior(interior, defaults, table);
            }
            else interior.ResetData(false);

            // 3. 보조도구를 낮췄을 때 높은 레벨 외형이 남거나, 인테리어만 초기화한 뒤 재구매가 막히지 않게 합니다.
            SynchronizeDevelopmentInterior(goods, interior, table);

            // 4. 변경 후 두 파일을 한 저장 작업으로 확정합니다. 실패한 스냅샷은 기존 매니저에 반영하지 않습니다.
            var entries = new List<JsonManager.SaveEntry> { goods.CreateSaveEntry(), interior.CreateSaveEntry() };
            if (!jsonManager.TrySaveBatch(entries))
                throw new InvalidOperationException("초기화 데이터를 저장할 수 없습니다.");
            AssetDatabase.Refresh();
            Debug.Log(mode == StoreResetMode.Goods
                ? "[TestDataReset] 상점 구매 및 기본 보조도구 인테리어 초기화 확정 (별/바다 구매 보존)"
                : mode == StoreResetMode.Interior
                    ? "[TestDataReset] 인테리어 초기화 확정 (현재 보조도구 레벨의 보유/배치 유지)"
                    : "[TestDataReset] 상점 + 인테리어 데이터 초기화 확정");
        }
        catch (Exception exception)
        {
            Debug.LogError("[TestDataReset] 상점 데이터 초기화 실패: " + exception.Message);
            EditorUtility.DisplayDialog("상점 데이터 초기화 실패", exception.Message, "확인");
        }
    }

    /// <summary>상점 레벨만 초기화할 때 기본 보조도구 외형만 교체하고 별/바다 및 다른 장식의 보유 상태는 유지합니다.</summary>
    private static void ResetDefaultDevelopmentInterior(InteriorDataManager interior, InteriorDataManager defaults,
        StoreInfo_Data table)
    {
        if (interior.dataList == null || defaults.dataList == null || defaults.dataList.Count == 0)
            throw new InvalidOperationException("기본 인테리어 데이터가 누락되었습니다.");

        foreach (InteriorData item in defaults.dataList)
        {
            if (item == null || !table.TryGetItem(item.storeinfo_id, out StoreInfo_Object product)) continue;
            bool isDevelopment = product.category == StoreItemCategory.Vase || product.category == StoreItemCategory.Box
                || product.category == StoreItemCategory.Thread;
            if (product.theme != ItemTheme.Default || !isDevelopment) continue;
            var replacement = new InteriorData(item.id, item.isHaving, item.isAdjusting, item.storeinfo_id);
            int index = interior.dataList.FindIndex(saved => saved != null && saved.storeinfo_id == item.storeinfo_id);
            if (index >= 0) interior.dataList[index] = replacement;
            else interior.dataList.Add(replacement);
        }
    }

    /// <summary>Vase/Box/Thread의 레벨 이하 외형은 보유하고 현재 레벨 외형 하나만 적용하도록 맞춥니다.</summary>
    private static void SynchronizeDevelopmentInterior(GoodsDataManager goods, InteriorDataManager interior,
        StoreInfo_Data table)
    {
        if (interior.dataList == null || interior.dataList.Count == 0)
            throw new InvalidOperationException("인테리어 초기화에 필요한 기본 데이터가 없습니다.");

        // 중복 저장 항목을 그대로 두면 같은 상품이 여러 번 적용될 수 있어 저장 전에 차단합니다.
        var savedIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (InteriorData item in interior.dataList)
            if (item == null || string.IsNullOrWhiteSpace(item.storeinfo_id) || !savedIds.Add(item.storeinfo_id))
                throw new InvalidOperationException("인테리어 저장 항목이 누락되었거나 상품 ID가 중복됩니다.");

        if (goods.dataList == null || goods.dataList.Count == 0)
            throw new InvalidOperationException("보조도구 초기화에 필요한 기본 데이터가 없습니다.");
        var goodsIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (GoodsData item in goods.dataList)
            if (item == null || string.IsNullOrWhiteSpace(item.id) || !goodsIds.Add(item.id))
                throw new InvalidOperationException("보조도구 저장 항목이 누락되었거나 ID가 중복됩니다.");

        StoreItemCategory[] categories = { StoreItemCategory.Rack, StoreItemCategory.Vase, StoreItemCategory.Box, StoreItemCategory.Thread };
        foreach (StoreItemCategory category in categories)
        {
            // 1. 보조도구의 현재 레벨과 해당 레벨의 기본 외형이 모두 존재하는지 확인합니다.
            string categoryName = category.ToString();
            GoodsData current = goods.GetValidatedGoodsData(categoryName);
            int expectedCount = category == StoreItemCategory.Rack ? 2 : 1;
            if (current == null || goods.GetGoodsDataList(categoryName).Count != expectedCount)
                throw new InvalidOperationException(categoryName + " 보조도구의 레벨 데이터가 올바르지 않습니다.");
            string currentId = table.GetIDByCategoryAndLevel(categoryName, current.level);
            if (!table.TryGetItem(currentId, out _))
                throw new InvalidOperationException(categoryName + " 현재 레벨에 해당하는 상점 상품이 없습니다.");
            // 횃대는 보유/배치 인테리어 항목이 없는 기존 구조이므로 레벨 검증만 수행합니다.
            if (category == StoreItemCategory.Rack) continue;
            if (interior.GetInteriorDataByStoreInfoId(currentId) == null)
                throw new InvalidOperationException(categoryName + " 현재 레벨에 해당하는 기본 인테리어가 없습니다.");

            // 2. 레벨과 보유 상태를 함께 맞춥니다. 다른 테마의 같은 카테고리는 보유를 유지하되 배치만 해제합니다.
            foreach (InteriorData item in interior.dataList)
            {
                if (!table.TryGetItem(item.storeinfo_id, out StoreInfo_Object product) || product.category != category) continue;
                if (product.theme == ItemTheme.Default)
                {
                    item.isHaving = product.level >= 0 && product.level <= current.level;
                    item.isAdjusting = product.level == current.level;
                }
                else item.isAdjusting = false;
            }
        }
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

        // 상점은 기존 상품을 덮어쓰기 전에 기본 저장 항목과 함께 검증합니다.
        // 잘못된 테이블을 받더라도 현재 정상 자산은 유지합니다.
        if (job.Asset is StoreInfo_Data)
        {
            List<string> errors = StoreProductValidationService.ValidateTable((List<StoreInfo_Object>)parsedRows);
            if (errors.Count > 0)
            {
                foreach (string error in errors) Debug.LogError("[StoreValidation] " + error);
                return false;
            }
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

        // 테이블 저장 후에는 이미지와 버튼 등 Store 씬에 연결된 참조까지 검사합니다.
        List<string> validationErrors = StoreProductValidationService.ValidateProject(true);
        foreach (string error in validationErrors) Debug.LogWarning("[StoreValidation] " + error);
        string validationMessage = validationErrors.Count == 0 ? "상점 상품 검증 통과"
            : $"상점 검증: {validationErrors.Count}개 확인 필요 (Console 참조)";
        string message = $"성공 {completedJobCount}개, 실패 {failedJobCount}개\n{validationMessage}";
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

/// <summary>
/// 상점 테이블, 기본 저장 데이터와 명시적인 상품 UI 연결을 읽기 전용으로 검증합니다.
/// 데이터 검증은 전달받은 목록만 사용하므로 씬이나 실제 저장 파일 없이도 호출할 수 있습니다.
/// </summary>
internal static class StoreProductValidationService
{
    private const string StoreScenePath = "Assets/Scenes/Store.unity";
    private const string StoreTablePath = "Assets/Scripts/DataTable/ScritableObjects/StoreInfo.asset";
    private static readonly StoreItemCategory[] DevelopmentCategories =
    {
        StoreItemCategory.Rack, StoreItemCategory.Vase, StoreItemCategory.Box, StoreItemCategory.Thread
    };

    [MenuItem("DreamCatcher/상점/상품 데이터 및 UI 검증")]
    public static void ValidateMenu()
    {
        List<string> errors = ValidateProject(true);
        if (errors.Count == 0)
        {
            Debug.Log("[StoreValidation] 상품 데이터, 기본 저장 항목과 UI 연결에 문제가 없습니다.");
            return;
        }
        foreach (string error in errors) Debug.LogWarning("[StoreValidation] " + error);
        Debug.LogWarning($"[StoreValidation] 확인할 항목: {errors.Count}개");
    }

    /// <summary>자산과 기본 JSON을 읽고, 요청하면 Store 씬의 UI 연결까지 검사합니다. 저장 함수는 호출하지 않습니다.</summary>
    public static List<string> ValidateProject(bool includeUi)
    {
        // 1. 프로젝트 기본 데이터만 읽습니다. 사용자 저장 로드와 복구를 호출하지 않습니다.
        var errors = new List<string>();
        StoreInfo_Data table = AssetDatabase.LoadAssetAtPath<StoreInfo_Data>(StoreTablePath);
        if (table == null)
        {
            errors.Add("상점 테이블 자산을 찾을 수 없습니다: " + StoreTablePath);
            return errors;
        }
        errors.AddRange(ValidateTable(table.dataList));
        if (!includeUi) return errors;

        // 2. 열린 씬을 우선 사용하고, Edit Mode에서만 임시로 추가 로드합니다.
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(StoreScenePath);
        UnityEngine.SceneManagement.Scene originalActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        bool openedForValidation = false;
        try
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    errors.Add("Play Mode에서는 Store 씬을 먼저 열어야 UI를 검증할 수 있습니다. 데이터 검증은 완료했습니다.");
                    return errors;
                }
                scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                    StoreScenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                openedForValidation = true;
            }

            // 3. 초기화나 자동 연결을 실행하지 않고 저장된 참조 자체를 검사합니다.
            StoreData[] stores = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<StoreData>(true)).ToArray();
            if (stores.Length != 1) errors.Add($"Store 씬의 StoreData는 1개여야 합니다. 현재: {stores.Length}개");
            foreach (StoreData store in stores) errors.AddRange(ValidateUi(store, table.dataList));
        }
        catch (Exception exception)
        {
            errors.Add("Store 씬 UI 검증을 완료할 수 없습니다: " + exception.Message);
        }
        finally
        {
            // 4. 이 검증이 연 씬만 저장 없이 닫고 이전 활성 씬을 유지합니다.
            if (openedForValidation && scene.IsValid())
            {
                if (originalActiveScene.IsValid() && originalActiveScene.isLoaded)
                    UnityEngine.SceneManagement.SceneManager.SetActiveScene(originalActiveScene);
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
            }
        }
        return errors;
    }

    /// <summary>갱신 전의 임시 상품 목록도 기본 JSON과 비교할 수 있도록 자산 적용과 검증을 분리합니다.</summary>
    public static List<string> ValidateTable(IReadOnlyList<StoreInfo_Object> products)
    {
        var errors = new List<string>();
        GoodsDataManager goods = ReadDefault<GoodsDataManager>(Constants.GoodsDataFile, errors);
        InteriorDataManager interior = ReadDefault<InteriorDataManager>(Constants.InteriorDataFile, errors);
        errors.AddRange(ValidateData(products, goods?.dataList, interior?.dataList));
        return errors;
    }

    private static T ReadDefault<T>(string fileName, List<string> errors) where T : class
    {
        TextAsset asset = Resources.Load<TextAsset>("DefaultJsonData/" + fileName);
        if (asset == null)
        {
            errors.Add("기본 JSON을 찾을 수 없습니다: " + fileName);
            return null;
        }
        try
        {
            T data = JsonUtility.FromJson<T>(asset.text);
            if (data == null) errors.Add("기본 JSON 내용이 비어 있습니다: " + fileName);
            return data;
        }
        catch (Exception exception)
        {
            errors.Add("기본 JSON을 읽을 수 없습니다: " + fileName + " / " + exception.Message);
            return null;
        }
    }

    /// <summary>상품 ID·가격·종류·레벨·효과와 기본 저장 항목의 대응을 검사하고 원본은 변경하지 않습니다.</summary>
    public static List<string> ValidateData(IReadOnlyList<StoreInfo_Object> products,
        IReadOnlyList<GoodsData> defaultGoods, IReadOnlyList<InteriorData> defaultInterior)
    {
        var errors = new List<string>();
        if (products == null || products.Count == 0)
        {
            errors.Add("상점 상품 목록이 비어 있습니다.");
            return errors;
        }

        // 1. 공통 필드와 중복 ID를 먼저 검사합니다.
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (StoreInfo_Object product in products)
        {
            if (product == null) { errors.Add("상점 상품 목록에 null 항목이 있습니다."); continue; }
            string label = string.IsNullOrWhiteSpace(product.id) ? "이름 없는 상품" : product.id;
            if (string.IsNullOrWhiteSpace(product.id)) errors.Add("상품 ID가 비어 있습니다.");
            else if (!ids.Add(product.id)) errors.Add("상품 ID가 중복되었습니다: " + product.id);
            if (product.gold < 0) errors.Add(label + ": 가격은 음수일 수 없습니다.");
            if (product.level < 0) errors.Add(label + ": 레벨은 음수일 수 없습니다.");
            if (!Enum.IsDefined(typeof(StoreItemCategory), product.category)) errors.Add(label + ": 상품 종류 값이 올바르지 않습니다.");
            if (!Enum.IsDefined(typeof(ItemTheme), product.theme)) errors.Add(label + ": 테마 값이 올바르지 않습니다.");
        }

        // 2. 보조도구는 기본 레벨부터 판매 가능한 마지막 레벨까지 중복이나 중간 누락이 없어야 합니다.
        foreach (StoreItemCategory category in DevelopmentCategories)
        {
            StoreInfo_Object[] levels = products.Where(product => product != null
                && product.theme == ItemTheme.Default && product.category == category).OrderBy(product => product.level).ToArray();
            if (levels.Length == 0) { errors.Add(category + ": 기본 테마의 보조도구 상품이 없습니다."); continue; }
            var seenLevels = new HashSet<int>();
            int expectedLevel = 0;
            foreach (StoreInfo_Object product in levels)
            {
                if (!seenLevels.Add(product.level)) errors.Add(category + ": 같은 레벨 상품이 중복되었습니다: " + product.level);
                else
                {
                    if (product.level != expectedLevel) errors.Add(category + ": 레벨이 연속하지 않습니다. 필요한 레벨: " + expectedLevel);
                    expectedLevel = product.level == int.MaxValue ? int.MaxValue : product.level + 1;
                }
                if (string.IsNullOrWhiteSpace(product.effect)) errors.Add(product.id + ": 보조도구 효과가 비어 있습니다.");
                else if (category != StoreItemCategory.Thread
                    && (!int.TryParse(product.effect, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) || value < 0))
                    errors.Add(product.id + ": 숫자 효과는 0 이상의 정수여야 합니다.");
            }
        }

        // 3. 초기 저장 데이터에 필요한 상품이 없으면 새 설치에서도 구매할 수 없으므로 대응을 검사합니다.
        ValidateDefaultGoods(defaultGoods, errors);
        if (defaultInterior == null || defaultInterior.Count == 0) errors.Add("기본 인테리어 저장 항목이 비어 있습니다.");
        else
        {
            var interiorIds = new HashSet<string>(StringComparer.Ordinal);
            var itemIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (InteriorData item in defaultInterior)
            {
                if (item == null) { errors.Add("기본 인테리어 목록에 null 항목이 있습니다."); continue; }
                if (string.IsNullOrWhiteSpace(item.id) || !itemIds.Add(item.id)) errors.Add("기본 인테리어 ID가 비어 있거나 중복되었습니다: " + item.id);
                if (string.IsNullOrWhiteSpace(item.storeinfo_id) || !interiorIds.Add(item.storeinfo_id))
                    errors.Add("기본 인테리어의 상품 ID가 비어 있거나 중복되었습니다: " + item.storeinfo_id);
                else if (!ids.Contains(item.storeinfo_id)) errors.Add("기본 인테리어가 없는 상품을 참조합니다: " + item.storeinfo_id);
            }
            foreach (StoreInfo_Object product in products)
            {
                if (product == null || string.IsNullOrWhiteSpace(product.id)) continue;
                // 기본 장식은 테이블에만 존재할 수 있습니다. 구매·보유 상태를 사용하는 상품에만 저장 항목을 요구합니다.
                bool needsSavedItem = product.theme != ItemTheme.Default
                    || (product.category != StoreItemCategory.Rack && DevelopmentCategories.Contains(product.category));
                if (needsSavedItem && !interiorIds.Contains(product.id))
                    errors.Add("기본 인테리어 저장 항목이 없습니다: " + product.id);
            }
        }
        return errors;
    }

    private static void ValidateDefaultGoods(IReadOnlyList<GoodsData> goods, List<string> errors)
    {
        if (goods == null || goods.Count == 0) { errors.Add("기본 보조도구 저장 항목이 비어 있습니다."); return; }
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (GoodsData item in goods)
        {
            if (item == null) { errors.Add("기본 보조도구 목록에 null 항목이 있습니다."); continue; }
            if (string.IsNullOrWhiteSpace(item.id) || !ids.Add(item.id)) errors.Add("기본 보조도구 ID가 비어 있거나 중복되었습니다: " + item.id);
        }
        foreach (StoreItemCategory category in DevelopmentCategories)
        {
            GoodsData[] items = goods.Where(item => item != null
                && string.Equals(item.category?.Trim(), category.ToString(), StringComparison.OrdinalIgnoreCase)).ToArray();
            int expectedCount = category == StoreItemCategory.Rack ? 2 : 1;
            if (items.Length != expectedCount) errors.Add(category + ": 기본 보조도구 저장 항목은 " + expectedCount + "개여야 합니다.");
            if (items.Any(item => item.level != 0)) errors.Add(category + ": 기본 보조도구의 레벨은 0이어야 합니다.");
        }
    }

    /// <summary>상품별 명시적 UI 참조와 이미지, 테마별 상품 ID가 테이블과 일치하는지 검사합니다.</summary>
    public static List<string> ValidateUi(StoreData store, IReadOnlyList<StoreInfo_Object> products)
    {
        var errors = new List<string>();
        if (store == null) { errors.Add("상점 UI 담당 객체가 없습니다."); return errors; }
        if (products == null) { errors.Add("UI와 비교할 상점 상품 목록이 없습니다."); return errors; }
        var buttons = new HashSet<BuyButtonInfo>();
        var categories = new HashSet<StoreItemCategory>();
        IReadOnlyList<StoreData.ProductView> development = store.DevelopmentViews;
        if (development == null || development.Count != DevelopmentCategories.Length)
            errors.Add("보조도구 UI는 Rack·Vase·Box·Thread의 4개 표시 대상을 연결해야 합니다.");
        if (development != null)
            foreach (StoreData.ProductView view in development)
            {
                ValidateView(view, "보조도구", true, buttons, errors);
                if (view == null) continue;
                StoreInfo_Object baseProduct = products.FirstOrDefault(product => product != null && product.id == view.productId);
                if (baseProduct == null || baseProduct.theme != ItemTheme.Default || baseProduct.level != 0
                    || !DevelopmentCategories.Contains(baseProduct.category))
                {
                    errors.Add("보조도구 UI에는 Rack·Vase·Box·Thread의 기본 레벨 상품 ID를 연결해야 합니다: " + view.productId);
                    continue;
                }
                if (!categories.Add(baseProduct.category)) errors.Add("보조도구 UI의 카테고리가 중복되었습니다: " + baseProduct.category);
                StoreInfo_Object[] levels = products.Where(product => product != null && product.theme == ItemTheme.Default
                    && product.category == baseProduct.category).ToArray();
                foreach (StoreInfo_Object product in levels)
                    if (view.levelImages == null || product.level < 0 || product.level >= view.levelImages.Length
                        || view.levelImages[product.level] == null) errors.Add(product.id + ": 해당 레벨의 상품 이미지가 없습니다.");
            }
        foreach (StoreItemCategory category in DevelopmentCategories)
            if (!categories.Contains(category)) errors.Add(category + ": 보조도구 UI가 없습니다.");

        foreach (ItemTheme theme in new[] { ItemTheme.Sea, ItemTheme.Star })
        {
            IReadOnlyList<StoreData.ProductView> views = store.GetInteriorViews(theme);
            var ids = new HashSet<string>(StringComparer.Ordinal);
            if (views == null) errors.Add(theme + ": 인테리어 UI 목록이 없습니다.");
            else foreach (StoreData.ProductView view in views)
            {
                ValidateView(view, theme.ToString(), true, buttons, errors);
                if (view == null) continue;
                if (string.IsNullOrWhiteSpace(view.productId) || !ids.Add(view.productId))
                    errors.Add(theme + ": UI 상품 ID가 비어 있거나 중복되었습니다: " + view.productId);
                StoreInfo_Object product = products.FirstOrDefault(item => item != null && item.id == view.productId);
                if (product == null || product.theme != theme) errors.Add(theme + ": UI 상품 ID가 테이블 테마와 맞지 않습니다: " + view.productId);
            }
            foreach (StoreInfo_Object product in products)
                if (product != null && product.theme == theme && product.isButtonActive && !ids.Contains(product.id))
                    errors.Add(theme + ": 판매 상품의 UI 연결이 없습니다: " + product.id);
        }

        ValidateView(store.SpecialFeedView, "특제먹이", false, buttons, errors);
        if (store.SpecialFeedView != null && store.SpecialFeedView.productId != StorePurchaseService.SpecialFeedProductId)
            errors.Add("특제먹이 UI의 상품 ID가 올바르지 않습니다.");
        return errors;
    }

    private static void ValidateView(StoreData.ProductView view, string label, bool needsSoldOut,
        HashSet<BuyButtonInfo> buttons, List<string> errors)
    {
        if (view == null) { errors.Add(label + ": 상품 UI 연결이 없습니다."); return; }
        label += " " + (string.IsNullOrEmpty(view.productId) ? "ID 없음" : view.productId);
        if (view.content == null) errors.Add(label + ": 상품 루트가 없습니다.");
        if (view.button == null) errors.Add(label + ": 구매 버튼이 없습니다.");
        else
        {
            if (!buttons.Add(view.button)) errors.Add(label + ": 다른 상품과 구매 버튼을 공유하고 있습니다.");
            if (view.button.GetComponent<UnityEngine.UI.Button>() == null) errors.Add(label + ": 구매 버튼의 Button 컴포넌트가 없습니다.");
        }
        if (view.image == null) errors.Add(label + ": 상품 이미지가 없습니다.");
        else if (view.image.sprite == null) errors.Add(label + ": 상품 이미지의 Sprite가 없습니다.");
        if (view.price == null) errors.Add(label + ": 가격 Text가 없습니다.");
        if (view.priceRoot == null) errors.Add(label + ": 가격 영역 루트가 없습니다.");
        if (view.description == null) errors.Add(label + ": 효과/설명 Text가 없습니다.");
        if (needsSoldOut && view.soldOut == null) errors.Add(label + ": 품절 표시가 없습니다.");
    }
}

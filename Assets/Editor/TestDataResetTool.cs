using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestDataResetTool
{
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

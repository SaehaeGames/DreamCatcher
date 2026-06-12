using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TestDataResetTool
{
    private static readonly string GoodsDataPath = Application.dataPath + "/Saves/GoodsDataFile.json";
    private static readonly string InteriorDataPath = Application.dataPath + "/Saves/InteriorDataFile.json";
    private static readonly string RackDataPath = Application.dataPath + "/Saves/RackDataFile.json";
    private static readonly string PlayerDataPath = Application.dataPath + "/Saves/PlayerDataFile.json";
    private static readonly string FeatherDataPath = Application.dataPath + "/Saves/FeatherNumInfo.json";
    private static readonly string DreamCatcherInventoryPath = Application.dataPath + "/Saves/DreamCatcherInventoryDataFile.json";
    private static readonly string DreamCatcherListPath = Application.dataPath + "/Saves/DreamCatcherListData.json";

    [System.Serializable]
    private class PlayerDataEntry { public string id; public string dataName; public float dataNumber; }
    [System.Serializable]
    private class PlayerDataWrapper { public List<PlayerDataEntry> dataList = new List<PlayerDataEntry>(); }

    [MenuItem("DreamCatcher/테스트 데이터 초기화/튜토리얼 초기화")]
    public static void ResetTutorial()
    {
        if (!EditorUtility.DisplayDialog("튜토리얼 초기화",
            "튜토리얼 진행도와 인벤토리(깃털, 드림캐쳐)를 초기화하고\n멧비둘기 깃털 1개를 지급합니다.\n계속하시겠습니까?", "초기화", "취소"))
            return;

        ResetTutorialProgress();
        ResetFeatherDataForTutorial();
        ResetDreamCatcherInventoryData();
        ResetDreamCatcherListData();
        ResetRackData();
        AssetDatabase.Refresh();
        Debug.Log("[TestDataReset] 튜토리얼 초기화 완료 (멧비둘기 깃털 1개 지급)");
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

        ResetRackData();
        AssetDatabase.Refresh();
        Debug.Log("[TestDataReset] 먹이 + 타이머 데이터 초기화 완료");
    }

    private static void ResetRackData()
    {
        string json = "{\n    \"datalist\": []\n}";
        File.WriteAllText(RackDataPath, json);
    }

    private static void ResetTutorialProgress()
    {
        if (!File.Exists(PlayerDataPath))
        {
            Debug.LogWarning("[TestDataReset] PlayerDataFile.json 없음 - 스킵");
            return;
        }
        string json = File.ReadAllText(PlayerDataPath);
        PlayerDataWrapper data = JsonUtility.FromJson<PlayerDataWrapper>(json);
        foreach (var entry in data.dataList)
        {
            if (entry.dataName == "nowSceneNum") entry.dataNumber = 0;
            else if (entry.dataName == "nowQuestNum") entry.dataNumber = 0;
            else if (entry.dataName == "questAccepted") entry.dataNumber = 0;
        }
        File.WriteAllText(PlayerDataPath, JsonUtility.ToJson(data, true));
    }

    private static void ResetFeatherDataForTutorial()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine("    \"birdCnt\": 16,");
        sb.AppendLine("    \"featherList\": [");
        for (int i = 0; i < 16; i++)
        {
            // 멧비둘기(JS_2000, index 0)만 깃털 3개 지급 (테스트용)
            int featherNum = (i == 0) ? 3 : 0;
            int appear = (i == 0) ? 1 : 0;
            string comma = (i < 15) ? "," : "";
            sb.AppendLine($"        {{ \"bird_id\": \"JS_{2000 + i}\", \"feather_number\": {featherNum}, \"appear\": {appear} }}{comma}");
        }
        sb.AppendLine("    ]");
        sb.Append("}");
        File.WriteAllText(FeatherDataPath, sb.ToString());
    }

    private static void ResetDreamCatcherInventoryData()
    {
        File.WriteAllText(DreamCatcherInventoryPath, "{\n    \"dataList\": []\n}");
    }

    private static void ResetDreamCatcherListData()
    {
        File.WriteAllText(DreamCatcherListPath, "{\n    \"dataList\": []\n}");
    }

    private static void ResetGoodsData()
    {
        string json = @"{
    ""dataList"": [
        { ""id"": ""JS_3000"", ""name"": ""RackFront"",    ""category"": ""Rack\r"",         ""level"": 0, ""interior_id"": ""SO_3000"" },
        { ""id"": ""JS_3001"", ""name"": ""RackBack"",     ""category"": ""Rack\r"",         ""level"": 0, ""interior_id"": ""SO_3001"" },
        { ""id"": ""JS_3002"", ""name"": ""Vase"",         ""category"": ""Vase\r"",         ""level"": 0, ""interior_id"": ""SO_3002"" },
        { ""id"": ""JS_3003"", ""name"": ""Poket"",        ""category"": ""Box\r"",          ""level"": 0, ""interior_id"": ""SO_3003"" },
        { ""id"": ""JS_3004"", ""name"": ""Thread"",       ""category"": ""Thread\r"",       ""level"": 0, ""interior_id"": ""SO_3004"" },
        { ""id"": ""JS_3005"", ""name"": ""Wallpaper"",    ""category"": ""Wallpaper\r"",    ""level"": 0, ""interior_id"": ""SO_3005"" },
        { ""id"": ""JS_3006"", ""name"": ""FrameUpper"",   ""category"": ""Wallpaper\r"",    ""level"": 0, ""interior_id"": ""SO_3006"" },
        { ""id"": ""JS_3007"", ""name"": ""FrameLower"",   ""category"": ""Wallpaper\r"",    ""level"": 0, ""interior_id"": ""SO_3007"" },
        { ""id"": ""JS_3008"", ""name"": ""Desk"",         ""category"": ""Wallpaper\r"",    ""level"": 0, ""interior_id"": ""SO_3008"" },
        { ""id"": ""JS_3009"", ""name"": ""Garland"",      ""category"": ""Garland\r"",      ""level"": 0, ""interior_id"": ""SO_3009"" },
        { ""id"": ""JS_3010"", ""name"": ""WindowFrame"",  ""category"": ""WindowFrame\r"",  ""level"": 0, ""interior_id"": ""SO_3010"" },
        { ""id"": ""JS_3011"", ""name"": ""Pad"",          ""category"": ""Pad\r"",          ""level"": 0, ""interior_id"": ""SO_3011"" },
        { ""id"": ""JS_3012"", ""name"": ""Fillpen"",      ""category"": ""Fillpen\r"",      ""level"": 0, ""interior_id"": ""SO_3012"" },
        { ""id"": ""JS_3013"", ""name"": ""Prob1"",        ""category"": ""Prob1\r"",        ""level"": 0, ""interior_id"": ""SO_3013"" },
        { ""id"": ""JS_3014"", ""name"": ""CrystalBall"",  ""category"": ""CrystalBall\r"", ""level"": 0, ""interior_id"": ""SO_3014"" },
        { ""id"": ""JS_3015"", ""name"": ""Telescope"",    ""category"": ""Telescope\r"",    ""level"": 0, ""interior_id"": ""SO_3015"" },
        { ""id"": ""JS_3016"", ""name"": ""Prob2"",        ""category"": ""Prob2"",          ""level"": 0, ""interior_id"": ""SO_3016"" }
    ]
}";
        File.WriteAllText(GoodsDataPath, json);
    }

    private static void ResetInteriorData()
    {
        var ids = new (string id, string storeinfoId)[]
        {
            ("JS_4000", "SO_4003"), ("JS_4001", "SO_4004"), ("JS_4002", "SO_4005"), ("JS_4003", "SO_4006"),
            ("JS_4004", "SO_4007"), ("JS_4005", "SO_4008"), ("JS_4006", "SO_4009"), ("JS_4007", "SO_4010"),
            ("JS_4008", "SO_4011"), ("JS_4009", "SO_4012"), ("JS_4010", "SO_4013"), ("JS_4011", "SO_4014"),
            ("JS_4012", "SO_4015"), ("JS_4013", "SO_4017"), ("JS_4014", "SO_4020"), ("JS_4015", "SO_4023"),
            ("JS_4016", "SO_4026"), ("JS_4017", "SO_4029"), ("JS_4018", "SO_4032"), ("JS_4019", "SO_4035"),
            ("JS_4020", "SO_4038"), ("JS_4021", "SO_4041"), ("JS_4022", "SO_4018"), ("JS_4023", "SO_4021"),
            ("JS_4024", "SO_4024"), ("JS_4025", "SO_4027"), ("JS_4026", "SO_4030"), ("JS_4027", "SO_4033"),
            ("JS_4028", "SO_4036"), ("JS_4029", "SO_4039"), ("JS_4030", "SO_4042"),
        };

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("{\n    \"dataList\": [");
        for (int i = 0; i < ids.Length; i++)
        {
            string comma = i < ids.Length - 1 ? "," : "";
            sb.AppendLine($"        {{ \"id\": \"{ids[i].id}\", \"isHaving\": false, \"isAdjusting\": false, \"storeinfo_id\": \"{ids[i].storeinfoId}\" }}{comma}");
        }
        sb.AppendLine("    ]\n}");
        File.WriteAllText(InteriorDataPath, sb.ToString());
    }
}

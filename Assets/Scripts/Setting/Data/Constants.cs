using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    // 상수 관리 클래스

    // 데이터 저장 파일 이름
    public const string PlayerDataFile = "PlayerDataFile";
    public const string GoodsDataFile = "GoodsDataFile";
    public const string RackDataFile = "RackDataFile";
    public const string InteriorDataFile = "InteriorDataFile";
    public const string QuestDataFile = "QuestDataFile";

    public const string FeatherDataFile = "FeatherDataFile";
    public const string DreamCatcherDataFile = "DreamCatcherListData";

    // 게임 오브젝트 태그 이름
    public const string Tag_TopBar = "TopBar";
    public const string Tag_AudioManager = "AudioManager";
    public const string Tag_GoodsManager = "GoodsManager";

    // playerData 내부 데이터
    public const string PlayerData_DreamMarble = "DreamMarble";
    public const string PlayerData_Gold = "Gold";
    public const string PlayerData_SpecialFeed = "SpecialFeed";
    public const string PlayerData_BGM = "BGM";
    public const string PlayerData_Effect = "Effect";
    public const string PlayerData_BGMMute = "BGMMute";
    public const string PlayerData_EffectMute = "EffectMute";
    public const string PlayerData_NowSceneNum = "nowSceneNum";
    public const string PlayerData_NowQuestNum = "nowQuestNum";
    public const string PlayerData_QuestAccepted = "questAccepted";

    // goodsData 내부 데이터
    public const string GoodsData_Rack = "Rack";
    public const string GoodsData_Vase = "Vase";
    public const string GoodsData_Box = "Box";
    public const string GoodsData_Thread = "Thread";
    public const int GoodsData_Rack_MaxLevel = 2;
    public const int GoodsData_Vase_MaxLevel = 3;
    public const int GoodsData_Box_MaxLevel = 3;
    public const int GoodsData_Thread_MaxLevel = 4;


    // storeData 관련 데이터
    //public const int DevelopmentItemMaxLevel = 2;
}

// 열거형 변수
public enum StoreType { Development, Interior, SpecialProduct };    // 상점 페이지 카테고리 열거형
public enum ItemTheme { Default, Sea, Star };    // 상점 아이템 타입 열거형
public enum StoreItemCategory  // 상점 아이템 카테고리 열거형
{
    Rack,
    Vase,
    Box,
    Thread,
    Wallpaper,
    Garland,
    WindowFrame,
    Pad,
    Fillpen,
    Prop1,
    CrystalBall,
    Telescope,
    Prop2
}

public enum InteriorItemCategory
{
    Wallpaper,
    GarLane,
    WindowFrame,
    Pad,
    Fillpen,
    Prop1,
    CrystallBall,
    Telescope,
    Prop2
}

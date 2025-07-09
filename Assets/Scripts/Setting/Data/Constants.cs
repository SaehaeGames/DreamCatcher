using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    // ��� ���� Ŭ����

    // ������ ���� ���� �̸�
    public const string PlayerDataFile = "PlayerDataFile";
    public const string GoodsDataFile = "GoodsDataFile";
    public const string RackDataFile = "RackDataFile";
    public const string InteriorDataFile = "InteriorDataFile";
    public const string QuestDataFile = "QuestDataFile";

    public const string FeatherDataFile = "FeatherDataFile";
    public const string DreamCatcherDataFile = "DreamCatcherListData";

    // ���� ������Ʈ �±� �̸�
    public const string Tag_TopBar = "TopBar";
    public const string Tag_AudioManager = "AudioManager";
    public const string Tag_GoodsManager = "GoodsManager";

    // playerData ���� ������
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

    // goodsData ���� ������
    public const string GoodsData_Rack = "Rack";
    public const string GoodsData_Vase = "Vase";
    public const string GoodsData_Box = "Box";
    public const string GoodsData_Thread = "Thread";
    public const int GoodsData_Rack_MaxLevel = 2;
    public const int GoodsData_Vase_MaxLevel = 3;
    public const int GoodsData_Box_MaxLevel = 3;
    public const int GoodsData_Thread_MaxLevel = 4;


    // storeData ���� ������
    //public const int DevelopmentItemMaxLevel = 2;
}

// ������ ����
public enum StoreType { Development, Interior, SpecialProduct };    // ���� ������ ī�װ� ������
public enum ItemTheme { Default, Sea, Star };    // ���� ������ Ÿ�� ������
public enum StoreItemCategory  // ���� ������ ī�װ� ������
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

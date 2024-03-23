using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    public PlayerData(string _name, float _number)
    {
        dataName = _name;
        dataNumber = _number;
    }

    public string dataName;       //상품 이름
    public float dataNumber;         //상품 레벨 또는 상품 id
}

public class PlayerDataContainer
{
    public PlayerDataContainer()
    {
        ResetPlayerData();
    }

    public int dataCount;   //데이터 종류 개수
    public PlayerData[] dataList;   //상단바 데이터 리스트(꿈구슬, 골드, 특제먹이)

    public void ResetPlayerData()
    {
        dataCount = 8;  //소프트코딩으로 수정 가능할지 고민

        dataList = new PlayerData[dataCount];

        dataList[0] = new PlayerData("DreamMarble", 0);
        dataList[1] = new PlayerData("Gold", 30000);
        dataList[2] = new PlayerData("SpecialFeed", 100);
        dataList[3] = new PlayerData("BGM", 1f);
        dataList[4] = new PlayerData("Effect", 1f);
        dataList[5] = new PlayerData("BGMMute", 0);
        dataList[6] = new PlayerData("EffectMute", 0);
        dataList[7] = new PlayerData("nowSceneNum", 0);
    }
}

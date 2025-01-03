using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class PlayerData
{
    public string dataName;          //상품 이름
    public float dataNumber;         //상품 레벨 또는 상품 id

    public PlayerData(string name, float number)
    {
        dataName = name;
        dataNumber = number;
    }
}

public class PlayerDataManager
{
    public List<PlayerData> dataList;

    public PlayerDataManager()
    {
        dataList = new List<PlayerData>();
        ResetData();
    }
     
    public void ResetData()
    {
        if (dataList != null)
            dataList.Clear();

        dataList = new List<PlayerData>()
        {
            /*new PlayerData("DreamMarble", 0),
            new PlayerData("Gold", 30000),
            new PlayerData("SpecialFeed", 100),
            new PlayerData("BGM", 1f),
            new PlayerData("Effect", 1f),
            new PlayerData("BGMMute", 0),
            new PlayerData("EffectMute", 0),
            new PlayerData("nowSceneNum", 0)*/

            new PlayerData(Constants.PlayerData_DreamMarble, 0),
            new PlayerData(Constants.PlayerData_Gold, 30000),
            new PlayerData(Constants.PlayerData_SpecialFeed, 100),
            new PlayerData(Constants.PlayerData_BGM, 1f),
            new PlayerData(Constants.PlayerData_Effect, 1f),
            new PlayerData(Constants.PlayerData_BGMMute, 0),
            new PlayerData(Constants.PlayerData_EffectMute, 0),
            new PlayerData(Constants.PlayerData_NowSceneNum, 0)
        };
    }

    public PlayerData GetPlayerData(string dataName)
    {
        PlayerData getData = dataList.FirstOrDefault(x => x.dataName == dataName);

        if (getData != null) 
        {
            Debug.Log(getData.dataName+"이 반환됨");
            return getData;
        }
        Debug.Log("null이 반환됨");
        return null;
    }
}

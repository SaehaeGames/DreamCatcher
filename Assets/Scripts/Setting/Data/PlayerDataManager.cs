using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class PlayerData
{
    public string id;
    public string dataName;          //상품 이름
    public float dataNumber;         //상품 레벨 또는 상품 id

    public PlayerData(string _id, string name, float number)
    {
        this.id = _id;
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

        int startId = 5000;
        dataList = new List<PlayerData>()
        {
            new PlayerData("JS_"+startId++, Constants.PlayerData_DreamMarble, 0),
            new PlayerData("JS_"+startId++, Constants.PlayerData_Gold, 30000),
            new PlayerData("JS_"+startId++, Constants.PlayerData_SpecialFeed, 100),
            new PlayerData("JS_" + startId ++, Constants.PlayerData_BGM, 1f),
            new PlayerData("JS_" + startId ++, Constants.PlayerData_Effect, 1f),
            new PlayerData("JS_" + startId ++, Constants.PlayerData_BGMMute, 0),
            new PlayerData("JS_" + startId ++, Constants.PlayerData_EffectMute, 0),
            new PlayerData("JS_" + startId ++, Constants.PlayerData_NowSceneNum, 0),
            new PlayerData("JS_" + startId ++, Constants.PlayerData_NowQuestNum, 1),
            new PlayerData("JS_" + startId ++, Constants.PlayerData_QuestAccepted, 0)
        };
    }

    public PlayerData GetPlayerDataByDataName(string _dataName)
    {
        PlayerData getData = dataList.FirstOrDefault(x => x.dataName == _dataName);

        if (getData != null) 
        {
            Debug.Log(getData.dataName+"이 반환됨");
            return getData;
        }
        Debug.Log("null이 반환됨");
        return null;
    }

    public PlayerData GetPlayerDataById(string _id)
    {
        PlayerData getData = dataList.FirstOrDefault(x => x.id == _id);
        if (getData != null)
        {
            return getData;
        }
        else
        {
            return null;
        }
    }
}

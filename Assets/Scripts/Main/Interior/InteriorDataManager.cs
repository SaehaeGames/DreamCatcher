using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Security.Cryptography;

[Serializable]
public class InteriorData
{

    public string id;           //인테리어 아이템 id
    public bool isHaving;         //인테리어 아이템 보유중 여부
    public bool isAdjusting;      //인테리어 아이템 적용중 여부
    public string storeinfo_id; //storeinfo_data의 아이디를 참조

    public InteriorData(string _id, bool _isHaving, bool _isAdjusting, string _storeinfo_id)
    {
        this.id = _id;
        this.isHaving = _isHaving;
        this.isAdjusting = _isAdjusting;
        this.storeinfo_id = _storeinfo_id;
    }
}

public class InteriorDataManager
{
    public List<InteriorData> dataList;
    private JsonManager jsonManager = new JsonManager();

    public InteriorDataManager()
    {
        dataList = new List<InteriorData>();
    }

    public void ResetData()
    {
        InteriorDataManager defaultData = jsonManager.LoadDefaultData<InteriorDataManager>(Constants.InteriorDataFile);
        dataList = defaultData.dataList ?? new List<InteriorData>();
        Save();
    }

    public void Save()
    {
        jsonManager.SaveData(Constants.InteriorDataFile, this);
    }

    public InteriorData GetInteriorDataByStoreInfoId(string _storeinfo_id)
    {
        InteriorData getData = dataList.FirstOrDefault(x => x.storeinfo_id == _storeinfo_id);
        if (getData != null)
            return getData;
        else
            return null;
    }

    public InteriorData GetInteriorDataById(string _id)
    {
        InteriorData getData = dataList.FirstOrDefault(x => x.id == _id);
        if (getData != null)
            return getData;
        else
            return null;
    }
}

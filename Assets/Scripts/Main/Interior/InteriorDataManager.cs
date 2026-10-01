using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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

    public void Load()
    {
        InteriorDataManager loadedData = jsonManager.LoadData<InteriorDataManager>(Constants.InteriorDataFile);
        dataList = loadedData.dataList ?? new List<InteriorData>();
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

    /// <summary>보유 상태와 자동 배치를 복사본에 적용하여 구매 확정 전 원본을 유지합니다.</summary>
    public bool TryCreatePurchaseData(StoreInfo_Object product, StoreInfo_Data table, bool adjust,
        out InteriorDataManager purchasedData)
    {
        purchasedData = null;
        if (product == null || table == null) return false;
        InteriorData current = GetInteriorDataByStoreInfoId(product.id);
        if (current == null || current.isHaving) return false;
        purchasedData = CopyData();
        purchasedData.GetInteriorDataByStoreInfoId(product.id).isHaving = true;
        if (adjust) purchasedData.SetAdjusting(product, table);
        return true;
    }

    /// <summary>보유한 아이템만 배치하고 저장 실패 시 기존 배치 상태를 유지합니다.</summary>
    public bool TryAdjustItem(string id, StoreInfo_Data table)
    {
        if (table == null || !table.TryGetItem(id, out StoreInfo_Object product)) return false;
        InteriorData item = GetInteriorDataByStoreInfoId(id);
        if (item == null || !item.isHaving) return false;
        InteriorDataManager adjusted = CopyData();
        adjusted.SetAdjusting(product, table);
        if (!jsonManager.TrySaveData(Constants.InteriorDataFile, adjusted)) return false;
        dataList = adjusted.dataList;
        return true;
    }

    private void SetAdjusting(StoreInfo_Object product, StoreInfo_Data table)
    {
        foreach (InteriorData item in dataList)
        {
            if (table.TryGetItem(item.storeinfo_id, out StoreInfo_Object other) && other.category == product.category)
                item.isAdjusting = item.storeinfo_id == product.id;
        }
    }

    private InteriorDataManager CopyData()
    {
        var copy = new InteriorDataManager();
        foreach (InteriorData item in dataList)
            if (item != null) copy.dataList.Add(new InteriorData(item.id, item.isHaving, item.isAdjusting, item.storeinfo_id));
        return copy;
    }

    public JsonManager.SaveEntry CreateSaveEntry()
    {
        return JsonManager.CreateSaveEntry(Constants.InteriorDataFile, this);
    }

    public void ApplyPurchaseData(InteriorDataManager purchasedData)
    {
        dataList = purchasedData.dataList;
    }
}

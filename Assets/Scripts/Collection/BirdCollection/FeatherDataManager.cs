using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum RemoveFeatherResult
{
    Success,
    InvalidIndex,
    InvalidId,
    InvalidAmount,
    DataNull,
    Empty,
    NotEnough
}

public class FeatherDataManager
{
    private JsonManager jsonManager = new JsonManager();
    private FeatherDataModel featherDataModel = new FeatherDataModel();
    public IReadOnlyList<FeatherData> featherDataList => featherDataModel.datalist;
    private const int featherDataCount = 16;

    public void AddFeather(string id, int amount)
    {
        var data = featherDataModel.datalist.Find(x => x.id == id);
        if (data == null) return;

        data.count += amount;

        if (data.appear == 0)
        {
            data.appear = 1;
        }

        Save();
    }

    public void AddFeather(int index, int amount) 
    {
        if (index < 0 || index >= featherDataCount)
        {
            Debug.LogError($"[FeatherDataManager] Invalid index: {index}");
            return;
        }

        var data = featherDataModel.datalist[index];
        if (data == null) return;

        data.count += amount;

        if (data.appear == 0)
        {
            data.appear = 1;
        }

        Save();
    }

    public RemoveFeatherResult RemoveFeather(int index, int amount)
    {
        if (index < 0 || index >= featherDataCount)
        {
            Debug.LogError($"[FeatherDataManager] Invalid index: {index}");
            return RemoveFeatherResult.InvalidIndex;
        }

        if (amount <= 0)
        {
            Debug.LogWarning($"[FeatherDataManager] Invalid remove amount: {amount} | index: {index}");
            return RemoveFeatherResult.InvalidAmount;
        }

        var data = featherDataModel.datalist[index];
        if (data == null)
        {
            Debug.LogError($"[FeatherDataManager] Data is null at index: {index}");
            return RemoveFeatherResult.DataNull;
        }

        if (data.count <= 0)
        {
            Debug.LogWarning($"[FeatherDataManager] Feather count is already 0 | id: {data.id}");
            return RemoveFeatherResult.Empty;
        }

        if (data.count < amount)
        {
            Debug.LogWarning($"[FeatherDataManager] Not enough feathers | id: {data.id}, current: {data.count}, remove: {amount}");
            return RemoveFeatherResult.NotEnough;
        }
        
        data.count -= amount;
        
        Save();

        return RemoveFeatherResult.Success;
    }

    public RemoveFeatherResult RemoveFeather(string id, int amount)
    {
        var data = featherDataModel.datalist.Find(x => x.id == id);

        if (data == null)
        {
            Debug.LogError($"[FeatherDataManager] Invalid id: {id}");
            return RemoveFeatherResult.InvalidId;
        }

        if (amount <= 0)
        {
            Debug.LogWarning($"[FeatherDataManager] Invalid remove amount: {amount} | id: {id}");
            return RemoveFeatherResult.InvalidAmount;
        }

        if (data.count <= 0)
        {
            Debug.LogWarning($"[FeatherDataManager] Feather count is already 0 | id: {data.id}");
            return RemoveFeatherResult.Empty;
        }

        if (data.count < amount)
        {
            Debug.LogWarning($"[FeatherDataManager] Not enough feathers | id: {data.id}, current: {data.count}, remove: {amount}");
            return RemoveFeatherResult.NotEnough;
        }

        data.count -= amount;

        Save();

        return RemoveFeatherResult.Success;
    }

    public void ResetData()
    {
        if (featherDataModel.datalist != null)
            featherDataModel.datalist.Clear();
        for (int i = 0; i < featherDataCount; i++)
        {
            featherDataModel.datalist.Add(new FeatherData("JS_" + (2000 + i), 0, 0));
        }
        Save();
    }

    public bool IsFeatherAppeared(int index)
    {
        if (index < 0 || index >= featherDataModel.datalist.Count)
        {
            Debug.LogError($"[FeatherDataManager] Invalid index: {index}");
            return false;
        }

        var data = featherDataModel.datalist[index];
        if (data == null)
        {
            Debug.LogError($"[FeatherDataManager] Data is null at index: {index}");
            return false;
        }

        return data.appear >= 1;
    }

    public bool IsFeatherAppeared(string id)
    {
        var data = featherDataModel.datalist.Find(x => x.id == id);

        if (data == null)
        {
            Debug.LogError($"[FeatherDataManager] Invalid id: {id}");
            return false;
        }

        return data.appear >= 1;
    }

    public int GetFeatherCount(int index)
    {
        if (index < 0 || index >= featherDataModel.datalist.Count)
        {
            Debug.LogError($"[FeatherDataManager] Invalid index: {index}");
            return 0;
        }

        var data = featherDataModel.datalist[index];
        if (data == null)
        {
            Debug.LogError($"[FeatherDataManager] Data is null at index: {index}");
            return 0;
        }

        return data.count;
    }

    public int GetFeatherCount(string id)
    {
        var data = featherDataModel.datalist.Find(x => x.id == id);

        if (data == null)
        {
            Debug.LogError($"[FeatherDataManager] Invalid id: {id}");
            return 0;
        }

        return data.count;
    }

    public void UnlockFeather(int index)
    {
        if (index < 0 || index >= featherDataModel.datalist.Count)
        {
            Debug.LogError($"[FeatherDataManager] Invalid index: {index}");
            return;
        }

        var data = featherDataModel.datalist[index];
        if (data == null)
        {
            Debug.LogError($"[FeatherDataManager] Data is null at index: {index}");
            return;
        }

        if (data.appear >= 1)
            return; // 이미 등장

        data.appear++;
        Save();
    }

    public void UnlockFeather(string id)
    {
        var data = featherDataModel.datalist.Find(x => x.id == id);

        if (data == null)
        {
            Debug.LogError($"[FeatherDataManager] Invalid id: {id}");
            return;
        }

        if (data.appear >= 1)
            return;

        data.appear++;
        Save();
    }
    
    public int GetFeatherDataListCount()
    {
        return featherDataModel.datalist.Count;
    }

    public void Load()
    {
        featherDataModel = jsonManager.LoadData<FeatherDataModel>(Constants.FeatherDataFile);

        if (featherDataModel == null)
            featherDataModel = new FeatherDataModel();
    }

    public void Save()
    {
        jsonManager.SaveData(Constants.FeatherDataFile, featherDataModel);
    }
}
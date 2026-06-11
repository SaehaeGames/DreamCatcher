 using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;

public class DreamCatcherDataManager
{
    private JsonManager jsonManager = new JsonManager();
    private const int IdBase = 1000; // 아이디 시작 지점
    private DreamCatcherDataModel dreamCatcherDataModel = new DreamCatcherDataModel();
    public IReadOnlyList<DreamCatcher> dreamCatchersDataList => dreamCatcherDataModel.dataList;

    private DreamCatcherInventoryDataManager dreamCatcherInventoryDataManager;

    public DreamCatcherDataManager(DreamCatcherInventoryDataManager _dreamCatcherInventoryDataManager)
    {
        dreamCatcherInventoryDataManager = _dreamCatcherInventoryDataManager;
    }
    public DreamCatcher AddDreamCatcher(DreamCatcher dreamCatcher)
    {
        if (dreamCatcherInventoryDataManager == null)
        {
            Debug.LogError("[DreamCatcherDataManager] DreamCatcherInventoryDataManager is null");
            return null;
        }

        if (dreamCatcher == null)
            return null;

        dreamCatcherDataModel.dataList.Add(dreamCatcher);
        Save();

        // dreamCatcherInventoryData 동기화
        dreamCatcherInventoryDataManager.AddDreamCatcherInventoryData(dreamCatcher);
        dreamCatcherInventoryDataManager.Save();
        
        return dreamCatcher;
    }

    public bool RemoveDreamCatcher(int index)
    {
        if (dreamCatcherInventoryDataManager == null)
        {
            Debug.LogError("[DreamCatcherDataManager] DreamCatcherInventoryDataManager is null");
            return false;
        }

        if (dreamCatcherDataModel == null || dreamCatcherDataModel.dataList == null)
        {
            Debug.LogError("[DreamCatcherDataManager] DataModel is null.");
            return false;
        }

        if (index < 0 || index >= dreamCatcherDataModel.dataList.Count)
        {
            Debug.LogError($"[DreamCatcherDataManager] Index {index} is out of range.");
            return false;
        }

        // dreamCatcherInventoryData 동기화
        var dreamCatcher = dreamCatcherDataModel.dataList[index];

        dreamCatcherInventoryDataManager.RemoveDreamCatcherInventoryData(dreamCatcher);
        dreamCatcherInventoryDataManager.Save();

        // 드림캐쳐 썸네일 삭제
        DeleteThumbnail(dreamCatcher);

        // 드림캐쳐 삭제
        dreamCatcherDataModel.dataList.RemoveAt(index);
        Save();
        return true;
    }

    public bool RemoveDreamCatcher(string id)
    {
        if (dreamCatcherDataModel == null || dreamCatcherDataModel.dataList == null)
        {
            Debug.LogError("[DreamCatcherDataManager] DataModel is null.");
            return false;
        }

        int index = dreamCatcherDataModel.dataList.FindIndex(dc => dc.DCid == id);

        if (index < 0)
        {
            Debug.LogWarning($"[DreamCatcherDataManager] DreamCatcher id {id} not found.");
            return false;
        }

        return RemoveDreamCatcher(index);
    }

    // Index로 드림캐쳐 찾기
    public DreamCatcher GetDreamCatcherDataByIndex(int index)
    {
        if (index < 0 || index >= dreamCatcherDataModel.dataList.Count)
        {
            Debug.LogError($"[DreamCatcherDataManager] Index {index} is out of range.");
            return null;
        }

        return dreamCatcherDataModel.dataList[index];
    }

    // ID로 드림캐쳐 찾기
    public DreamCatcher GetDreamCatcherById(string id)
    {
        DreamCatcher result = dreamCatcherDataModel.dataList.FirstOrDefault(dc => dc.DCid == id);

        if(result == null)
        {
            Debug.LogWarning($"[DreamCatcherDataManager] DreamCatcher with ID {id} not found!");
        }

        return result;
    }

    public void SetDreamCatcherThumbnailPath(string id, string path)
    {
        DreamCatcher dreamCatcher = GetDreamCatcherById(id);
        if (dreamCatcher != null)
        {
            dreamCatcher.ThumbnailPath = path;
        }
    }

    public int GetDreamCatcherCount()
    {
        return dreamCatcherDataModel.dataList.Count;
    }
    // 드림캐쳐 데이터 리셋
    public void ResetDreamCatcherData()
    {
        dreamCatcherDataModel.dataList.Clear();
    }

    public void Load()
    {
        dreamCatcherDataModel = jsonManager.LoadData<DreamCatcherDataModel>(Constants.DreamCatcherDataFile);

        if (dreamCatcherDataModel == null)
            dreamCatcherDataModel = new DreamCatcherDataModel();
    }

    public void Save()
    {
        jsonManager.SaveData(Constants.DreamCatcherDataFile, dreamCatcherDataModel);
    }

    public string GetNextAvailableId()
    {
        HashSet<int> usedIds = new HashSet<int>();

        foreach (var dc in dreamCatcherDataModel.dataList)
        {
            if(dc.DCid.StartsWith("JS_"))
            {
                int id = int.Parse(dc.DCid.Substring(3));
                usedIds.Add(id);
            }
        }

        int idCandidate = IdBase;

        while (usedIds.Contains(idCandidate))
        {
            idCandidate++;
        }

        return "JS_" + idCandidate;
    }

    public void DeleteThumbnail(DreamCatcher dreamCatcher)
    {
        string path = dreamCatcher.GetThumbnailPath();

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

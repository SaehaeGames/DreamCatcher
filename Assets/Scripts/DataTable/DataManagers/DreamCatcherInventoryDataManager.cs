using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DreamCatcherInventoryDataManager
{
    private JsonManager jsonManager = new JsonManager();
    private DreamCatcherInventoryDataModel dreamCatcherInventoryDataModel = new DreamCatcherInventoryDataModel();
    public IReadOnlyList<DreamCatcherInventoryData> dreamCatcherInventoryDataList => dreamCatcherInventoryDataModel.dataList;

    public void AddDreamCatcherInventoryData(DreamCatcher dreamCatcher)
    {
        if (dreamCatcher == null)
            return;

        if (dreamCatcherInventoryDataModel == null || dreamCatcherInventoryDataModel.dataList == null)
        {
            Debug.LogError("[DreamCatcherInventoryDataManager] DataModel is null.");
            return;
        }

        var data = dreamCatcherInventoryDataModel.dataList.FirstOrDefault(
            x => x.TemplateHash == dreamCatcher.TemplateHash
        );

        if (data != null)
        {
            data.Number++;
        }
        else
        {
            dreamCatcherInventoryDataModel.dataList.Add(new DreamCatcherInventoryData(dreamCatcher));
        }
    }

    public bool RemoveDreamCatcherInventoryData(string templateHash)
    {
        if (dreamCatcherInventoryDataModel == null || dreamCatcherInventoryDataModel.dataList == null)
        {
            Debug.LogError("[DreamCatcherInventoryDataManager] DataModel is null.");
            return false;
        }

        var data = dreamCatcherInventoryDataModel.dataList.FirstOrDefault(
            x => x.TemplateHash == templateHash
        );

        if (data != null)
        {
            if (data.Number > 1)
            {
                data.Number--;
            }
            else
            {
                dreamCatcherInventoryDataModel.dataList.Remove(data);
            }
        }
        else
        {
            Debug.LogWarning("[DreamCatcherInventoryDataManager] templateHash to remove is wrong.");
            return false;
        }

        return true;
    }

    public void ResetData()
    {
        if (dreamCatcherInventoryDataModel.dataList != null)
            dreamCatcherInventoryDataModel.dataList.Clear();
    }

    public DreamCatcherInventoryData GetDreamCatcherInventoryData(string _templateHash)
    {
        DreamCatcherInventoryData getData = dreamCatcherInventoryDataModel.dataList.FirstOrDefault(x => x.TemplateHash == _templateHash);
        if (getData != null)
            return getData;
        else
            return null;
    }

    public void Load()
    {
        dreamCatcherInventoryDataModel = jsonManager.LoadData<DreamCatcherInventoryDataModel>(Constants.DreamCatcherInventoryDataFile);

        if (dreamCatcherInventoryDataModel == null)
            dreamCatcherInventoryDataModel = new DreamCatcherInventoryDataModel();
    }

    public void Save()
    {
        jsonManager.SaveData(Constants.DreamCatcherInventoryDataFile, dreamCatcherInventoryDataModel);
    }
}

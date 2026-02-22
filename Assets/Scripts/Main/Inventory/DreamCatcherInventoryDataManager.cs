using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DreamCatcherInventoryData
{
    public string TemplateHash;
    public string DisplayName;
    public string Description;
    public int Number;

    public DreamCatcherInventoryData() 
    {
        TemplateHash = "";
        DisplayName = "";
        Description = "";
        Number = 0;
    }

    public DreamCatcherInventoryData(string _TemplateHash, string _DisplayName, string _Description, int _Number)
    {
        TemplateHash = _TemplateHash;
        DisplayName = _DisplayName;
        Description = _Description;
        Number = _Number;
    }

    public DreamCatcherInventoryData(DreamCatcher dreamCatcher)
    {
        TemplateHash = dreamCatcher.TemplateHash;
        DisplayName = "드림 캐쳐";
        Description = DreamCatcherDescriptionBuilder.Build(dreamCatcher);
        Number = 1;
    }

    # region Set 함수

    public void SetNumber(int _Number)
    {
        Number = _Number;
    }

    public void SetDescription(string _Description)
    {
        Description = _Description;
    }

    public void SetTemplateHash(string _TemplateHash)
    {
        TemplateHash = _TemplateHash;
    }

    public void SetDisplayName(string _DisplayName)
    {
        DisplayName = _DisplayName;
    }
    # endregion

    # region Get 함수
    public int GetNumber()
    {
        return Number;
    }

    public string GetDescription()
    {
        return Description;
    }

    public string GetDisplayName()
    {
        return DisplayName;
    }

    public string GetTemplateHash()
    {
        return TemplateHash;
    }
    # endregion
}

public class DreamCatcherInventoryDataManager
{
    public List<DreamCatcherInventoryData> dataList;
    public DreamCatcherInventoryDataManager()
    {
        dataList = new List<DreamCatcherInventoryData>();
        ResetData();
    }

    public void AddDreamCatcherInventoryData(DreamCatcher dreamCatcher)
    {
        if (dreamCatcher == null)
            return;

        var data = dataList.FirstOrDefault(
            x => x.TemplateHash == dreamCatcher.TemplateHash
        );

        if (data != null)
        {
            data.Number += 1;
        }
        else
        {
            dataList.Add(new DreamCatcherInventoryData(dreamCatcher));
        }
    }

    public void ResetData()
    {
        if (dataList != null)
            dataList.Clear();
    }

    public DreamCatcherInventoryData GetDreamCatcherInventoryData(string _templateHash)
    {
        DreamCatcherInventoryData getData = dataList.FirstOrDefault(x => x.TemplateHash == _templateHash);
        if (getData != null)
            return getData;
        else
            return null;
    }
}

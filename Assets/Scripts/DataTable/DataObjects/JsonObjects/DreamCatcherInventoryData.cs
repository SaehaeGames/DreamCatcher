using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DreamCatcherInventoryData
{
    public string TemplateHash;
    public string DisplayName;
    public string Description;
    public List<string> DCids;
    public int Number => DCids?.Count ?? 0;

    public DreamCatcherInventoryData()
    {
        TemplateHash = "";
        DisplayName = "";
        Description = "";
        DCids = new List<string>();
    }

    public DreamCatcherInventoryData(string _TemplateHash, string _DisplayName, string _Description, int _Number)
    {
        TemplateHash = _TemplateHash;
        DisplayName = _DisplayName;
        Description = _Description;
        DCids = new List<string>();
    }

    public DreamCatcherInventoryData(DreamCatcher dreamCatcher)
    {
        TemplateHash = dreamCatcher.TemplateHash;
        DisplayName = "드림 캐쳐";
        Description = DreamCatcherDescriptionBuilder.Build(dreamCatcher);
        DCids = new List<string> { dreamCatcher.DCid };
    }

    # region Set 함수

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

    public IReadOnlyList<string> GetDCids()
    {
        return DCids;
    }
    # endregion
}

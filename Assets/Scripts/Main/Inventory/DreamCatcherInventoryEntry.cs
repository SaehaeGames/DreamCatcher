using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DreamCatcherInventoryEntry
{
    public string TemplateHash;
    public string DisplayName;
    public string Description;
    public int Number;

    public DreamCatcherInventoryEntry(string _TemplateHash, string _DisplayName, string _Description, int _Number)
    {
        TemplateHash = _TemplateHash;
        DisplayName = _DisplayName;
        Description = _Description;
        Number = _Number;
    }

    public DreamCatcherInventoryEntry(DreamCatcher dreamCatcher)
    {
        TemplateHash = dreamCatcher.TemplateHash;
        DisplayName = "드림 캐쳐";
        Description = DreamCatcherDescriptionBuilder.Build(dreamCatcher);
        Number = 0;
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

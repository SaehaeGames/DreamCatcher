using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public enum InventoryItemType
{
    None,
    Feather,
    DreamCatcher
}

[System.Serializable]
public class InventoryEntry
{
    public string id;
    public InventoryItemType type;
    public string name;
    public string description;
    public int count;
}

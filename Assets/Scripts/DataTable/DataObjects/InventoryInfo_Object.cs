using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryInfo_Object
{
    public string id;
    public string name;
    public string content;

    public InventoryInfo_Object()
    {
        id = "SO_0000";
        name = content = "";
    }

    public InventoryInfo_Object(string _id, string _name, string _content)
    {
        id = _id;
        name = _name;
        content = _content;
    }
}

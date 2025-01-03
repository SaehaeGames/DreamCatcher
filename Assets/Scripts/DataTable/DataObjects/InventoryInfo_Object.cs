using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryInfo_Object
{
    public int id;
    public string name;
    public string content;

    public InventoryInfo_Object()
    {
        id = 0;
        name = content = "";
    }

    public InventoryInfo_Object(int _id, string _name, string _content)
    {
        id = _id;
        name = _name;
        content = _content;
    }
}

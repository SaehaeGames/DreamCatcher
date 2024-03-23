using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryInfo_feather_Object
{
    public int id;
    public string name;
    public string contents;
    public int appear;
    public int number;

    public InventoryInfo_feather_Object()
    {
        id = appear = number = 0;
        name = contents = "";
    }

    public InventoryInfo_feather_Object(int _id, string _name, string _contents, int _appear, int _number)
    {
        id = _id;
        name = _name;
        contents = _contents;
        appear = _appear;
        number = _number;
    }
}

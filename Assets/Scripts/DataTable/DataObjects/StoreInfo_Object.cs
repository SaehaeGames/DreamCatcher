using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StoreInfo_Object
{
    public int id;
    public string name;
    public StoreItemCategory category;
    public string contents;
    public string effect;
    public int gold;
    
    public StoreInfo_Object()
    {
        id = gold = 0;
        name = contents = effect = "";
    }

    public StoreInfo_Object(int _id, string _name, StoreItemCategory _category, string _contents, string _effect, int _gold)
    {
        id = _id;
        name = _name;
        category = _category;
        contents = _contents;
        effect = _effect;
        gold = _gold;
    }

    public enum StoreItemCategory
    {
        Rack,
        Vase,
        Box,
        Thread
    }
}

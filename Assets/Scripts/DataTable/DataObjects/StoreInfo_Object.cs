using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StoreInfo_Object
{
    public int id;
    public string name;
    public string category;
    public string effect;
    public int gold;
    
    public StoreInfo_Object()
    {
        id = gold = 0;
        name = category = effect = "";
    }

    public StoreInfo_Object(int _id, string _name, string _category, string _effect, int _gold)
    {
        id = _id;
        name = _name;
        category = _category;
        effect = _effect;
        gold = _gold;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StoreInfo_Object
{
    public string id;
    public string name;
    public StoreItemCategory category;
    public ItemTheme theme;
    public int level;
    public string contents;
    public string effect;
    public int gold;
    public bool isButtonActive;
    
    public StoreInfo_Object()
    {
        id = "SO_0000";
        level = gold = 0;
        name = contents = effect = "";
        isButtonActive = false;
    }

    public StoreInfo_Object(string _id, string _name, StoreItemCategory _category, ItemTheme _theme, int _level, string _contents, string _effect, int _gold, bool _isButtonActive)
    {
        id = _id;
        name = _name;
        category = _category;
        theme = _theme;
        level = _level;
        contents = _contents;
        effect = _effect;
        gold = _gold;
        isButtonActive= _isButtonActive;
    }
}

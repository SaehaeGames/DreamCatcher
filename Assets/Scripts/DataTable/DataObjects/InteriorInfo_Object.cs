using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InteriorInfo_Object
{
    public int id;   //인테리어 아이템 아이디
    public string name; //인테리어 아이템 이름
    public string category;   //인테리어 아이템 카테고리
    public string theme;   //인테리어 아이템 테마
    public string contents; //인테리어 아이템 설명
    public int gold;    //인테리어 아이템 가격

    public InteriorInfo_Object()
    {
        id = gold = 0;
        name = category = theme = contents = "";
    }

    public InteriorInfo_Object(int _id, string _name, string _category, string _theme, string _contents,  int _gold)
    {
        id = _id;
        name = _name;
        category = _category;
        theme = _theme;
        contents = _contents;
        gold = _gold;
    }

    public enum InteriorItemCategory
    {
        Wallpaper,
        GarLane,
        WindowFrame,
        Pad,
        Quill,
        Prop1,
        CrystallBall,
        Telescope,
        Prop2
    }

    public enum InteriorItemTheme
    {
        Default,
        Sea,
        Star
    }
}

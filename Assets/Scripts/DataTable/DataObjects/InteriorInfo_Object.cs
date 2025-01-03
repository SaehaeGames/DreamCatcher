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

    public InteriorInfo_Object()
    {
        id = 0;
        name = category = "";
    }

    public InteriorInfo_Object(int _id, string _name, string _category)
    {
        id = _id;
        name = _name;
        category = _category;
    }
}

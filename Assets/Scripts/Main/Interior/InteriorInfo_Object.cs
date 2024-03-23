using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InteriorInfo_Object
{
    public int id;   //인테리어 아이템 아이디
    public string name; //인테리어 아이템 이름
    public string category_1;   //인테리어 아이템 카테고리 1 (아이템 종류 분류)
    public string category_2;   //인테리어 아이템 카테고리 2 (아이템 테마 분류)
    public int gold;    //인테리어 아이템 가격

    public InteriorInfo_Object()
    {
        id = gold = 0;
        name = category_1 = category_2 = "";
    }

    public InteriorInfo_Object(int _id, string _name, string _category_1, string _category_2, int _gold)
    {
        id = _id;
        name = _name;
        category_1 = _category_1;
        category_2 = _category_2;
        gold = _gold;
    }
}

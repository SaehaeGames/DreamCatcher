using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FeatherData
{
    public string id;
    public int count;
    public int appear;

    public FeatherData(string _id, int _count, int _appear)
    {
        id = _id;
        count = _count;
        appear = _appear;
    }

    # region Set 함수
    public void SetCount(int _count)
    {
        count = _count;
    }

    public void SetAppear(int _appear)
    {
        appear = _appear;
    }
    # endregion

    # region Get 함수
    public int GetCount()
    {
        return count;
    }

    public int GetAppear()
    {
        return appear;
    }

    public string GetId()
    {
        return id;
    }
    # endregion
}

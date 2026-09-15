using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiaryInfo_Object
{
    public string id;
    public string contents;

    public DiaryInfo_Object()
    {
        this.id = "SO_0000";
        this.contents = "";
    }

    public DiaryInfo_Object(string _id, string _contents)
    {
        this.id = _id;
        this.contents = _contents;
    }

    public string GetId()
    {
        return id;
    }

    public string GetContents()
    {
        return contents;
    }

    public void SetId(string _id)
    {
        id = _id;
    }

    public void SetContents(string _contents)
    {
        contents = _contents;
    }
}

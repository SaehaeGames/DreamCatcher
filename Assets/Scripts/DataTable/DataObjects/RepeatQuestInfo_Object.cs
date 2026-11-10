using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RepeatQuestInfo_Object
{
    public string id;
    public string title;
    public string contents;
    public string from;

    public RepeatQuestInfo_Object()
    {
        id = "SO_0000";
        title = contents = from = "";
    }

    public RepeatQuestInfo_Object(string _id, string _title, string _contents, string _from)
    {
        id = _id;
        title = _title;
        contents = _contents;
        from = _from;
    }

    public string GetId()
    {
        return id;
    }

    public string GetTitle()
    {
        return title;
    }

    public string GetContents()
    {
        return contents;
    }

    public string GetFrom()
    {
        return from;
    }
}

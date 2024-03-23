using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestInfo_Object
{
    public int id;
    public string title;
    public string contents;
    public string from;

    public QuestInfo_Object()
    {
        id = 0;
        title = contents = from = "";
    }

    public QuestInfo_Object(int _id, string _title, string _contents, string _from)
    {
        id = _id;
        title = _title;
        contents = _contents;
        from = _from;
    }
}

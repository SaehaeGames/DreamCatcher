using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestInfo_Object
{
    public string id;
    public string title;
    public string contents;
    public string from;
    public string questDreamCatcher;
    public int questDreamCatcherNum;

    public QuestInfo_Object()
    {
        id = "SO_0000";
        questDreamCatcher = "";
        questDreamCatcherNum = 0;
        title = contents = from = "";
    }

    public QuestInfo_Object(string _id, string _title, string _contents, string _from, string _questDreamCatcher, int _questDreamCatcherNum)
    {
        id = _id;
        title = _title;
        contents = _contents;
        from = _from;
        questDreamCatcher = _questDreamCatcher;
        questDreamCatcherNum = _questDreamCatcherNum;
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

    public string GetQuestDreamCatcher()
    {
        return questDreamCatcher;
    }

    public int GetQuestDreamCatcherNum()
    {
        return questDreamCatcherNum;
    }
}

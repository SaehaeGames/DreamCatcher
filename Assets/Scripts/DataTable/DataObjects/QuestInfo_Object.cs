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
    public int questDreamCatcher;
    public int questDreamCatcherNum;

    public QuestInfo_Object()
    {
        id = "SO_0000";
        questDreamCatcher = questDreamCatcherNum = 0;
        title = contents = from = "";
    }

    public QuestInfo_Object(string _id, string _title, string _contents, string _from, int _questDreamCatcher, int _questDreamCatcherNum)
    {
        id = _id;
        title = _title;
        contents = _contents;
        from = _from;
        questDreamCatcher = _questDreamCatcher;
        questDreamCatcherNum = _questDreamCatcherNum;
    }
}

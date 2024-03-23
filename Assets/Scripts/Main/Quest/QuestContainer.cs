using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class QuestData
{
    public QuestData(int _questID, int _questCheck, bool _questClear, DreamCatcher _questDreamCatcher)
    {
        questID = _questID;
        questCheck = _questCheck;
        questClear = _questClear;
        questDreamCatcher = _questDreamCatcher;
    }

    public int questID;     //퀘스트 id
    public int questCheck;   //퀘스트 확인 여부
    public bool questClear;  //퀘스트 클리어 여부
    public DreamCatcher questDreamCatcher;  //퀘스트 드림캐쳐
}

public class QuestContainer
{
    public QuestContainer()
    {
        ResetQuestData();
    }

    public void ResetQuestData()
    {
        questList = new List<QuestData>();

        DreamCatcher temp1 = new DreamCatcher(0, null, null, 1, 2, 3, 4);
        DreamCatcher temp2 = new DreamCatcher(0, null, null, 11, 22, 33, 44);
        DreamCatcher temp3 = new DreamCatcher(0, null, null, 111, 222, 333, 444);

        questList.Add(new QuestData(0000, 0, true, null));
        questList.Add(new QuestData(6001, 0, false, temp1));
        questList.Add(new QuestData(6002, 0, false, temp2));
        questList.Add(new QuestData(6003, 0, false, temp3));
    }
    public List<QuestData> questList;   //퀘스트 리스트
}

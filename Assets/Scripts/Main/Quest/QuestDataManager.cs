using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class QuestData
{
    public int id;     //퀘스트 id
    public bool isChecked;   //퀘스트 확인 여부
    public bool isClear;  //퀘스트 클리어 여부
    public DreamCatcher questDreamCatcher;  //퀘스트 드림캐쳐

    public QuestData()
    {
        id = 0;
        isChecked = isClear = false;
        questDreamCatcher = null;
    }

    public QuestData(int id, bool isChecked, bool isClear, DreamCatcher questDreamCatcher)
    {
        this.id = id;
        this.isChecked = isChecked;
        this.isClear = isClear;
        this.questDreamCatcher = questDreamCatcher;
    }
}

public class QuestDataManager
{
    public List<QuestData> dataList;

    public QuestDataManager()
    {
        dataList = new List<QuestData>();
        ResetData();
    }

    public void ResetData()
    {
        if (dataList != null)
            dataList.Clear();

        List<QuestInfo_Object> infoDataList = GameManager.instance.questinfo_data.dataList;
        for (int i = 0; i < infoDataList.Count; i++)
            dataList.Add(new QuestData(infoDataList[i].id, false, false, null)); // ** 여기 null 부분에 퀘스트 드림캐쳐 정보 넣기
    }

    public QuestData GetQuestData(int id)
    {
        QuestData getData = dataList.FirstOrDefault(x => x.id == id);
        if (getData != null)
            return getData;
        else
            return null;
    }
}



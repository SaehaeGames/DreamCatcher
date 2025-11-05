using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class QuestData
{
    public string id;     //퀘스트 id
    public bool isChecked;   //퀘스트 확인 여부
    public bool isClear;  //퀘스트 클리어 여부
    public string questInfo_Id;

    public QuestData()
    {
        id = "JS_0000";
        isChecked = isClear = false;
        questInfo_Id = "SO_0000";
    }

    public QuestData(string _id, bool _isChecked, bool _isClear, string _questInfoId)
    {
        this.id = _id;
        this.isChecked = _isChecked;
        this.isClear = _isClear;
        this.questInfo_Id = _questInfoId;
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
        int startId = 6000;
        List<QuestInfo_Object> infoDataList = GameManager.instance.questinfo_data.dataList;
        for (int i = 0; i < infoDataList.Count; i++)
        {
            dataList.Add(new QuestData("JS_" + startId, false, false, infoDataList[i].id));
            startId++;
        }

        foreach (var q in dataList)
        {
            Debug.Log(q.id);
        }
    }

    public QuestData GetQuestData(string _id)
    {
        QuestData getData = dataList.FirstOrDefault(x => x.id == _id);
        if (getData != null)
            return getData;
        else
            return null;
    }
}



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
    public int questInfoId;

    public QuestData()
    {
        id = 0;
        isChecked = isClear = false;
        questInfoId = 0;
    }

    public QuestData(int _id, bool _isChecked, bool _isClear, int _questInfoId)
    {
        this.id = _id;
        this.isChecked = _isChecked;
        this.isClear = _isClear;
        this.questInfoId = _questInfoId;
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
        int questDataManagerId = 6000;
        List<QuestInfo_Object> infoDataList = GameManager.instance.questinfo_data.dataList;
        for (int i = 0; i < infoDataList.Count; i++)
        {
            dataList.Add(new QuestData(questDataManagerId, false, false, infoDataList[i].id)); // ** 여기 null 부분에 퀘스트 드림캐쳐 정보 넣기
            questDataManagerId++;
        }
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



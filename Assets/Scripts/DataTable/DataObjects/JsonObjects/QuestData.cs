using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region Set 함수

    public void SetIsChecked(bool _isChecked)
    {
        this.isChecked = _isChecked;
    }

    public void SetIsClear(bool _isClear)
    {
        this.isClear = _isClear;
    }

    public void SetQuestInfo_Id(string _questInfo_Id)
    {
        questInfo_Id = _questInfo_Id;
    }

    #endregion

    #region Get 함수
    public bool GetIsChecked()
    {
        return isChecked;
    }

    public bool GetIsClear()
    {
        return isClear;
    }

    public string GetQuestInfo_Id()
    {
        return questInfo_Id;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class QuestDataManager
{
    private JsonManager jsonManager = new JsonManager();
    private QuestDataModel questDataModel = new QuestDataModel();
    public IReadOnlyList<QuestData> questDataList => questDataModel.dataList;
    private const int IdBase = 6000;

    public void ClearQuest(int questIndex)
    {
        QuestData questData = GetQuestData(questIndex);

        if (questData == null)
            return;

        questData.isClear = true;
        Save();
    }

    public void CheckQuest(int questIndex)
    {
        QuestData questData = GetQuestData(questIndex);

        if (questData == null)
            return;

        questData.isChecked = true;
        Save();
    }

    public void ResetData()
    {
        if (questDataModel.dataList != null)
            questDataModel.dataList.Clear();
        
        List<QuestInfo_Object> infoDataList = GameManager.instance.questinfo_data.dataList;
        for (int i = 0; i < infoDataList.Count; i++)
        {
            questDataModel.dataList.Add(new QuestData("JS_" + (IdBase + i), false, false, infoDataList[i].id));
        }

        Save();
    }

    public QuestData GetQuestData(string questDataId)
    {
        QuestData getData = questDataModel.dataList.FirstOrDefault(x => x.id == questDataId);
        if (getData != null)
            return getData;
        else
            return null;
    }

    public QuestData GetQuestData(int questIndex)
    {
        if (questIndex < 0 || questIndex >= questDataModel.dataList.Count)
        {
            Debug.LogError($"Quest Index Out Of Range : {questIndex}");
            return null;
        }

        QuestData getData = questDataModel.dataList[questIndex];
        if (getData != null)
            return getData;
        else
            return null;
    }

    public QuestInfo_Object GetQuestInfo(int questIndex)
    {
        QuestData data = GetQuestData(questIndex);

        if (data == null)
            return null;

        return GameManager.instance.questinfo_data.GetQuestInfo(data.questInfo_Id);
    }

    public bool IsQuestChecked(string questDataId)
    {
        QuestData questData = GetQuestData(questDataId);

        if (questData == null) return false;

        return questData.isChecked;
    }

    public bool IsQuestChecked(int questIndex)
    {
        QuestData questData = GetQuestData(questIndex);
        
        if(questData==null) return false;

        return questData.isChecked;
    }
    public bool IsQuestCleared(string questDataId)
    {
        QuestData questData = GetQuestData(questDataId);

        if (questData == null) return false;

        return questData.isClear;
    }

    public bool IsQuestCleared(int questIndex)
    {
        QuestData questData = GetQuestData(questIndex);

        if (questData == null) return false;

        return questData.isClear;
    }
    public void Load()
    {
        questDataModel = jsonManager.LoadData<QuestDataModel>(Constants.QuestDataFile);

        if (questDataModel == null)
            questDataModel = new QuestDataModel();
    }

    public void Save()
    {
        jsonManager.SaveData(Constants.QuestDataFile, questDataModel);
    }
}
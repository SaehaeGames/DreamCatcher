using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatQuestDataManager
{
    private JsonManager jsonManager = new JsonManager();
    private RepeatQuestDataModel repeatQuestDataModel = new RepeatQuestDataModel();

    public void SetRepeatQuestDreamCatcherList(RepeatQuestDreamCatcher repeatQuestDreamCatcher1, RepeatQuestDreamCatcher repeatQuestDreamCatcher2, RepeatQuestDreamCatcher repeatQuestDreamCatcher3)
    {
        repeatQuestDataModel.repeatQuestDreamCatcherList[0] = repeatQuestDreamCatcher1;
        repeatQuestDataModel.repeatQuestDreamCatcherList[1] = repeatQuestDreamCatcher2;
        repeatQuestDataModel.repeatQuestDreamCatcherList[2] = repeatQuestDreamCatcher3;
        Save();
    }

    public void SetRepeatQuestDreamCatcherList(List<RepeatQuestDreamCatcher> dreamCatcherList)
    {
        repeatQuestDataModel.repeatQuestDreamCatcherList = dreamCatcherList;

        for (int i = 0; i < repeatQuestDataModel.repeatQuestDreamCatcherList.Count; i++)
        {
            var quest = repeatQuestDataModel.repeatQuestDreamCatcherList[i];
        }

        Save();
    }

    public void ReadFirstLetter()
    {
        repeatQuestDataModel.hasReadFirstLetter = true;
        Save();
    }

    public void UpdateQuestInProgress(bool inProgress)
    {
        repeatQuestDataModel.questInProgress = inProgress;
        Save();
    }

    public bool GetQuestInProgress()
    {
        return repeatQuestDataModel.questInProgress;
    }

    public bool GetHasReadFirstLetter()
    {
        return repeatQuestDataModel.hasReadFirstLetter;
    }

    public RepeatQuestDreamCatcher GetRepeatQuestDreamCatcherList(int index)
    {
        var quest = repeatQuestDataModel.repeatQuestDreamCatcherList[index];

        return quest;
    }

    public void Load()
    {
        repeatQuestDataModel = jsonManager.LoadData<RepeatQuestDataModel>(Constants.RepeatQuestDataFile);

        if (repeatQuestDataModel == null)
            repeatQuestDataModel = new RepeatQuestDataModel();

        if (repeatQuestDataModel.repeatQuestDreamCatcherList == null)
            repeatQuestDataModel.repeatQuestDreamCatcherList = new List<RepeatQuestDreamCatcher>();
    }

    public void Save()
    {
        jsonManager.SaveData(Constants.RepeatQuestDataFile, repeatQuestDataModel);
    }
}

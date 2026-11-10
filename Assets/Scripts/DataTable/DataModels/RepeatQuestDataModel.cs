using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RepeatQuestDataModel
{
    public bool hasReadFirstLetter;
    public bool questInProgress;
    public List<RepeatQuestDreamCatcher> repeatQuestDreamCatcherList;

    public RepeatQuestDataModel()
    {
        hasReadFirstLetter = false;
        questInProgress = false;
        repeatQuestDreamCatcherList = new List<RepeatQuestDreamCatcher>(3);
    }

    public RepeatQuestDataModel(bool _hasReadFirstLetter, bool _questInProgress, List<RepeatQuestDreamCatcher> _repeatQuestDreamCatcherList)
    {
        hasReadFirstLetter = _hasReadFirstLetter;
        questInProgress = _questInProgress;
        repeatQuestDreamCatcherList = _repeatQuestDreamCatcherList;
    }
}

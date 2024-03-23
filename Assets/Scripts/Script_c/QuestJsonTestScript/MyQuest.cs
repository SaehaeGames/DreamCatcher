using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyQuest : MonoBehaviour
{
    public int QuestCnt;
    public List<Quest> questList;

    public MyQuest()
    {
        QuestCnt = 0;
        questList = new List<Quest>();
        //ResetDreamCatcherData();
    }

    public Quest GetQuestData(int index)
    {
        Debug.Log("arrange test: " + questList[index]);
        return questList[index];
    }


    public void ResetQuestData()
    {

    }
}

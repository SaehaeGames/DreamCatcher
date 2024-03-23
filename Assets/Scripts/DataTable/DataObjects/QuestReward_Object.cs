using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestReward_Object
{
    public int id;
    public string name;
    public string rewardType;    

    public QuestReward_Object()
    {
        id = 9000;
        name = rewardType = "";
    }

    public QuestReward_Object(int _id, string _name, string _rewardType)
    {
        id = _id;
        name = _name;
        rewardType = _rewardType;
    }
}

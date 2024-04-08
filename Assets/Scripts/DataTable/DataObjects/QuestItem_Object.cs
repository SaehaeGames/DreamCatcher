using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestItem_Object
{
    public int id;
    public string name;
    public RewardType rewardType;    

    public QuestItem_Object()
    {
        id = 0;
        name = "";
    }

    public QuestItem_Object(int _id, string _name, RewardType _rewardType)
    {
        id = _id;
        name = _name;
        rewardType = _rewardType;
    }

    public enum RewardType
    {
        Item,
        Unlock
    }
}

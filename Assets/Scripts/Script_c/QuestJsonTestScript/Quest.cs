using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest : MonoBehaviour
{
    public int quest_num;
    public string quest_content;
    public int quest_gift;
    public int quest_dcnum;
    public DreamCatcher quest_dcshape;
    public bool quest_accept;
    public bool quest_complete;

    public Quest(int _quest_num, string _quest_content, int _quest_gift, int _quest_dcnum, DreamCatcher _quest_dcshape, bool _quest_accept, bool _quest_complete)
    {
        quest_num = _quest_num;
        quest_content = _quest_content;
        quest_gift = _quest_gift;
        quest_dcnum = _quest_num;
        quest_dcshape = _quest_dcshape;
        quest_accept = _quest_accept;
        quest_complete = _quest_complete;
    }

    public string GetQuestContent()
    {
        return quest_content;
    }

    public DreamCatcher GetQuestDCshape()
    {
        return quest_dcshape;
    }
}

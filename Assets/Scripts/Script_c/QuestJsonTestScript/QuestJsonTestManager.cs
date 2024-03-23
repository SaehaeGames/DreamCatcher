using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestJsonTestManager : MonoBehaviour
{
    public int[] DCline = new int[64];
    public bool[] DCbead = new bool[48];

    QuestDataManager questDataManager;
    MyQuest myQuestList;
    public Text textUi;

    private void OnEnable()
    {
        questDataManager = this.GetComponent<QuestDataManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        myQuestList = questDataManager.GetQuestData();
        Debug.Log(myQuestList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadQuest()
    {
        myQuestList = questDataManager.GetQuestData();
        textUi.text = myQuestList.questList[0].GetQuestContent();
        textUi.text += '\n' + myQuestList.questList[0].GetQuestDCshape().GetColor();
    }

    public void SaveQuest(int num)
    {
        DreamCatcher myDreamCatcher = new DreamCatcher(1001, DCline, DCbead, 0, 1, 2, 3);
        myQuestList.questList.Add(new Quest(2001, "안녕", 1, 1, myDreamCatcher, true, true));
        Debug.Log(myQuestList.questList[0].quest_content);
        questDataManager.DataSaveText(myQuestList);
        Debug.Log(myQuestList.questList);
    }
}

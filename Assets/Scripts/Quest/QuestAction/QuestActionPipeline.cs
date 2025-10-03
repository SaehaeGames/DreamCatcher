using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestActionPipeline : MonoBehaviour
{
    [SerializeField]
    private List<InteractiveSequenceBase> questActions;

    private InteractiveSequenceBase currentQuestAction = null;
    private int currentIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        // 퀘스트 액션 리스트 초기화
        questActions.Clear();
        for(int i=0; i<this.transform.childCount; i++)
        {
            questActions.Add(this.transform.GetChild(i).gameObject.GetComponent<InteractiveSequenceBase>());
        }

        SetNextQuestAction();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentQuestAction != null)
        {
            currentQuestAction.Execute(this);
        }
    }

    public void SetNextQuestAction()
    {
        if(currentQuestAction!=null)
        {
            currentQuestAction.Exit();
        }

        if(currentIndex>=questActions.Count-1)
        {
            currentQuestAction = null;
            this.gameObject.SetActive(false);
            return;
        }

        currentIndex++;
        currentQuestAction = questActions[currentIndex];

        currentQuestAction.Enter();
    }
}

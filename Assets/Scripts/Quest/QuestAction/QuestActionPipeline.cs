using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestActionPipeline : MonoBehaviour
{
    [SerializeField]
    private List<InteractiveSequenceBase> questActions;

    private InteractiveSequenceBase currentQuestAction = null;
    private int currentIndex = -1;

    private PlayerDataManager playerDataManager;
    private QuestDataManager questDataManager;

    // Start is called before the first frame update
    void Awake()
    {
        playerDataManager = GameManager.instance.playerDataManager;
        questDataManager = GameManager.instance.questDataManager;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentQuestAction != null)
        {
            currentQuestAction.Execute(this);
        }
    }

    public void StartQuestAction()
    {
        if (playerDataManager == null)
            playerDataManager = GameManager.instance.playerDataManager;

        if (questDataManager == null)
            questDataManager = GameManager.instance.questDataManager;

        // 퀘스트 액션 리스트 초기화
        questActions.Clear();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            questActions.Add(this.transform.GetChild(i).gameObject.GetComponent<InteractiveSequenceBase>());
        }

        playerDataManager.SetIsQuestActionPlaying(true);
        playerDataManager.Save();

        SetNextQuestAction();
    }

    public void EndQuestAction()
    {
        currentQuestAction = null;
        playerDataManager.SetIsQuestActionPlaying(false);
        playerDataManager.Save();
        questDataManager.Save();
        this.gameObject.SetActive(false);
    }

    public void SetNextQuestAction()
    {
        Debug.Log("SetNextQuestAction"+currentIndex);
        if (currentQuestAction!=null)
        {
            currentQuestAction.Exit();
        }

        if(IsLastQuestAction())
        {
            Debug.Log("LastQuestAction");
            EndQuestAction();
            return;
        }

        currentIndex++;
        currentQuestAction = questActions[currentIndex];

        Debug.Log($"currentQuestAction = {currentQuestAction}");
        Debug.Log("Enter 호출 직전");

        currentQuestAction.Enter();

        Debug.Log("Enter 호출 직후");
    }

    public bool IsLastQuestAction()
    {
       return currentIndex == questActions.Count - 1;
    }
}

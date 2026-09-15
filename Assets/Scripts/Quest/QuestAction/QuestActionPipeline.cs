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

    private System.Action onStart;
    private System.Action onComplete;

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

    public void StartQuestAction(System.Action onStart = null, System.Action onComplete = null)
    {
        // 콜백 함수
        this.onStart = onStart;
        this.onComplete = onComplete;

        onStart?.Invoke();

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

        SetNextQuestAction();
    }

    public void EndQuestAction()
    {
        currentQuestAction = null;

        onComplete?.Invoke();

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

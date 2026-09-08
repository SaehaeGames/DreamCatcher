using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestActionType
{
    None,
    Accept,
    Complete
}
public class QuestActionController : MonoBehaviour
{
    public GameObject questActionPipelines;
    private PlayerDataManager playerDataManager;
    private QuestDataManager questDataManager;
    private int completedQuestNum;
    public Transform[] quests;

    private void Awake()
    {
        // 퀘스트 액션 갯수 파악
        int questCount = questActionPipelines.transform.childCount;
        quests = new Transform[questCount];

        // 모든 퀘스트 액션 비활성화
        for(int i=0; i< questCount; i++)
        {
            quests[i] = questActionPipelines.transform.GetChild(i); // 퀘스트 액션 저장
            quests[i].GetChild(1).gameObject.SetActive(false); // End 비활성화
            quests[i].GetChild(0).gameObject.SetActive(false); // Start 비활성화
        }
    }

    void Start()
    {
        // 플레이어 데이터(PlayerDataFile) 로드
        playerDataManager = GameManager.instance.playerDataManager;
        questDataManager = GameManager.instance.questDataManager;
        if (playerDataManager == null)
        {
            Debug.LogError("_playerDataManager instance is null");
        }
        if (questDataManager == null)
        {
            Debug.LogError("_questDataManager instance is null");
        }

        // 현재 퀘스트 번호 불러오기
        completedQuestNum = playerDataManager.GetCurrentMainQuestIndex(); // 현재의 퀘스트 번호 불러오기

        if (this == null)
        { Debug.LogError("QuestActionManager instance is null"); }

        if (playerDataManager == null)
        { Debug.LogError("_playerDataContainer is null"); }

    }

    public void PlayQuestAction(
        QuestActionType type,
        int questIndex,
        System.Action onStart = null,
        System.Action onComplete = null)
    {
        if (type == QuestActionType.Accept)
        {
            ActivateAcceptQuestAction(questIndex, onStart,onComplete);
        }
        else if (type == QuestActionType.Complete)
        {
            ActivateCompleteQuestAction(questIndex, onStart, onComplete);
        }
    }

    public void ActivateAcceptQuestAction(
        int questIndex,
        System.Action onStart = null,
        System.Action onComplete = null)
    {
        Debug.Log("<color=cyan>Handle Set Quest Start Active ----- Start</color>");
        if (completedQuestNum >= 2 && completedQuestNum < quests.Length)
        {
            Transform startObject = quests[completedQuestNum].GetChild(0);
            
            if (startObject != null)
            {
                Debug.Log("<color=cyan>Start QuestAction 활성화</color>");
                startObject.gameObject.SetActive(true); // Start 활성화
                startObject.gameObject.GetComponent<QuestActionPipeline>().StartQuestAction(onStart, onComplete);
            }
        }
        Debug.Log("<color=cyan>Handle Set Quest Start Active ----- End</color>");
    }

    public void ActivateCompleteQuestAction(
        int questIndex,
        System.Action onStart = null,
        System.Action onComplete = null)
    {
        Debug.Log("<color=cyan>Handle Set Quest End Active ----- Start</color>");
        if (completedQuestNum >= 2 && completedQuestNum < quests.Length)
        {
            Transform endObject = quests[completedQuestNum].GetChild(1);
            if (endObject != null)
            {
                Debug.Log("<color=cyan>End QuestAction 활성화</color>");
                endObject.gameObject.SetActive(true); // End 활성화
                endObject.gameObject.GetComponent<QuestActionPipeline>().StartQuestAction(onStart, onComplete);
            }
        }
        Debug.Log("<color=cyan>Handle Set Quest End Active ----- End</color>");
    }
}

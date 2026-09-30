using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public enum QuestActionType
{
    None,
    Accept,
    Complete,
    Charon
}
public class QuestActionController : MonoBehaviour
{
    public GameObject mainQuestActions;
    public GameObject charonQuestActions;
    private PlayerDataManager playerDataManager;
    private QuestDataManager questDataManager;
    //private int completedQuestNum;
    private Transform[] mainQuests;
    private Transform[] charonLetters;

    private void Awake()
    {
        // 퀘스트 액션 갯수 파악
        int questCount = mainQuestActions.transform.childCount;
        mainQuests = new Transform[questCount];

        // 모든 퀘스트 액션 비활성화
        for(int i=0; i< questCount; i++)
        {
            mainQuests[i] = mainQuestActions.transform.GetChild(i); // 퀘스트 액션 저장
            mainQuests[i].GetChild(1).gameObject.SetActive(false); // End 비활성화
            mainQuests[i].GetChild(0).gameObject.SetActive(false); // Start 비활성화
        }

        int charonQuestCount = charonQuestActions.transform.childCount;
        charonLetters = new Transform[charonQuestCount];

        for (int i = 0; i < charonQuestCount; i++)
        {
            charonLetters[i] = charonQuestActions.transform.GetChild(i);
            charonLetters[i].gameObject.SetActive(false);
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
        else if(type==QuestActionType.Charon)
        {
            Debug.Log("카론 퀘스트 액션 재생");
            ActivateCharonLetterQuestAction(questIndex, onStart, onComplete);
        }
    }

    public void ActivateAcceptQuestAction(
        int questIndex,
        System.Action onStart = null,
        System.Action onComplete = null)
    {
        Debug.Log("<color=cyan>Handle Set Quest Start Active ----- Start</color>");
        if (questIndex >= 2 && questIndex < mainQuests.Length)
        {
            Transform startObject = mainQuests[questIndex].GetChild(0);
            
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
        if (questIndex >= 2 && questIndex < mainQuests.Length)
        {
            Transform endObject = mainQuests[questIndex].GetChild(1);
            if (endObject != null)
            {
                Debug.Log("<color=cyan>End QuestAction 활성화</color>");
                endObject.gameObject.SetActive(true); // End 활성화
                endObject.gameObject.GetComponent<QuestActionPipeline>().StartQuestAction(onStart, onComplete);
            }
        }
        Debug.Log("<color=cyan>Handle Set Quest End Active ----- End</color>");
    }

    public void ActivateCharonLetterQuestAction(
        int questIndex,
        System.Action onStart = null,
        System.Action onComplete = null)
    {
        if (questIndex >= 0 && questIndex < charonLetters.Length)
        {
            Transform letter = charonLetters[questIndex];
            if (letter != null)
            {
                Debug.Log("<color=cyan>Charon QuestAction 활성화</color>");
                letter.gameObject.SetActive(true); // End 활성화
                letter.gameObject.GetComponent<QuestActionPipeline>().StartQuestAction(onStart, onComplete);
            }
        }
    }
}

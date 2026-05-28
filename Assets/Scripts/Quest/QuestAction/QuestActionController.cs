using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void HandleSetQuestStartActive()
    {
        Debug.Log("<color=cyan>Handle Set Quest Start Active ----- Start</color>");
        completedQuestNum = playerDataManager.GetCurrentMainQuestIndex(); // 현재의 퀘스트 번호 불러오기
        bool questAccepted = questDataManager.IsQuestChecked(completedQuestNum); // 현재 퀘스트 수락 상태인지 불러오기
        if (completedQuestNum >= 2 && completedQuestNum < quests.Length && !questAccepted)
        {
            Transform startObject = quests[completedQuestNum].GetChild(0);
            
            if (startObject != null)
            {
                Debug.Log("<color=cyan>Start QuestAction 활성화</color>");
                startObject.gameObject.SetActive(true); // Start 활성화

                // 퀘스트 수락 상태 업데이트
                questDataManager.CheckStartQuest(completedQuestNum); // [퀘스트 중]으로 수정
            }
        }
        Debug.Log("<color=cyan>Handle Set Quest Start Active ----- End</color>");
    }

    public void ActiveQuestEndActionActive(int questIndex)
    {
        Debug.Log("<color=cyan>Handle Set Quest End Active ----- Start</color>");
        bool isQuestCleared = questDataManager.IsQuestCleared(questIndex); // 현재 퀘스트 수락 상태인지 불러오기
        if (completedQuestNum >= 2 && completedQuestNum < quests.Length && isQuestCleared)
        {
            Transform endObject = quests[completedQuestNum].GetChild(1);
            if (endObject != null)
            {
                Debug.Log("<color=cyan>End QuestAction 활성화</color>");
                endObject.gameObject.SetActive(true); // End 활성화 
            }
        }
        Debug.Log("<color=cyan>Handle Set Quest End Active ----- End</color>");
    }
}

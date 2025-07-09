using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestActionManager : MonoBehaviour
{
    private PlayerDataManager _playerDataContainer;
    private int currentQuestNum;
    public Transform[] quests;

    public event Action OnSetQuestStartActive;
    public event Action OnSetQuestEndActive;


    private void Awake()
    {
        // 퀘스트 액션 갯수 파악
        int questCount = this.transform.childCount;
        quests = new Transform[questCount];

        // 모든 퀘스트 액션 비활성화
        for(int i=0; i<this.transform.childCount; i++)
        {
            quests[i] = this.transform.GetChild(i); // 퀘스트 액션 저장
            quests[i].GetChild(1).gameObject.SetActive(false); // End 비활성화
            quests[i].GetChild(0).gameObject.SetActive(false); // Start 비활성화
        }
    }

    void Start()
    {
        // 플레이어 데이터(PlayerDataFile) 로드
        _playerDataContainer = GameManager.instance.playerDataManager;
        if(_playerDataContainer == null)
        {
            Debug.LogError("_playerDataContainer instance is null");
        }
        else
        {
            Debug.Log("_playerDataContainer instance is not null");
        }

        // 현재 퀘스트 번호 불러오기
        currentQuestNum = (int)_playerDataContainer.dataList[8].dataNumber; // 현재의 퀘스트 번호 불러오기
        Debug.LogError("currentQuestNum is here");

        if (this == null)
        { Debug.LogError("QuestActionManager instance is null"); }

        if (_playerDataContainer == null)
        { Debug.LogError("_playerDataContainer is null"); }

    }

    private void OnEnable()
    {
        OnSetQuestStartActive += HandleSetQuestStartActive;
        OnSetQuestEndActive += HandleSetQuestEndActive;
    }

    private void OnDisable()
    {
        OnSetQuestStartActive -= HandleSetQuestStartActive;
        OnSetQuestEndActive -= HandleSetQuestEndActive;
    }

    public void HandleSetQuestStartActive()
    {
        Debug.Log("<color=cyan>Handle Set Quest Start Active ----- Start</color>");
        currentQuestNum = (int)_playerDataContainer.dataList[8].dataNumber; // 현재의 퀘스트 번호 불러오기
        int questAccepted = (int)_playerDataContainer.dataList[9].dataNumber; // 현재 퀘스트 수락 상태인지 불러오기
        if (currentQuestNum >= 3 && currentQuestNum < quests.Length + 3 && questAccepted == 0)
        {
            Transform startObject = quests[currentQuestNum - 3].GetChild(0);
            
            if (startObject != null)
            {
                Debug.Log("<color=cyan>Start QuestAction 활성화</color>");
                startObject.gameObject.SetActive(true); // Start 활성화

                // 퀘스트 수락 상태 업데이트
                _playerDataContainer.dataList[9].dataNumber = 1; // [퀘스트 중]으로 수정
                GameManager.instance.jsonManager.SaveData(Constants.PlayerDataFile, _playerDataContainer);
            }
        }
        Debug.Log("<color=cyan>Handle Set Quest Start Active ----- End</color>");
    }

    public void HandleSetQuestEndActive()
    {
        currentQuestNum = (int)_playerDataContainer.dataList[8].dataNumber; // 현재의 퀘스트 번호 불러오기
        int questAccepted = (int)_playerDataContainer.dataList[9].dataNumber; // 현재 퀘스트 수락 상태인지 불러오기
        if (currentQuestNum >= 3 && currentQuestNum < quests.Length + 3 && questAccepted == 1)
        {
            Transform endObject = quests[currentQuestNum - 3].GetChild(1);
            if (endObject != null)
            {
                Debug.Log("<color=cyan>Start QuestAction 비활성화</color>");
                endObject.gameObject.SetActive(true); // End 활성화 

                // 퀘스트 수락 상태 업데이트
                _playerDataContainer.dataList[9].dataNumber = 0; // [퀘스트 없음]으로 수정
                GameManager.instance.jsonManager.SaveData(Constants.PlayerDataFile, _playerDataContainer);
            }
        }
    }
}

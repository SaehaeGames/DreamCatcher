using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DeliveryResult
{
    Success,
    WrongItem,
    Error
}

public class DeliveryManager : MonoBehaviour
{
    // 데이터
    private DreamCatcherDataManager dreamCatcherDataManager;
    private QuestInfo_Data questInfo_data;
    private DreamCatcherInventoryData selectedDreamCatcherInventoryData;
    private PlayerDataManager playerDataManager;
    private QuestDataManager questDataManager;
    private RepeatQuestDataManager repeatQuestDataManager;

    private QuestType questType;
    private RepeatQuestDreamCatcher matchedRepeatQuestDreamCatcher;

    // Start is called before the first frame update
    void Start()
    {
        // 데이터 불러오기
        dreamCatcherDataManager = GameManager.instance.dreamCatcherDataManager;
        questInfo_data = GameManager.instance.questinfo_data;
        playerDataManager = GameManager.instance.playerDataManager;
        questDataManager = GameManager.instance.questDataManager;
        repeatQuestDataManager = GameManager.instance.repeatQuestDataManager;
    }

    public bool HasEnoughDreamCatchers(DreamCatcherInventoryData selectedDreamCatcher)
    {
        int requiredCount = questInfo_data.dataList[0].GetQuestDreamCatcherNum(); // 나중에 수정 현재 퀘스트로
        int currentCount = selectedDreamCatcher.GetNumber();

        if (requiredCount <= currentCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public DeliveryResult TryDeliveryDreamCatcher()
    {
        // 초기화
        matchedRepeatQuestDreamCatcher = null;

        // 안전 코드
        if (selectedDreamCatcherInventoryData == null)
        {
            Debug.LogError("[DeliveryManager] selectedDreamCatcherInventoryData is NULL");
            return DeliveryResult.Error;
        }

        questType = GetComponent<QuestManager>().GetCurrentQuestType();

        DeliveryResult result;

        switch (questType)
        {
            case QuestType.MainQuest:
                result = TryMainQuestDelivery();
                break;
                
            case QuestType.RepeatQuest:
                result = TryRepeatQuestDelivery();
                break;
            default:
                result = DeliveryResult.Error;
                break;
        }
        
        // 드림캐쳐 납품(삭제) => 틀릴 때는 삭제를 원치 않을 시 성공했을 때만 삭제하도록 위치 이동
        DeliveryDreamCatcher();

        return result;
    }

    private DeliveryResult TryMainQuestDelivery()
    {
        // 현재 퀘스트에서 요구하는 드림캐쳐 불러오기
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();
        string questDreamCatcher = questInfo_data.dataList[currentMainQuestIndex * 2].questDreamCatcher;

        // 선택한 드림캐쳐 불러오기
        string selectedDreamCatcher = selectedDreamCatcherInventoryData.GetTemplateHash();

        // 안전 코드
        if (questDreamCatcher == null || selectedDreamCatcher == null)
        {
            Debug.LogError(
                $"[DeliveryManager] TryDeliveryDreamCatcher - Null detected | " +
                $"questDreamCatcher: {(questDreamCatcher == null ? "NULL" : questDreamCatcher)}, " +
                $"selectedDreamCatcher: {(selectedDreamCatcher == null ? "NULL" : selectedDreamCatcher)}"
            );
            return DeliveryResult.Error;
        }

        // 드림캐쳐 비교 및 처리
        if (questDreamCatcher != selectedDreamCatcher)
        {
            return DeliveryResult.WrongItem;
        }
        else
        {
            return DeliveryResult.Success;
        }
    }

    private DeliveryResult TryRepeatQuestDelivery()
    {
        // 선택한 드림캐쳐 불러오기
        IReadOnlyList<string> dcIds = selectedDreamCatcherInventoryData.GetDCids();
        DreamCatcher selectedDreamCatcher = dreamCatcherDataManager.GetDreamCatcherById(dcIds[0]);

        // 선택한 드림캐쳐의 깃털 목록
        HashSet<int> selectedFeathers = new HashSet<int>
        {
            selectedDreamCatcher.GetFeather(0),
            selectedDreamCatcher.GetFeather(1),
            selectedDreamCatcher.GetFeather(2)
        };

        for (int i = 0; i < 3; i++)
        {
            RepeatQuestDreamCatcher questDreamCatcher =
                repeatQuestDataManager.GetRepeatQuestDreamCatcherList(i);

            // 실 색상 비교
            if (questDreamCatcher.DCcolor != selectedDreamCatcher.GetColor())
                continue;

            // 퀘스트에서 요구하는 깃털 목록
            HashSet<int> questFeathers = new HashSet<int>
            {
                questDreamCatcher.DCfeather1,
                questDreamCatcher.DCfeather2,
                questDreamCatcher.DCfeather3
            };

            // 깃털 3개가 모두 일치하는지 확인
            if (questFeathers.SetEquals(selectedFeathers))
            {
                matchedRepeatQuestDreamCatcher = questDreamCatcher;
                return DeliveryResult.Success;
            }
        }

        return DeliveryResult.WrongItem;
    }

    public void DeliveryDreamCatcher()
    {
        if (selectedDreamCatcherInventoryData == null)
        {
            Debug.LogError("[DeliveryManager] selectedDreamCatcherInventoryData is NULL");
            return;
        }

        // 해당 드림캐쳐 삭제
        IReadOnlyList<string> dcIds = selectedDreamCatcherInventoryData.GetDCids();
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();
        int requiredCount = questInfo_data.dataList[currentMainQuestIndex].GetQuestDreamCatcherNum();
        if (HasEnoughDreamCatchers(selectedDreamCatcherInventoryData))
        {
            for (int i = 0; i < requiredCount; i++)
            {
                dreamCatcherDataManager.RemoveDreamCatcher(dcIds[i]);
            }
        }
    }

    public void CompleteCurrentQuest()
    {
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();

        // 퀘스트 완료 처리
        questDataManager.CheckEndQuest(currentMainQuestIndex);
        questDataManager.Save();

        // 꿈 구슬 지급
        playerDataManager.AddDreamMarble(1);
        playerDataManager.Save();
    }

    public void DeliveryFailed()
    {
        
    }

    public void SetSelectedDreamCatcherInventoryData(DreamCatcherInventoryData _selectedDreamCatcherInventoryData)
    {
        selectedDreamCatcherInventoryData = _selectedDreamCatcherInventoryData;
    }

    public RepeatQuestDreamCatcher GetMatchedRepeatQuestDreamCatcher()
    {
        return matchedRepeatQuestDreamCatcher;
    }
}

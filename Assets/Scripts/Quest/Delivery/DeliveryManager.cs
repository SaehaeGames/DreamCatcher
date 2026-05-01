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

    // Start is called before the first frame update
    void Start()
    {
        // 데이터 불러오기
        dreamCatcherDataManager = GameManager.instance.dreamCatcherDataManager;
        questInfo_data = GameManager.instance.questinfo_data;
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
        // 안전 코드
        if (selectedDreamCatcherInventoryData == null)
        {
            Debug.LogError("[DeliveryManager] selectedDreamCatcherInventoryData is NULL");
            return DeliveryResult.Error;
        }

        // 현재 퀘스트에서 요구하는 드림캐쳐 불러오기
        string questDreamCatcher = questInfo_data.dataList[0].questDreamCatcher;

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
            DeliveryFailed();
            return DeliveryResult.WrongItem;
        }
        else
        {
            DeliverySuccess();
            return DeliveryResult.Success;
        }
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
        int requiredCount = questInfo_data.dataList[0].GetQuestDreamCatcherNum(); // 나중에 현재 퀘스트로 수정
        if (HasEnoughDreamCatchers(selectedDreamCatcherInventoryData))
        {
            for (int i = 0; i < requiredCount; i++)
            {
                dreamCatcherDataManager.RemoveDreamCatcher(dcIds[i]);
            }
        }
    }

    public void DeliverySuccess()
    {
        // 드림캐쳐 납품(삭제)
        DeliveryDreamCatcher();

        // 퀘스트 완료 처리

        // 현재 퀘스트 데이터 업데이트

        Debug.Log("dreamCatcher is Same");
    }

    public void DeliveryFailed()
    {
        // 드림캐쳐 납품(삭제)
        DeliveryDreamCatcher();

        Debug.Log("dreamCatcher is not Same");
    }

    public void SetSelectedDreamCatcherInventoryData(DreamCatcherInventoryData _selectedDreamCatcherInventoryData)
    {
        selectedDreamCatcherInventoryData = _selectedDreamCatcherInventoryData;
    }
}

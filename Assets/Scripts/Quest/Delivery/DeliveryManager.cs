using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    private DreamCatcherDataManager dreamCatcherDataManager;
    private QuestInfo_Data questInfo_data;

    private DreamCatcherInventoryData selectedDreamCatcherInventoryData;

    // Start is called before the first frame update
    void Start()
    {
        // 데이터 불러오기
        dreamCatcherDataManager = GameManager.instance.dreamCatcherDataManager;
        questInfo_data = GameManager.instance.questinfo_data;

        // 테스트 용 (나중에 이 함수를 UI 버튼 클릭 부분에서 호출)
        //TryDeliveryDreamCatcher("JS_1000");
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

    public void TryDeliveryDreamCatcher()
    {
        // 현재 퀘스트에서 요구하는 드림캐쳐 불러오기
        string questDreamCatcher = questInfo_data.dataList[0].questDreamCatcher;

        // 선택한 드림캐쳐 불러오기
        string selectedDreamCatcher = selectedDreamCatcherInventoryData.GetTemplateHash();

        // 드림캐쳐 비교 및 처리
        bool testResult = IsSameDreamCatcher(questDreamCatcher, selectedDreamCatcher);
        if(testResult)
        {
            DeliverySuccess();
        }
        else
        {
            DeliveryFailed();
        }
    }

    public void DeliverySuccess()
    {
        Debug.Log("dreamCatcher is Same");
        // 해당 드림캐쳐 삭제

        // 퀘스트 완료 처리
        // 현재 퀘스트 데이터 업데이트

    }

    public void DeliveryFailed()
    {
        Debug.Log("dreamCatcher is not Same");
    }

    private bool IsSameDreamCatcher(string dreamCatcher1, string dreamCatcher2)
    {
        if(dreamCatcher1 == null || dreamCatcher2 == null)
        {
            Debug.LogError("dreamCatcher1 or dreamCatcher2 is null 비교 불가");
            return false;
        }

        // 드림캐쳐 비교
        if (dreamCatcher1 != dreamCatcher2)
        {
            return false;
        }

        return true;
    }

    public void SetSelectedDreamCatcherInventoryData(DreamCatcherInventoryData _selectedDreamCatcherInventoryData)
    {
        selectedDreamCatcherInventoryData = _selectedDreamCatcherInventoryData;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUIController : MonoBehaviour
{
    [Header("[UI Object(CheckWin)]")]
    public Text description;
    public Image dreamcatcherImg;
    public ScriptBox scriptBox;

    [Space]
    [Header("[UI Panels]")]
    public GameObject QuestPanel;
    public GameObject InventoryPanel;

    [Space]
    [Header("[Managers]")]
    public GameObject managers;
    private DeliveryManager deliveryManager;
    private QuestActionController questActionManager;
    private PlayerDataManager playerDataManager;

    private DreamCatcherInventoryData selectedDreamCatcherInventoryData;

    private void Awake()
    {
        deliveryManager = managers.GetComponent<DeliveryManager>();
        questActionManager = managers.GetComponent<QuestActionController>();
    }

    private void Start()
    {
        playerDataManager = GameManager.instance.playerDataManager;
    }

    public void OpenCheckWin(DreamCatcherInventoryData dreamCatcherInventoryData)
    {
        // 창 활성화
        this.gameObject.SetActive(true);
        this.transform.GetChild(0).gameObject.SetActive(true);

        // 선택 드림캐쳐 표시
        description.text = dreamCatcherInventoryData.GetDescription();
        dreamcatcherImg.sprite = null; // 나중에 수정

        // 선택한 드림캐쳐 저장
        deliveryManager.SetSelectedDreamCatcherInventoryData(dreamCatcherInventoryData);
    }

    public void OpenObjectNotEnoughWin()
    {
        this.gameObject.SetActive(true);
        this.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void OnClickDeliveryButton()
    {
        // 인벤토리 창 비활성화
        InventoryPanel.SetActive(false);
        // 퀘스트 창 비활성화
        QuestPanel.SetActive(false);

        DeliveryResult deliveryResult = deliveryManager.TryDeliveryDreamCatcher();

        switch (deliveryResult)
        {
            case DeliveryResult.Success:
                // 퀘스트 종료 퀘스트 액션 재생
                deliveryManager.CompleteCurrentQuest();
                QuestPanel.GetComponent<QuestUIController>().OpenWhiteWallPaper();
                deliveryManager.MoveNextQuest();
                break;
            case DeliveryResult.WrongItem:
                // 잘못된 드림캐쳐 안내 대사창 재생
                scriptBox.ScriptBoxOnOff(true);
                scriptBox.SetScriptBox(7203, 7208);
                deliveryManager.DeliveryFailed();
                break;
            case DeliveryResult.Error:
                Debug.LogError("[DeliveryUIController] DeliveryResult is Error");
                break;
        }

        // 납품창 비활성화
        this.gameObject.SetActive(false);

    }

    public void CompleteMainQuest()
    {
        int completedQuestIndex = playerDataManager.GetCurrentMainQuestIndex() - 1;
        questActionManager.ActiveQuestEndActionActive(completedQuestIndex);
    }
}

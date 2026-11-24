using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    private QuestManager questManager;
    private PlayerDataManager playerDataManager;

    private DreamCatcherInventoryData selectedDreamCatcherInventoryData;

    [Header("공통 팝업")]
    [SerializeField] private GameObject unlockPopUp;
    [SerializeField] private Button okBtn;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text messageText;

    private void Awake()
    {
        questManager = managers.GetComponent<QuestManager>();
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
        dreamcatcherImg.sprite = DreamCatcherThumbnailRenderer.LoadThumbnail(dreamCatcherInventoryData.GetDCids()[0]);

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

        QuestType questType = questManager.GetCurrentQuestType();

        switch (questType)
        {
            case QuestType.MainQuest:
                HandleMainQuestDelivery(deliveryResult);
                break;

            case QuestType.RepeatQuest:
                HandleRepeatQuestDelivery(deliveryResult);
                break;
        }

        // 납품창 비활성화
        this.gameObject.SetActive(false);
    }

    private void HandleMainQuestDelivery(DeliveryResult result)
    {
        switch (result)
        {
            case DeliveryResult.Success:
                // 퀘스트 종료 퀘스트 액션 재생
                deliveryManager.CompleteCurrentQuest();
                QuestPanel.GetComponent<QuestUIController>().OpenWhiteWallPaper();
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
    }

    private void HandleRepeatQuestDelivery(DeliveryResult result)
    {
        switch (result)
        {
            case DeliveryResult.Success:
                // 반복 퀘스트 성공 연출
                Debug.Log("반복 퀘스트 성공");
                RepeatQuestDreamCatcher completedDreamCatcher = deliveryManager.GetMatchedRepeatQuestDreamCatcher();

                questManager.ClearRepeatQuest(completedDreamCatcher);
                break;

            case DeliveryResult.WrongItem:
                // 잘못된 드림캐쳐 안내 팝업 활성화
                if (!TutorialUnlockPopupUtility.Resolve(
                    this,
                    PopupType.Text,
                    ref unlockPopUp,
                    ref okBtn,
                    ref itemImage,
                    ref messageText))
                {
                    return;
                }

                TutorialUnlockPopupUtility.Show(
                    PopupType.Text,
                    unlockPopUp,
                    itemImage,
                    messageText,
                    null,
                    "납품 조건이 맞지 않습니다.");

                TutorialUnlockPopupUtility.BindOkButton(
                    okBtn,
                    OnOkButtonClicked);
                break;

            case DeliveryResult.Error:
                Debug.LogError("[DeliveryUI] Repeat Quest Delivery Error");
                break;
        }
    }
    
    private void OnOkButtonClicked()
    {
        TutorialUnlockPopupUtility.Hide(unlockPopUp);

        if (okBtn != null)
        {
            TutorialUnlockPopupUtility.UnbindOkButton(
                okBtn,
                OnOkButtonClicked);
        }
    }
}

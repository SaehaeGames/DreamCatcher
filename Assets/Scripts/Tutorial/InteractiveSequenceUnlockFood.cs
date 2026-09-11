using UnityEngine;
using UnityEngine.UI;

public enum FoodType
{
    None,
    PigeonBean,
    Berry,
    Earthworm,
    LeanMeat
}

public class InteractiveSequenceUnlockFood : InteractiveSequenceBase
{
    [Header("해금 종류")]
    public FoodType foodType;

    [Header("공통 팝업")]
    public GameObject unlockPopUp;
    public Button okBtn;
    public Image itemImage;
    public Text messageText;

    [Header("해금 표시")]
    public Sprite unlockIcon;
    [TextArea] public string unlockMessage;

    private PlayerDataManager playerDataManager;
    private TutorialPipeline tutorialPipeline;
    private bool isCompleted;
    private bool hasAppliedUnlock;
    private bool hasQueuedUnlock;

    public override void Enter()
    {
        isCompleted = false;
        hasAppliedUnlock = false;
        hasQueuedUnlock = false;
        playerDataManager = GameManager.instance.playerDataManager;
        tutorialPipeline = GetComponentInParent<TutorialPipeline>();


        if (!TutorialUnlockPopupUtility.Resolve(
                gameObject, ref unlockPopUp, ref okBtn, ref itemImage, ref messageText))
        {
            return;
        }

        TutorialUnlockPopupUtility.Show(
            unlockPopUp,
            itemImage,
            messageText,
            unlockIcon,
            string.IsNullOrWhiteSpace(unlockMessage) ? GetDefaultMessage() : unlockMessage);

        okBtn.onClick.RemoveListener(OnOkButtonClicked);
        okBtn.onClick.AddListener(OnOkButtonClicked);
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (isCompleted)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (isCompleted)
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    public override void Exit()
    {
        if (unlockPopUp != null)
        {
            unlockPopUp.SetActive(false);
        }

        if (okBtn != null)
        {
            okBtn.onClick.RemoveListener(OnOkButtonClicked);
        }

        isCompleted = false;
    }

    private void OnOkButtonClicked()
    {
        QueueOrApplyUnlock();
        isCompleted = true;
    }

    private void QueueOrApplyUnlock()
    {
        if (tutorialPipeline == null)
        {
            ApplyUnlock(true);
            return;
        }

        if (hasQueuedUnlock) return;

        tutorialPipeline.RegisterCompletionAction(ApplyUnlockWithoutSave);
        hasQueuedUnlock = true;
    }

    private void ApplyUnlockWithoutSave()
    {
        ApplyUnlock(false);
    }

    private void ApplyUnlock(bool saveImmediately)
    {
        if (hasAppliedUnlock) return;

        switch (foodType)
        {
            case FoodType.PigeonBean:
                playerDataManager.UnlockFoodLevel(0, saveImmediately);
                break;
            case FoodType.Berry:
                playerDataManager.UnlockFoodLevel(1, saveImmediately);
                break;
            case FoodType.Earthworm:
                playerDataManager.UnlockFoodLevel(2, saveImmediately);
                break;
            case FoodType.LeanMeat:
                playerDataManager.UnlockFoodLevel(3, saveImmediately);
                break;
            default:
                Debug.LogWarning($"[InteractiveSequenceUnlockFood] {gameObject.name}: 해금할 먹이 종류가 설정되지 않았습니다.");
                break;
        }

        hasAppliedUnlock = true;
    }

    private string GetDefaultMessage()
    {
        switch (foodType)
        {
            case FoodType.PigeonBean:
                return "비둘기콩이 해금되었습니다.";
            case FoodType.Berry:
                return "베리가 해금되었습니다.";
            case FoodType.Earthworm:
                return "지렁이가 해금되었습니다.";
            case FoodType.LeanMeat:
                return "살코기가 해금되었습니다.";
            default:
                return "먹이가 해금되었습니다.";
        }
    }
}

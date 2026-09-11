using UnityEngine;
using UnityEngine.UI;

public enum LetterType
{
    None,
    RepeatQuest,
    Charon
}

public class InteractiveSequenceUnlockLetter : InteractiveSequenceBase
{
    [Header("해금 종류")]
    public LetterType letterType;

    [Header("공통 팝업")]
    public GameObject unlockPopUp;
    public Button okBtn;
    public Image itemImage;
    public Text messageText;

    [Header("해금 표시")]
    public Sprite unlockIcon;
    [TextArea] public string unlockMessage;

    [Header("반복 퀘스트 보상")]
    public Sprite specialFeedIcon;
    [TextArea] public string specialFeedMessage = "특제먹이를 획득했습니다.";

    private PlayerDataManager playerDataManager;
    private TutorialPipeline tutorialPipeline;
    private bool isCompleted;
    private bool isShowingSpecialFeedReward;
    private bool hasAppliedUnlock;
    private bool hasQueuedUnlock;

    public override void Enter()
    {
        isCompleted = false;
        isShowingSpecialFeedReward = false;
        hasAppliedUnlock = false;
        hasQueuedUnlock = false;
        playerDataManager = GameManager.instance.playerDataManager;
        tutorialPipeline = GetComponentInParent<TutorialPipeline>();

        if (!TutorialUnlockPopupUtility.Resolve(
                gameObject, ref unlockPopUp, ref okBtn, ref itemImage, ref messageText))
        {
            return;
        }

        ShowUnlockPopup();

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
        isShowingSpecialFeedReward = false;
    }

    private void OnOkButtonClicked()
    {
        if (letterType == LetterType.RepeatQuest && !isShowingSpecialFeedReward)
        {
            isShowingSpecialFeedReward = true;
            TutorialUnlockPopupUtility.Show(
                unlockPopUp, itemImage, messageText, specialFeedIcon, specialFeedMessage);
            return;
        }

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

        if (letterType == LetterType.RepeatQuest)
        {
            playerDataManager.UnlockRepeatQuestWithSpecialFeed(1, saveImmediately);
        }
        else if (letterType == LetterType.Charon)
        {
            playerDataManager.UnlockCharonLetter(saveImmediately);
        }

        hasAppliedUnlock = true;
    }

    private void ShowUnlockPopup()
    {
        string defaultMessage = letterType == LetterType.RepeatQuest
            ? "반복 퀘스트가 해금되었습니다."
            : "카론의 편지가 해금되었습니다.";

        TutorialUnlockPopupUtility.Show(
            unlockPopUp,
            itemImage,
            messageText,
            unlockIcon,
            string.IsNullOrWhiteSpace(unlockMessage) ? defaultMessage : unlockMessage);
    }
}

internal static class TutorialUnlockPopupUtility
{
    public static bool Resolve(
        GameObject owner,
        ref GameObject unlockPopUp,
        ref Button okBtn,
        ref Image itemImage,
        ref Text messageText)
    {
        if (unlockPopUp == null)
        {
            GameObject tutorialOverlay = GameObject.FindGameObjectWithTag("TutorialOverlay");
            if (tutorialOverlay != null)
            {
                Transform popupTransform = tutorialOverlay.transform.Find("UnlockPopUp");
                if (popupTransform != null)
                {
                    unlockPopUp = popupTransform.gameObject;
                }
            }
        }

        if (unlockPopUp == null)
        {
            Debug.LogError($"[TutorialUnlockPopup] {owner.name}: UnlockPopUp을 찾을 수 없습니다.");
            return false;
        }

        if (okBtn == null)
        {
            Transform buttonTransform = unlockPopUp.transform.Find("OKBtn");
            if (buttonTransform != null)
            {
                okBtn = buttonTransform.GetComponent<Button>();
            }
        }

        if (itemImage == null)
        {
            Transform imageTransform = unlockPopUp.transform.Find("ItemFrame/ItemImage");
            if (imageTransform != null)
            {
                itemImage = imageTransform.GetComponent<Image>();
            }
        }

        if (messageText == null)
        {
            Transform textTransform = unlockPopUp.transform.Find("Text");
            if (textTransform != null)
            {
                messageText = textTransform.GetComponent<Text>();
            }
        }

        if (okBtn == null)
        {
            Debug.LogError($"[TutorialUnlockPopup] {owner.name}: OKBtn을 찾을 수 없습니다.");
            return false;
        }

        return true;
    }

    public static void Show(
        GameObject unlockPopUp,
        Image itemImage,
        Text messageText,
        Sprite icon,
        string message)
    {
        if (itemImage != null && icon != null)
        {
            itemImage.sprite = icon;
            itemImage.enabled = true;
        }

        if (messageText != null && !string.IsNullOrWhiteSpace(message))
        {
            messageText.text = message;
        }

        unlockPopUp.SetActive(true);
    }
}

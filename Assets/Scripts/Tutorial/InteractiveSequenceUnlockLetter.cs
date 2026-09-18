using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 튜토리얼에서 처리할 편지·반복 퀘스트 해금 종류를 구분함.
/// </summary>
public enum LetterType
{
    None,
    RepeatQuest,
    Charon
}

/// <summary>
/// 편지 또는 반복 퀘스트 해금 팝업을 표시하고 Tutorial Scene 완료 시 데이터 변경을 확정함.
/// 반복 퀘스트는 같은 팝업을 한 번 더 사용하여 특제 먹이 지급 안내를 보여줌.
/// </summary>
public class InteractiveSequenceUnlockLetter : InteractiveSequenceBase
{
    [Header("해금 종류")]
    [SerializeField] private LetterType letterType;

    [Header("공통 팝업")]
    [SerializeField] private GameObject unlockPopUp;
    [SerializeField] private Button okBtn;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text messageText;

    [Header("해금 표시")]
    [SerializeField] private Sprite unlockIcon;
    [SerializeField, TextArea] private string unlockMessage;

    [Header("반복 퀘스트 보상")]
    [SerializeField] private Sprite specialFeedIcon;
    [SerializeField, TextArea] private string specialFeedMessage = "특제먹이를 획득했습니다.";

    private UnlockRuntimeState runtime;

    /// <summary>
    /// 데이터 관리자와 공통 팝업을 준비하고 현재 해금 종류의 첫 안내를 표시함.
    /// </summary>
    public override void Enter()
    {
        // 1. 재실행 시 이전 팝업 단계와 완료·예약 상태를 비움.
        runtime = default;

        // 2. 해금 데이터 관리자와 현재 Tutorial Scene 파이프라인을 가져옴.
        if (GameManager.instance == null || GameManager.instance.playerDataManager == null)
        {
            Debug.LogError("[InteractiveSequenceUnlockLetter] PlayerDataManager를 찾을 수 없습니다.");
            return;
        }

        runtime.PlayerDataManager = GameManager.instance.playerDataManager;
        runtime.TutorialPipeline = GetComponentInParent<TutorialPipeline>();

        // 3. Inspector 참조 또는 공용 TutorialOverlay에서 팝업 구성 요소를 준비함.
        if (!TutorialUnlockPopupUtility.Resolve(
                this, ref unlockPopUp, ref okBtn, ref itemImage, ref messageText))
        {
            return;
        }

        // 4. 해금 안내를 표시하고 OK 입력을 구독함.
        ShowUnlockPopup();
        okBtn.onClick.RemoveListener(OnOkButtonClicked);
        okBtn.onClick.AddListener(OnOkButtonClicked);
    }

    /// <summary>
    /// 필요한 팝업 확인이 모두 끝나면 다음 튜토리얼 시퀀스로 진행함.
    /// </summary>
    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (runtime.IsCompleted)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    /// <summary>
    /// 필요한 팝업 확인이 모두 끝나면 다음 퀘스트 액션으로 진행함.
    /// </summary>
    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (runtime.IsCompleted)
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    /// <summary>
    /// 공통 팝업을 닫고 OK 리스너와 현재 팝업 단계를 정리함.
    /// </summary>
    public override void Exit()
    {
        // 1. 현재 팝업을 닫음.
        if (unlockPopUp != null)
        {
            unlockPopUp.SetActive(false);
        }

        // 2. 재활성화 때 클릭 콜백이 중복되지 않도록 등록한 리스너를 제거함.
        if (okBtn != null)
        {
            okBtn.onClick.RemoveListener(OnOkButtonClicked);
        }

        runtime.IsCompleted = false;
        runtime.IsShowingSpecialFeedReward = false;
    }

    /// <summary>
    /// 반복 퀘스트는 두 번째 보상 팝업을 표시하고 그 외에는 해금 작업을 예약한 뒤 완료함.
    /// </summary>
    private void OnOkButtonClicked()
    {
        // 1. 반복 퀘스트의 첫 확인에서는 같은 팝업을 특제 먹이 지급 안내로 교체함.
        if (letterType == LetterType.RepeatQuest && !runtime.IsShowingSpecialFeedReward)
        {
            runtime.IsShowingSpecialFeedReward = true;
            TutorialUnlockPopupUtility.Show(
                unlockPopUp,
                itemImage,
                messageText,
                specialFeedIcon,
                specialFeedMessage);
            return;
        }

        // 2. 팝업 확인 후 해금·보상 작업을 예약하고 시퀀스를 완료 상태로 바꿈.
        QueueOrApplyUnlock();
        runtime.IsCompleted = true;
    }

    /// <summary>
    /// 튜토리얼에서는 Scene 완료 작업으로 예약하고 공용 시퀀스로 단독 실행될 때는 즉시 저장함.
    /// </summary>
    private void QueueOrApplyUnlock()
    {
        // 1. TutorialPipeline 밖에서 실행된 경우에는 진행 번호 저장 주기가 없으므로 즉시 저장함.
        if (runtime.TutorialPipeline == null)
        {
            ApplyUnlock(true);
            return;
        }

        // 2. 완료 작업은 한 번만 등록함. (같은 OK 입력이 여러 번 전달되어도 한 번만 등록함.)
        if (runtime.HasQueuedUnlock)
        {
            return;
        }

        // 3. 강제 종료 시 같은 단계를 다시 시작할 수 있도록 Scene 마지막 시퀀스까지 실제 변경을 보류함.
        runtime.TutorialPipeline.RegisterCompletionAction(ApplyUnlockWithoutSave);
        runtime.HasQueuedUnlock = true;
    }

    /// <summary>
    /// TutorialManager가 진행 번호와 함께 저장할 수 있도록 저장 호출 없이 해금 데이터만 변경함.
    /// </summary>
    private void ApplyUnlockWithoutSave()
    {
        ApplyUnlock(false);
    }

    /// <summary>
    /// 선택한 종류에 따라 반복 퀘스트와 특제 먹이 또는 카론 편지를 한 번만 해금함.
    /// </summary>
    private void ApplyUnlock(bool saveImmediately)
    {
        // 1. 같은 예약 작업이나 콜백이 반복되어도 중복 해금과 보상 지급을 막음.
        if (runtime.HasAppliedUnlock)
        {
            return;
        }

        // 2. Inspector에서 선택한 종류에 맞는 PlayerDataManager 기능을 호출함.
        switch (letterType)
        {
            case LetterType.RepeatQuest:
                runtime.PlayerDataManager.UnlockRepeatQuestWithSpecialFeed(1, saveImmediately);
                break;
            case LetterType.Charon:
                runtime.PlayerDataManager.UnlockCharonLetter(saveImmediately);
                break;
            default:
                Debug.LogWarning($"[InteractiveSequenceUnlockLetter] {gameObject.name}: 해금 종류가 설정되지 않았습니다.");
                break;
        }

        runtime.HasAppliedUnlock = true;
    }

    /// <summary>
    /// Inspector 문구가 비어 있으면 종류별 기본 문구를 선택하여 공통 팝업에 표시함.
    /// </summary>
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

    /// <summary>
    /// 편지·반복 퀘스트 해금 단계에서만 사용하는 관리자와 팝업 진행 상태를 묶은 값 형식 상태.
    /// </summary>
    private struct UnlockRuntimeState
    {
        public PlayerDataManager PlayerDataManager;
        public TutorialPipeline TutorialPipeline;
        public bool IsCompleted;
        public bool IsShowingSpecialFeedReward;
        public bool HasAppliedUnlock;
        public bool HasQueuedUnlock;
    }
}

/// <summary>
/// 먹이와 편지 해금 시퀀스가 같은 TutorialOverlay 팝업을 찾고 아이콘·문구를 교체하는 공통 기능.
/// </summary>
internal static class TutorialUnlockPopupUtility
{
    /// <summary>
    /// Inspector 참조가 비어 있으면 TutorialOverlay의 고정 경로에서 팝업 구성 요소를 찾아 사용할 수 있는지 반환함.
    /// </summary>
    public static bool Resolve(
        Component owner,
        ref GameObject unlockPopUp,
        ref Button okBtn,
        ref Image itemImage,
        ref Text messageText)
    {
        // 1. 팝업 루트가 없으면 TutorialContext와 태그 검색 순서로 공용 UnlockPopUp을 찾음.
        if (unlockPopUp == null)
        {
            GameObject tutorialOverlay = ResolveTutorialOverlay(owner);
            Transform popupTransform = tutorialOverlay != null
                ? tutorialOverlay.transform.Find("UnlockPopUp")
                : null;
            unlockPopUp = popupTransform != null ? popupTransform.gameObject : null;
        }

        if (unlockPopUp == null)
        {
            Debug.LogError($"[TutorialUnlockPopup] {owner.name}: UnlockPopUp을 찾을 수 없습니다.");
            return false;
        }

        // 2. 개별 UI 참조가 없으면 팝업 내부의 정해진 경로에서 찾음.
        if (okBtn == null)
        {
            Transform buttonTransform = unlockPopUp.transform.Find("OKBtn");
            okBtn = buttonTransform != null ? buttonTransform.GetComponent<Button>() : null;
        }

        if (itemImage == null)
        {
            Transform imageTransform = unlockPopUp.transform.Find("ItemFrame/ItemImage");
            itemImage = imageTransform != null ? imageTransform.GetComponent<Image>() : null;
        }

        if (messageText == null)
        {
            Transform textTransform = unlockPopUp.transform.Find("Text");
            messageText = textTransform != null ? textTransform.GetComponent<Text>() : null;
        }

        // 3. 진행 입력에 필요한 OK 버튼을 찾음.
        if (okBtn == null)
        {
            Debug.LogError($"[TutorialUnlockPopup] {owner.name}: OKBtn을 찾을 수 없습니다.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 공통 팝업의 이전 내용을 지우고 현재 보상의 아이콘과 문구로 교체한 뒤 표시함.
    /// </summary>
    public static void Show(
        GameObject unlockPopUp,
        Image itemImage,
        Text messageText,
        Sprite icon,
        string message)
    {
        if (unlockPopUp == null)
        {
            return;
        }

        // 1. 아이콘이 없는 보상에서 이전 팝업 아이콘이 남지 않도록 Sprite와 활성 상태를 함께 갱신함.
        if (itemImage != null)
        {
            itemImage.sprite = icon;
            itemImage.enabled = icon != null;
        }

        // 2. 빈 문구도 그대로 반영하여 이전 보상 문구가 재사용되지 않게 함.
        if (messageText != null)
        {
            messageText.text = message ?? string.Empty;
        }

        // 3. 모든 내용을 교체한 뒤 팝업을 표시함.
        unlockPopUp.SetActive(true);
    }

    /// <summary>
    /// TutorialContext를 우선 사용하고 이전 씬 구성은 안전한 태그 검색으로 지원함.
    /// </summary>
    private static GameObject ResolveTutorialOverlay(Component owner)
    {
        TutorialContext context = TutorialContext.Get(owner);
        if (context != null && context.TutorialOverlay != null)
        {
            return context.TutorialOverlay;
        }

        try
        {
            return GameObject.FindGameObjectWithTag(Constants.Tag_TutorialOverlay);
        }
        catch (UnityException)
        {
            return null;
        }
    }
}
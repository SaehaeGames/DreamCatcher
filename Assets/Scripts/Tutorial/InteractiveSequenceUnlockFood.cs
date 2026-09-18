using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 튜토리얼에서 해금할 먹이와 PlayerData의 해금 레벨을 연결하는 값.
/// </summary>
public enum FoodType
{
    None,
    PigeonBean,
    Berry,
    Earthworm,
    LeanMeat
}

/// <summary>
/// 먹이 해금 팝업을 표시하고 현재 Tutorial Scene 전체가 끝날 때 해금 데이터를 확정함.
/// 단계 도중 종료하면 예약 작업이 취소되어 같은 단계를 처음부터 다시 진행할 수 있음.
/// </summary>
public class InteractiveSequenceUnlockFood : InteractiveSequenceBase
{
    [Header("해금 종류")]
    [SerializeField] private FoodType foodType;

    [Header("공통 팝업")]
    [SerializeField] private GameObject unlockPopUp;
    [SerializeField] private Button okBtn;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text messageText;

    [Header("해금 표시")]
    [SerializeField] private Sprite unlockIcon;
    [SerializeField, TextArea] private string unlockMessage;

    private UnlockRuntimeState runtime;

    /// <summary>
    /// 데이터 관리자와 공통 팝업을 준비하고 현재 먹이의 해금 안내를 표시함.
    /// </summary>
    public override void Enter()
    {
        // 1. 재실행 시 이전 완료·예약 상태를 비움.
        runtime = default;

        // 2. 해금 데이터 관리자와 현재 Tutorial Scene 파이프라인을 가져옴.
        if (GameManager.instance == null || GameManager.instance.playerDataManager == null)
        {
            Debug.LogError("[InteractiveSequenceUnlockFood] PlayerDataManager를 찾을 수 없습니다.");
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

        // 4. 현재 먹이의 아이콘과 안내 문구를 표시하고 OK 입력을 구독함.
        TutorialUnlockPopupUtility.Show(
            unlockPopUp,
            itemImage,
            messageText,
            unlockIcon,
            string.IsNullOrWhiteSpace(unlockMessage) ? GetDefaultMessage() : unlockMessage);

        okBtn.onClick.RemoveListener(OnOkButtonClicked);
        okBtn.onClick.AddListener(OnOkButtonClicked);
    }

    /// <summary>
    /// 팝업 확인이 끝나면 다음 튜토리얼 시퀀스로 진행함.
    /// </summary>
    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (runtime.IsCompleted)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    /// <summary>
    /// 팝업 확인이 끝나면 다음 퀘스트 액션으로 진행함.
    /// </summary>
    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (runtime.IsCompleted)
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    /// <summary>
    /// 공통 팝업을 닫고 이 단계가 등록한 OK 리스너와 완료 상태를 정리함.
    /// </summary>
    public override void Exit()
    {
        // 1. 다른 해금 단계가 같은 팝업을 다시 구성할 수 있도록 현재 팝업을 닫음.
        if (unlockPopUp != null)
        {
            unlockPopUp.SetActive(false);
        }

        // 2. 재활성화 때 클릭 콜백이 중복되지 않도록 직접 등록한 리스너를 제거함.
        if (okBtn != null)
        {
            okBtn.onClick.RemoveListener(OnOkButtonClicked);
        }

        runtime.IsCompleted = false;
    }

    /// <summary>
    /// 확인 입력 시 해금 작업을 예약하고 팝업 단계를 완료 상태로 바꿈.
    /// </summary>
    private void OnOkButtonClicked()
    {
        // 1. 현재 실행 환경에 맞게 해금을 완료 시점에 예약하거나 즉시 적용함.
        QueueOrApplyUnlock();

        // 2. 데이터 처리 요청이 끝난 뒤 파이프라인이 다음 시퀀스로 진행할 수 있게 함.
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
    /// 선택한 먹이를 해금 레벨로 변환하여 PlayerData에 한 번만 반영함.
    /// </summary>
    private void ApplyUnlock(bool saveImmediately)
    {
        // 1. 같은 예약 작업이나 콜백이 반복되어도 중복 해금을 막음.
        if (runtime.HasAppliedUnlock)
        {
            return;
        }

        // 2. Inspector의 FoodType을 PlayerData가 사용하는 해금 레벨로 변환함.
        int foodLevel = GetFoodLevel();
        if (foodLevel < 0)
        {
            Debug.LogWarning($"[InteractiveSequenceUnlockFood] {gameObject.name}: 해금할 먹이 종류가 설정되지 않았습니다.");
            runtime.HasAppliedUnlock = true;
            return;
        }

        // 3. 튜토리얼 완료 경로는 false로 전달해 진행 번호 저장과 한 번에 기록함.
        runtime.PlayerDataManager.UnlockFoodLevel(foodLevel, saveImmediately);
        runtime.HasAppliedUnlock = true;
    }

    /// <summary>
    /// FoodType을 PlayerData의 0부터 시작하는 먹이 해금 레벨로 변환함.
    /// </summary>
    private int GetFoodLevel()
    {
        switch (foodType)
        {
            case FoodType.PigeonBean:
                return 0;
            case FoodType.Berry:
                return 1;
            case FoodType.Earthworm:
                return 2;
            case FoodType.LeanMeat:
                return 3;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Inspector 안내 문구가 비어 있을 때 먹이 종류에 맞는 기본 문구를 반환함.
    /// </summary>
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

    /// <summary>
    /// 먹이 해금 단계에서만 사용하는 데이터 관리자와 완료 상태를 묶은 값 형식 상태.
    /// </summary>
    private struct UnlockRuntimeState
    {
        public PlayerDataManager PlayerDataManager;
        public TutorialPipeline TutorialPipeline;
        public bool IsCompleted;
        public bool HasAppliedUnlock;
        public bool HasQueuedUnlock;
    }
}
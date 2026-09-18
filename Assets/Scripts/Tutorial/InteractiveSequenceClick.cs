using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어가 지정된 UI 버튼을 클릭할 때까지 기다리는 튜토리얼 시퀀스.
/// 원본 버튼을 튜토리얼 오버레이 위에서도 누를 수 있도록 임시 정렬하고,
/// 단계가 끝나면 버튼의 부모·좌표·입력 컴포넌트를 시작 전 상태로 되돌림.
/// </summary>
public class InteractiveSequenceClick : InteractiveSequenceBase
{
    [Header("화살표 강조 ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;
    [SerializeField] private bool doClickBtn;
    [SerializeField] private Sprite arrowImg;

    [Header("클릭/드래그 대상-입력")]
    [SerializeField] private GameObject clickBtn;

    [Header("그림자 패널 사용-입력")]
    [SerializeField] private bool panelChange;
    [SerializeField] private int panelChangeNum;
    [SerializeField] private GameObject shadowPanal;
    [SerializeField] private Sprite[] shadowImages;

    [Header("분리된 씬 이동/그림자 설정")]
    [SerializeField] private bool useExplicitSettings;
    [SerializeField] private bool changeSceneOnComplete;
    [SerializeField] private SceneState nextSceneState = SceneState.None;
    [SerializeField] private bool showShadowPanel;
    [SerializeField] private int shadowImageIndex = -1;

    private const int TutorialSortingOrder = 100;

    // 기존 Scene 데이터의 panelChangeNum 값을 SceneState로 해석하기 위한 호환용 매핑.
    private static readonly SceneState[] LegacySceneStates =
    {
        SceneState.Main,
        SceneState.Making,
        SceneState.CollectionDream,
        SceneState.Store
    };

    // 실행 중 상태를 역할별 객체로 묶어, 시퀀스 필드가 서로 어떤 목적으로 쓰이는지 정리함.
    // 값 형식으로 보관해 시퀀스 컴포넌트마다 보조 객체를 추가 할당하지 않음.
    private RuntimeReferences runtime;
    private ButtonTransformSnapshot buttonTransform;
    private ButtonInputOverride buttonInput;
    private OverlayRuntimeState overlay;

    private int suspendedBottomBarMenu = -1;

    /// <summary>
    /// 클릭 대상과 공용 UI 참조를 준비하고, 튜토리얼용 강조 연출을 시작함.
    /// </summary>
    public override void Enter()
    {
        // 1. 클릭 대상과 클릭 완료 상태를 전달할 InteractiveButton을 검사함.
        if (!TryPrepareClickTarget())
        {
            return;
        }

        // 2. 현재 씬의 공용 UI·관리자 참조를 준비하고 이전 클릭 상태를 초기화함.
        ResolveRuntimeReferences();
        InitializeStepState();

        // 3. 일반 버튼 이동보다 튜토리얼 완료 저장이 먼저 처리되도록 BottomBar 이동을 잠시 중단함.
        SuspendBottomBarNavigation();

        // 4. Inspector 설정에 맞는 화살표 강조와 그림자 패널을 구성함.
        SetupTutorialVisuals();
    }

    /// <summary>
    /// 클릭 대상에 붙은 InteractiveButton이 클릭을 기록하면 다음 튜토리얼 단계로 이동함.
    /// </summary>
    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (runtime.InteractiveButton != null && runtime.InteractiveButton.GetButtonClicked())
        {
            tutorialPipeline.SetNextTutorial(GetNextSceneState());
        }
    }

    /// <summary>
    /// 클릭 대상에 붙은 InteractiveButton이 클릭을 기록하면 다음 퀘스트 액션으로 이동함.
    /// </summary>
    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (runtime.InteractiveButton != null && runtime.InteractiveButton.GetButtonClicked())
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    /// <summary>
    /// 시퀀스가 임시로 바꾼 BottomBar 리스너, 버튼 입력 계층, Transform, 강조 UI를 모두 복구함.
    /// 중간 종료나 오브젝트 비활성화 뒤에도 다음 실행이 이전 상태의 영향을 받지 않게 하는 정리함.
    /// </summary>
    public override void Exit()
    {
        // 1. 일반 메뉴 이동과 원본 버튼의 입력 컴포넌트 설정을 복구함.
        RestoreBottomBarNavigation();
        buttonInput.Restore();

        // 2. 오버레이 아래로 옮긴 버튼의 부모·순서·RectTransform을 원래 값으로 되돌림.
        buttonTransform.Restore(clickBtn);

        // 3. 화살표·차단 패널·동적으로 생성한 그림자 패널을 정리함.
        ClearTutorialVisuals();
    }

    /// <summary>
    /// 클릭 대상과 클릭 기록 컴포넌트가 준비되어 있는지 검사함.
    /// </summary>
    private bool TryPrepareClickTarget()
    {
        if (clickBtn == null)
        {
            Debug.LogError($"[InteractiveSequenceClick] {gameObject.name}의 clickBtn이 설정되지 않았습니다.");
            return false;
        }

        // InteractiveButton은 실제 Button.onClick을 튜토리얼 진행 상태로 전달하는 역할을 함.
        runtime.InteractiveButton = clickBtn.GetComponent<InteractiveButton>();
        if (runtime.InteractiveButton != null)
        {
            return true;
        }

        Debug.LogError($"[InteractiveSequenceClick] {clickBtn.name}에 InteractiveButton이 없습니다.");
        return false;
    }

    /// <summary>
    /// TutorialManager가 TutorialContext에서 공용 참조를 가져옴.
    /// Context가 없는 구성도 실행할 수 있도록 씬 검색은 호환용 보조로 남겨 둠.
    /// </summary>
    private void ResolveRuntimeReferences()
    {
        // 1. TutorialManager가 캐시한 Context에서 대사창과 UI Canvas를 우선 가져옴.
        runtime.Context = TutorialContext.Get(this);
        runtime.ScriptBox = runtime.Context != null
            ? runtime.Context.ScriptBox
            : FindObjectOfType<ScriptBox>();

        runtime.UiCanvas = runtime.Context != null ? runtime.Context.UiCanvas : null;
        if (runtime.UiCanvas == null)
        {
            runtime.UiCanvas = FindObjectWithTag(Constants.Tag_UICanvas);
        }

        // 2. Context가 없는 이전 씬 구성은 안전한 태그 검색으로 BottomBar를 보완함.
        runtime.BottomBar = runtime.Context != null ? runtime.Context.BottomBar : null;
        if (runtime.BottomBar == null)
        {
            GameObject bottomBarObject = FindObjectWithTag(Constants.Tag_BottomBar);
            if (bottomBarObject != null)
            {
                runtime.BottomBar = bottomBarObject.GetComponent<BottomBar>();
            }
        }
    }

    /// <summary>
    /// 이전 대사창을 닫고, 버튼의 클릭 기록을 초기화함.
    /// </summary>
    private void InitializeStepState()
    {
        if (transform.GetSiblingIndex() == 0 && runtime.ScriptBox != null)
        {
            runtime.ScriptBox.ScriptBoxOnOff(false);
        }

        runtime.InteractiveButton.SetButtonClicked(false);
    }

    /// <summary>
    /// 현재 단계 설정에 따라 화살표 강조와 그림자 패널을 각각 준비함.
    /// </summary>
    private void SetupTutorialVisuals()
    {
        // 1. 화살표 강조 단계는 오버레이 연출과 원본 버튼 입력 계층을 함께 준비함.
        if (highlightArrowOnOff)
        {
            SetupArrowHighlight();
            if (overlay.Root != null)
            {
                SetClickButtonToOverlay();
            }
        }

        // 2. 그림자 패널은 화살표 설정과 독립적으로 필요한 단계에서만 생성함.
        if (ShouldShowShadowPanel())
        {
            SetupShadowPanel();
        }
    }

    /// <summary>
    /// TutorialOverlay의 화살표와 BlockPanal을 켜고 현재 단계의 화살표 이미지를 재생함.
    /// BlockPanal은 강조 대상 이외의 UI가 튜토리얼 진행 중 눌리는 것을 막음.
    /// </summary>
    private void SetupArrowHighlight()
    {
        // 1. Context를 우선 사용하고 없으면 태그로 TutorialOverlay를 찾음.
        overlay.Root = runtime.Context != null ? runtime.Context.TutorialOverlay : null;
        if (overlay.Root == null)
        {
            overlay.Root = FindObjectWithTag(Constants.Tag_TutorialOverlay);
        }

        if (overlay.Root == null)
        {
            Debug.LogError($"[InteractiveSequenceClick] {gameObject.name}에서 TutorialOverlay를 찾을 수 없습니다.");
            return;
        }

        // 2. 이름이 고정된 화살표와 입력 차단 패널을 확인함.
        Transform arrowTransform = overlay.Root.transform.Find("ArrowImage");
        Transform blockTransform = overlay.Root.transform.Find("BlockPanal");
        if (arrowTransform == null || blockTransform == null)
        {
            Debug.LogError($"[InteractiveSequenceClick] {overlay.Root.name}에 ArrowImage 또는 BlockPanal이 없습니다.");
            return;
        }

        overlay.Arrow = arrowTransform.gameObject;
        overlay.Block = blockTransform.gameObject;

        Image arrowImage = overlay.Arrow.GetComponent<Image>();
        Animator arrowAnimator = overlay.Arrow.GetComponent<Animator>();
        if (arrowImage == null || arrowAnimator == null)
        {
            Debug.LogError("[InteractiveSequenceClick] ArrowImage의 Image 또는 Animator가 없습니다.");
            return;
        }

        // 3. 현재 단계의 Sprite를 적용하고 깜박임 애니메이션을 시작함.
        overlay.Arrow.SetActive(true);
        overlay.Block.SetActive(true);
        arrowImage.sprite = arrowImg;
        arrowAnimator.enabled = true;
        arrowAnimator.Play("blinkArrow");
    }

    /// <summary>
    /// 원본 버튼의 상태를 먼저 저장한 뒤 튜토리얼 오버레이 위에서 입력받을 수 있게 만듦.
    /// doClickBtn이 켜진 단계는 따로 위치를 바꾸는 등 부모를 바꾸지 않고 독립적인 Canvas 정렬을 사용하여 원래 좌표를 보존함.
    /// </summary>
    private void SetClickButtonToOverlay()
    {
        // 1. 종료 시 복구할 원본 계층과 UI 배치 값을 저장함.
        buttonTransform.Capture(clickBtn);

        // 2. 원본 좌표를 유지해야 하는 버튼은 독립적인 Canvas 정렬만 임시 적용함.
        if (doClickBtn)
        {
            buttonInput.Apply(clickBtn, TutorialSortingOrder);
            return;
        }

        // 3. 일부 이전 단계는 오버레이의 자식으로 위치가 바뀌는 방식으로 되어 있어 기존 동작을 유지함.
        clickBtn.transform.SetParent(overlay.Root.transform, true);
    }

    /// <summary>
    /// BottomBar의 일반 씬 이동 리스너를 임시 제거함.
    /// Button.onClick이 Update보다 먼저 실행될 수 있으므로, 튜토리얼 완료 저장 전에 씬이 바뀔 수 있는 것을 방지하기 위함.
    /// </summary>
    private void SuspendBottomBarNavigation()
    {
        if (runtime.BottomBar == null || !runtime.BottomBar.TryGetMenuIndex(clickBtn, out int menu))
        {
            return;
        }

        suspendedBottomBarMenu = menu;
        runtime.BottomBar.onClickRemove(menu);
    }

    /// <summary>
    /// 시퀀스가 끝난 뒤 제거했던 BottomBar의 일반 씬 이동 리스너를 복구함.
    /// </summary>
    private void RestoreBottomBarNavigation()
    {
        if (runtime.BottomBar != null && suspendedBottomBarMenu >= 0)
        {
            runtime.BottomBar.OnClickAdd(suspendedBottomBarMenu);
        }

        suspendedBottomBarMenu = -1;
    }

    /// <summary>
    /// 선택된 그림자 이미지를 가진 패널을 UI Canvas 아래에 생성함.
    /// 인스턴스는 원본 프리팹을 수정하지 않으며 Exit에서 제거함.
    /// </summary>
    private void SetupShadowPanel()
    {
        // 1. 현재 설정 방식에서 사용할 이미지 인덱스와 필수 참조를 검사함.
        int imageIndex = GetShadowImageIndex();
        if (shadowPanal == null || runtime.UiCanvas == null || shadowImages == null
            || imageIndex < 0 || imageIndex >= shadowImages.Length)
        {
            Debug.LogError($"[InteractiveSequenceClick] {gameObject.name}의 그림자 패널 설정이 올바르지 않습니다.");
            return;
        }

        // 2. 원본 프리팹을 변경하지 않도록 UI Canvas 아래에 별도 인스턴스를 생성함.
        overlay.ShadowInstance = Instantiate(shadowPanal, runtime.UiCanvas.transform, false);
        Image shadowImage = overlay.ShadowInstance.GetComponent<Image>();
        if (shadowImage != null)
        {
            shadowImage.sprite = shadowImages[imageIndex];
            return;
        }

        // 3. Image가 없는 잘못된 인스턴스는 즉시 제거하여 빈 패널이 입력을 막지 않게 함.
        Debug.LogError("[InteractiveSequenceClick] 그림자 패널 프리팹에 Image가 없습니다.");
        Destroy(overlay.ShadowInstance);
        overlay.ShadowInstance = null;
    }

    /// <summary>
    /// 이 단계가 끝날 때 이동할 씬을 반환함.
    /// </summary>
    private SceneState GetNextSceneState()
    {
        if (useExplicitSettings)
        {
            return changeSceneOnComplete ? nextSceneState : SceneState.None;
        }

        /// 기존 Scene 데이터는 panelChangeNum 배열 매핑으로 해석함.
        return panelChange && panelChangeNum >= 0 && panelChangeNum < LegacySceneStates.Length
            ? LegacySceneStates[panelChangeNum]
            : SceneState.None;
    }

    /// <summary>
    /// 그림자 패널 표시 여부를 확인함.
    /// </summary>
    private bool ShouldShowShadowPanel()
    {
        return useExplicitSettings ? showShadowPanel : panelChange;
    }

    /// <summary>
    /// 그림자 이미지 인덱스를 반환함.
    /// </summary>
    private int GetShadowImageIndex()
    {
        return useExplicitSettings ? shadowImageIndex : panelChangeNum;
    }

    /// <summary>
    /// 화살표·차단 패널·동적 그림자 패널을 종료 상태로 되돌림.
    /// 기존 오버레이 오브젝트는 끄고, 이 시퀀스가 생성한 그림자 인스턴스만 파괴함.
    /// </summary>
    private void ClearTutorialVisuals()
    {
        // 1. 공용 화살표의 단계별 Sprite를 지우고 오브젝트를 비활성화함.
        if (overlay.Arrow != null)
        {
            Image arrowImage = overlay.Arrow.GetComponent<Image>();
            if (arrowImage != null)
            {
                arrowImage.sprite = null;
            }

            overlay.Arrow.SetActive(false);
        }

        // 2. 강조 대상 이외의 입력을 막던 BlockPanal을 끔.
        if (overlay.Block != null)
        {
            overlay.Block.SetActive(false);
        }

        // 3. 이 시퀀스가 직접 생성한 그림자 인스턴스만 파괴함.
        if (overlay.ShadowInstance != null)
        {
            Destroy(overlay.ShadowInstance);
        }

        overlay.Clear();
    }

    /// <summary>
    /// 태그가 비어 있거나 프로젝트에 등록되지 않은 경우에도 초기화 전체가 중단되지 않게 안전하게 검색함.
    /// </summary>
    private static GameObject FindObjectWithTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return null;
        }

        try
        {
            return GameObject.FindGameObjectWithTag(tag);
        }
        catch (UnityException)
        {
            return null;
        }
    }

    /// <summary>
    /// 씬에서 공유하는 참조를 한곳에 모은 실행 상태.
    /// Inspector 데이터가 아니므로 직렬화하지 않고 Enter마다 현재 씬 기준으로 다시 넣음.
    /// </summary>
    private struct RuntimeReferences
    {
        public TutorialContext Context;
        public ScriptBox ScriptBox;
        public GameObject UiCanvas;
        public BottomBar BottomBar;
        public InteractiveButton InteractiveButton;
    }

    /// <summary>
    /// 버튼을 오버레이 자식으로 옮기기 전에 저장하는 Transform 스냅샷.
    /// RectTransform의 앵커와 크기까지 저장해야 서로 다른 Canvas 아래로 이동해도 정확히 복원할 수 있음.
    /// </summary>
    private struct ButtonTransformSnapshot
    {
        private Transform parent;
        private int siblingIndex;
        private RectTransform rectTransform;
        private Vector2 anchorMin;
        private Vector2 anchorMax;
        private Vector2 pivot;
        private Vector2 anchoredPosition;
        private Vector2 sizeDelta;
        private Vector3 localScale;
        private Quaternion localRotation;
        private bool captured;

        /// <summary>
        /// 대상 버튼의 현재 부모 순서와 UI 배치 값을 한 번의 복구 단위로 저장함.
        /// </summary>
        public void Capture(GameObject target)
        {
            Transform targetTransform = target.transform;
            parent = targetTransform.parent;
            siblingIndex = targetTransform.GetSiblingIndex();
            rectTransform = targetTransform as RectTransform;
            localScale = targetTransform.localScale;
            localRotation = targetTransform.localRotation;
            captured = true;

            if (rectTransform == null)
            {
                return;
            }

            anchorMin = rectTransform.anchorMin;
            anchorMax = rectTransform.anchorMax;
            pivot = rectTransform.pivot;
            anchoredPosition = rectTransform.anchoredPosition;
            sizeDelta = rectTransform.sizeDelta;
        }

        /// <summary>
        /// 저장한 부모·순서·RectTransform 값을 대상 버튼에 다시 적용함.
        /// </summary>
        public void Restore(GameObject target)
        {
            if (!captured || target == null)
            {
                return;
            }

            Transform targetTransform = target.transform;
            targetTransform.SetParent(parent, false);

            if (parent != null)
            {
                // 부모에 다시 붙인 뒤 유효한 범위로 보정하여 원래 UI 그리기 순서에 되돌림.
                targetTransform.SetSiblingIndex(Mathf.Clamp(siblingIndex, 0, parent.childCount - 1));
            }

            targetTransform.localScale = localScale;
            targetTransform.localRotation = localRotation;

            if (rectTransform != null)
            {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = sizeDelta;
            }

            captured = false;
        }
    }

    /// <summary>
    /// 원본 버튼을 오버레이보다 높은 정렬 순서에서 클릭 가능하게 만드는 임시 입력 상태.
    /// 기존 컴포넌트는 설정만 저장·복원하고, 이 시퀀스가 추가한 컴포넌트만 제거함.
    /// </summary>
    private struct ButtonInputOverride
    {
        private Canvas canvas;
        private GraphicRaycaster raycaster;
        private bool addedCanvas;
        private bool addedRaycaster;
        private bool originalOverrideSorting;
        private int originalSortingOrder;
        private bool originalCanvasEnabled;
        private bool originalRaycasterEnabled;

        /// <summary>
        /// 대상에 Canvas와 GraphicRaycaster가 없으면 임시로 추가하고, 있으면 기존 설정을 보관한 뒤 덮어씀.
        /// </summary>
        public void Apply(GameObject target, int sortingOrder)
        {
            canvas = target.GetComponent<Canvas>();
            addedCanvas = canvas == null;
            if (addedCanvas)
            {
                canvas = target.AddComponent<Canvas>();
            }
            else
            {
                originalCanvasEnabled = canvas.enabled;
                originalOverrideSorting = canvas.overrideSorting;
                originalSortingOrder = canvas.sortingOrder;
            }

            raycaster = target.GetComponent<GraphicRaycaster>();
            addedRaycaster = raycaster == null;
            if (addedRaycaster)
            {
                raycaster = target.AddComponent<GraphicRaycaster>();
            }
            else
            {
                originalRaycasterEnabled = raycaster.enabled;
            }

            // 독립 Canvas가 오버레이보다 먼저 Raycast를 받도록 활성화하고 정렬 순서를 높임.
            canvas.enabled = true;
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;
            raycaster.enabled = true;
        }

        /// <summary>
        /// 임시 컴포넌트는 제거하고 원래 존재하던 컴포넌트에는 저장해 둔 설정을 복구함.
        /// </summary>
        public void Restore()
        {
            if (raycaster != null && addedRaycaster)
            {
                UnityEngine.Object.Destroy(raycaster);
            }

            if (canvas != null)
            {
                if (addedCanvas)
                {
                    UnityEngine.Object.Destroy(canvas);
                }
                else
                {
                    canvas.enabled = originalCanvasEnabled;
                    canvas.overrideSorting = originalOverrideSorting;
                    canvas.sortingOrder = originalSortingOrder;
                }
            }

            if (raycaster != null && !addedRaycaster)
            {
                raycaster.enabled = originalRaycasterEnabled;
            }

            canvas = null;
            raycaster = null;
            addedCanvas = false;
            addedRaycaster = false;
        }
    }

    /// <summary>
    /// 현재 시퀀스가 사용 중인 오버레이 오브젝트를 묶은 실행 상태.
    /// </summary>
    private struct OverlayRuntimeState
    {
        public GameObject Root;
        public GameObject Arrow;
        public GameObject Block;
        public GameObject ShadowInstance;

        /// <summary>
        /// 파괴되거나 비활성화된 오브젝트 참조가 다음 실행에 남지 않도록 실행 상태를 비움.
        /// </summary>
        public void Clear()
        {
            Root = null;
            Arrow = null;
            Block = null;
            ShadowInstance = null;
        }
    }
}

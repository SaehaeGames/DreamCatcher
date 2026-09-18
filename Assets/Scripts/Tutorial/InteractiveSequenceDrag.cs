using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 지정한 오브젝트의 유효한 드래그 완료를 기다리며 화살표와 입력 대상의 표시 계층을 임시로 조정함.
/// 단계가 끝나면 드래그 대상과 판정 대상의 부모·형제 순서, 오버레이 UI를 모두 복구함.
/// </summary>
public class InteractiveSequenceDrag : InteractiveSequenceBase
{
    [Header("화살표 강조 ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;
    [SerializeField] private Sprite arrowImg;

    [Header("클릭/드래그 대상-입력")]
    [SerializeField] private GameObject dragObj;

    private DragRuntimeState runtime;

    /// <summary>
    /// 드래그 판정 컴포넌트와 공용 UI를 준비하고 현재 설정에 맞는 입력 계층을 구성함.
    /// </summary>
    public override void Enter()
    {
        // 1. 재실행 시 이전 참조를 비우고 드래그 대상 설정을 검사함.
        runtime = default;
        if (!TryResolveDragTarget())
        {
            return;
        }

        // 2. TutorialContext에서 공용 오버레이와 Canvas 참조를 가져옴.
        ResolveSharedReferences();

        // 3. 이전 드래그 완료 상태를 초기화하고 대상 오브젝트의 원래 계층을 저장함.
        runtime.DragHandler.SetObjectDragged(false);
        runtime.DragObjectHierarchy.Capture(dragObj.transform);

        // 4. 화살표 사용 여부에 따라 판정 대상을 오버레이 또는 UI Canvas 아래에 배치함.
        if (highlightArrowOnOff)
        {
            if (!SetupArrowHighlight())
            {
                return;
            }

            dragObj.transform.SetParent(runtime.Arrow.transform, true);
            runtime.DragHandler.SetTargetParent(runtime.Arrow.transform);
            return;
        }

        if (runtime.UiCanvas == null)
        {
            Debug.LogError($"[InteractiveSequenceDrag] {gameObject.name}: UI Canvas를 찾을 수 없습니다.");
            return;
        }

        runtime.DragHandler.SetTargetParent(runtime.UiCanvas.transform);
    }

    /// <summary>
    /// 유효한 드래그가 완료되면 다음 튜토리얼 시퀀스로 진행함.
    /// </summary>
    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (HasCompletedDrag())
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    /// <summary>
    /// 유효한 드래그가 완료되면 다음 퀘스트 액션으로 진행함.
    /// </summary>
    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (HasCompletedDrag())
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    /// <summary>
    /// 드래그 상태, 대상 계층, 화살표와 입력 차단 패널을 시작 전 상태로 복구함.
    /// </summary>
    public override void Exit()
    {
        // 1. 드롭 판정을 위해 옮긴 타겟들과 완료 상태를 초기화함.
        if (runtime.DragHandler != null)
        {
            runtime.DragHandler.SetObjectDragged(false);
            runtime.DragHandler.ClearTargetConfiguration();
        }

        // 2. 화살표 아래로 옮긴 실제 드래그 오브젝트를 원래 부모와 형제 순서로 되돌림.
        runtime.DragObjectHierarchy.Restore();

        // 3. 이 단계가 사용한 오버레이 이미지를 지우고 강조 UI를 끔.
        if (runtime.ArrowImage != null)
        {
            runtime.ArrowImage.sprite = null;
        }

        if (runtime.Arrow != null)
        {
            runtime.Arrow.SetActive(false);
        }

        if (runtime.BlockPanel != null)
        {
            runtime.BlockPanel.SetActive(false);
        }

        // 4. 파괴되거나 비활성화될 수 있는 씬 참조를 다음 실행에 남기지 않도록 함.
        runtime = default;
    }

    /// <summary>
    /// 드래그 대상에 완료 상태를 기록할 InteractiveDragObj가 있는지 검사함.
    /// </summary>
    private bool TryResolveDragTarget()
    {
        if (dragObj == null)
        {
            Debug.LogError($"[InteractiveSequenceDrag] {gameObject.name}의 dragObj가 설정되지 않았습니다.");
            return false;
        }

        runtime.DragHandler = dragObj.GetComponent<InteractiveDragObj>();
        if (runtime.DragHandler != null)
        {
            return true;
        }

        Debug.LogError($"[InteractiveSequenceDrag] {dragObj.name}에 InteractiveDragObj가 없습니다.");
        return false;
    }

    /// <summary>
    /// TutorialContext의 캐시를 우선 사용하고 이전 씬 구성은 태그 검색으로 지원함.
    /// </summary>
    private void ResolveSharedReferences()
    {
        runtime.Context = TutorialContext.Get(this);
        runtime.UiCanvas = runtime.Context != null ? runtime.Context.UiCanvas : null;
        runtime.Overlay = runtime.Context != null ? runtime.Context.TutorialOverlay : null;

        if (runtime.UiCanvas == null)
        {
            runtime.UiCanvas = FindObjectWithTag(Constants.Tag_UICanvas);
        }

        if (runtime.Overlay == null)
        {
            runtime.Overlay = FindObjectWithTag(Constants.Tag_TutorialOverlay);
        }
    }

    /// <summary>
    /// TutorialOverlay의 화살표와 차단 패널을 찾아 현재 단계의 강조 연출을 시작함.
    /// </summary>
    private bool SetupArrowHighlight()
    {
        // 1. 공용 오버레이와 이름이 고정된 자식 오브젝트를 확인함.
        if (runtime.Overlay == null)
        {
            Debug.LogError($"[InteractiveSequenceDrag] {gameObject.name}: TutorialOverlay를 찾을 수 없습니다.");
            return false;
        }

        Transform arrowTransform = runtime.Overlay.transform.Find("ArrowImage");
        Transform blockTransform = runtime.Overlay.transform.Find("BlockPanal");
        if (arrowTransform == null || blockTransform == null)
        {
            Debug.LogError($"[InteractiveSequenceDrag] {runtime.Overlay.name}: ArrowImage 또는 BlockPanal이 없습니다.");
            return false;
        }

        runtime.Arrow = arrowTransform.gameObject;
        runtime.BlockPanel = blockTransform.gameObject;
        runtime.ArrowImage = runtime.Arrow.GetComponent<Image>();
        runtime.ArrowAnimator = runtime.Arrow.GetComponent<Animator>();

        // 2. 화살표 연출에 필요한 UI 컴포넌트를 검사함.
        if (runtime.ArrowImage == null || runtime.ArrowAnimator == null)
        {
            Debug.LogError("[InteractiveSequenceDrag] ArrowImage의 Image 또는 Animator가 없습니다.");
            return false;
        }

        // 3. 입력 차단 패널과 화살표를 켜고 현재 단계의 이미지를 적용함.
        runtime.BlockPanel.SetActive(true);
        runtime.Arrow.SetActive(true);
        runtime.ArrowImage.sprite = arrowImg;
        runtime.ArrowAnimator.enabled = true;
        runtime.ArrowAnimator.Play("blinkArrow");
        return true;
    }

    /// <summary>
    /// InteractiveDragObj가 기록한 실제 드래그 완료 여부를 반환함.
    /// </summary>
    private bool HasCompletedDrag()
    {
        return runtime.DragHandler != null && runtime.DragHandler.GetObjectDraged();
    }

    /// <summary>
    /// 태그가 비어 있는 경우에 검색함.
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
    /// 실행 중 사용하는 씬 참조와 임시 UI 상태를 역할별로 묶은 값 형식 상태.
    /// </summary>
    private struct DragRuntimeState
    {
        public TutorialContext Context;
        public GameObject UiCanvas;
        public GameObject Overlay;
        public GameObject Arrow;
        public GameObject BlockPanel;
        public Image ArrowImage;
        public Animator ArrowAnimator;
        public InteractiveDragObj DragHandler;
        public TransformHierarchySnapshot DragObjectHierarchy;
    }

    /// <summary>
    /// 오버레이 아래로 이동한 실제 드래그 오브젝트를 원래 계층으로 되돌리기 위한 스냅샷.
    /// </summary>
    private struct TransformHierarchySnapshot
    {
        private Transform target;
        private Transform parent;
        private int siblingIndex;
        private bool captured;

        /// <summary>
        /// 대상과 현재 부모·형제 순서를 저장함.
        /// </summary>
        public void Capture(Transform targetTransform)
        {
            target = targetTransform;
            parent = targetTransform.parent;
            siblingIndex = targetTransform.GetSiblingIndex();
            captured = true;
        }

        /// <summary>
        /// 저장한 부모와 형제 순서를 대상에 다시 적용함.
        /// </summary>
        public void Restore()
        {
            if (!captured || target == null)
            {
                return;
            }

            target.SetParent(parent, true);
            if (parent != null)
            {
                target.SetSiblingIndex(Mathf.Clamp(siblingIndex, 0, parent.childCount - 1));
            }

            captured = false;
        }
    }
}
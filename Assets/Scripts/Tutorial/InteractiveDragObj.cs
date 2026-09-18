using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 튜토리얼에서 지정한 오브젝트의 드래그 성공 여부를 감지함.
/// 실제 이동과 먹이 배치는 기존 드래그 컴포넌트가 담당하고 이 클래스는 튜토리얼 완료 여부만 기록함.
/// </summary>
public class InteractiveDragObj : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("드롭 판정 대상")]
    [SerializeField] private GameObject[] targets;

    private FeedDrag feedDrag;
    private TargetParentSnapshot[] targetSnapshots;
    private bool objectDraged;
    private bool isTargetConfigured;

    /// <summary>
    /// 먹이 드래그 컴포넌트가 있으면 실제 배치 완료 이벤트를 구독함.
    /// </summary>
    private void Awake()
    {
        feedDrag = GetComponent<FeedDrag>();
        if (feedDrag != null)
        {
            feedDrag.PlacementCompleted += OnFeedPlacementCompleted;
        }
    }

    /// <summary>
    /// 첫 실행에서 이전 완료 상태가 남지 않도록 초기화함.
    /// </summary>
    private void Start()
    {
        objectDraged = false;
    }

    /// <summary>
    /// 오브젝트 파괴 시 FeedDrag 이벤트 구독을 해제함.
    /// </summary>
    private void OnDestroy()
    {
        if (feedDrag != null)
        {
            feedDrag.PlacementCompleted -= OnFeedPlacementCompleted;
        }
    }

    /// <summary>
    /// 새 드래그가 시작되면 이전 완료 상태를 지우고 튜토리얼 대상 설정 여부를 확인함.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. 같은 오브젝트의 이전 드래그 결과를 초기화함.
        objectDraged = false;

        // 2. 시퀀스가 타겟을 준비하지 않았다면 경고 로그를 남김.
        if (!isTargetConfigured)
        {
            Debug.LogWarning($"[InteractiveDragObj] {gameObject.name}: 드래그 타겟이 준비되지 않았습니다.");
        }
    }

    /// <summary>
    /// 외부 시퀀스가 드래그 완료 상태를 설정함.
    /// </summary>
    public void SetObjectDragged(bool isDragged)
    {
        objectDraged = isDragged;
    }

    /// <summary>
    /// 현재 드래그가 유효한 배치로 끝났는지 반환함.
    /// </summary>
    public bool GetObjectDraged()
    {
        return objectDraged;
    }

    /// <summary>
    /// 드롭 판정 대상들을 임시 부모 아래로 옮기고 원래 부모와 형제 순서를 저장함.
    /// </summary>
    public void SetTargetParent(Transform temporaryParent)
    {
        // 1. 이전 단계에서 남은 부모 변경이 있으면 먼저 복구함.
        RestoreTargets();

        // 2. 필수 설정을 검사하고 대상 수만큼 스냅샷 공간을 준비함.
        if (temporaryParent == null || targets == null || targets.Length == 0)
        {
            Debug.LogError($"[InteractiveDragObj] {gameObject.name}: 드래그 타겟 설정값이 없습니다.");
            isTargetConfigured = false;
            return;
        }

        targetSnapshots = new TargetParentSnapshot[targets.Length];
        int configuredTargetCount = 0;

        // 3. 각 대상의 원래 계층 정보를 저장한 뒤 입력 오버레이 아래로 옮김.
        for (int i = 0; i < targets.Length; i++)
        {
            GameObject target = targets[i];
            if (target == null)
            {
                continue;
            }

            targetSnapshots[i].Capture(target.transform);
            target.transform.SetParent(temporaryParent, true);
            configuredTargetCount++;
        }

        isTargetConfigured = configuredTargetCount > 0;
    }

    /// <summary>
    /// 임시로 옮긴 드롭 판정 대상들을 원래 부모와 형제 순서로 되돌림.
    /// </summary>
    public void RestoreTargets()
    {
        if (targetSnapshots == null)
        {
            return;
        }

        // 1. 저장된 모든 대상의 계층을 복구함.
        for (int i = 0; i < targetSnapshots.Length; i++)
        {
            targetSnapshots[i].Restore();
        }

        // 2. 복구한 스냅샷을 비워 다음 단계에서 이전 참조를 재사용하지 않게 함.
        targetSnapshots = null;
    }

    /// <summary>
    /// 대상 부모를 복구하고 현재 튜토리얼의 드래그 판정을 비활성화함.
    /// </summary>
    public void ClearTargetConfiguration()
    {
        RestoreTargets();
        isTargetConfigured = false;
    }

    /// <summary>
    /// 드래그가 끝난 위치 또는 FeedDrag의 실제 횃대 배치 결과로 성공 여부를 확정함.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // 1. 먹이 드래그는 시각적 위치 대신 실제 횃대 배치 번호를 성공 기준으로 사용함.
        if (feedDrag != null && feedDrag.LastRackNumber >= 0)
        {
            objectDraged = true;
            return;
        }

        // 2. 일반 UI 드래그는 포인터가 대상 또는 그 자식 계층 위에서 끝났는지 검사함.
        // 실패 후 다시 시도할 수 있도록 임시 부모는 시퀀스 Exit까지 유지함.
        GameObject droppedObject = eventData != null
            ? eventData.pointerCurrentRaycast.gameObject
            : null;
        objectDraged = IsDroppedOnAnyTarget(droppedObject);

        if (targets == null || targets.Length == 0)
        {
            Debug.LogError($"[InteractiveDragObj] {gameObject.name}: 드롭 판정 대상이 없습니다.");
        }
    }

    /// <summary>
    /// FeedDrag가 실제 횃대 배치를 끝냈을 때 전달한 결과를 튜토리얼 완료 상태에 반영함.
    /// </summary>
    private void OnFeedPlacementCompleted(bool succeeded)
    {
        objectDraged = succeeded;
    }

    /// <summary>
    /// 드롭된 오브젝트가 등록된 대상 중 하나와 같은 계층에 속하는지 검사함.
    /// </summary>
    private bool IsDroppedOnAnyTarget(GameObject droppedObject)
    {
        if (targets == null)
        {
            return false;
        }

        for (int i = 0; i < targets.Length; i++)
        {
            if (IsTargetOrChild(droppedObject, targets[i]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 두 오브젝트가 동일하거나 서로 부모·자식 관계인지 확인함.
    /// </summary>
    private static bool IsTargetOrChild(GameObject droppedObject, GameObject target)
    {
        if (droppedObject == null || target == null)
        {
            return false;
        }

        Transform droppedTransform = droppedObject.transform;
        Transform targetTransform = target.transform;
        return droppedTransform == targetTransform
            || droppedTransform.IsChildOf(targetTransform)
            || targetTransform.IsChildOf(droppedTransform);
    }

    /// <summary>
    /// 오버레이 아래로 이동한 드롭 판정 대상의 원래 계층을 복구하기 위한 값 형식 스냅샷.
    /// </summary>
    private struct TargetParentSnapshot
    {
        private Transform target;
        private Transform parent;
        private int siblingIndex;
        private bool captured;

        /// <summary>
        /// 대상 Transform과 현재 부모·형제 순서를 저장함.
        /// </summary>
        public void Capture(Transform targetTransform)
        {
            target = targetTransform;
            parent = targetTransform.parent;
            siblingIndex = targetTransform.GetSiblingIndex();
            captured = true;
        }

        /// <summary>
        /// 저장된 대상의 부모와 형제 순서를 원래 값으로 복구함.
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

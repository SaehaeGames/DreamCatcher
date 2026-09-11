using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class InteractiveDragObj : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    // 플레이어 데이터
    private PlayerDataManager curPlayerData;   //플레이어 데이터 정보
    private int curScene;
    public bool objectDraged;
    [SerializeField] private GameObject[] targets;
    private Transform[] startParents;
    private int numberOfTargets;
    private bool isTargetConfigured;

    // Start is called before the first frame update
    void Start()
    {
        // 변수 초기화
        objectDraged = false;
    }


    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"OnBeginDrag 호출. isTargetConfigured: {isTargetConfigured}");

        objectDraged = false; // 초기화

        // Tutorial 오브젝트들이 활성화 되어 있지 않으면 코드 비활성화
        if (!isTargetConfigured)
        {
            Debug.LogWarning("startParent가 null이라 스크립트 비활성화됨");
            this.GetComponent<InteractiveDragObj>().enabled = false;
        }
        else
        {
            this.GetComponent<InteractiveDragObj>().enabled = true;
        }
    }

    public void SetTargetParent(Transform arrowTransform)
    {
        if (arrowTransform == null || targets == null)
        {
            Debug.LogError("[InteractiveDragObj] 드래그 타겟 설정값이 없습니다.");
            isTargetConfigured = false;
            return;
        }

        Debug.Log($"[SetTargetParent 호출] arrowTransform: {arrowTransform.name}, targets.Length: {targets.Length}");

        numberOfTargets = targets.Length;
        startParents = new Transform[numberOfTargets];
        for (int i = 0; i < numberOfTargets; i++)
        {
            if (targets[i] == null) continue;

            startParents[i] = targets[i].transform.parent;
            targets[i].transform.SetParent(arrowTransform);
        }
        isTargetConfigured = numberOfTargets > 0;
    }

    public void SetObjctDraged(bool dragSet)
    {
        objectDraged = dragSet;
    }

    public bool GetObjectDraged()
    {
        return objectDraged;
    }

    public void RestoreTargets()
    {
        if (targets == null || startParents == null) return;

        int restoreCount = Mathf.Min(targets.Length, startParents.Length);
        for (int i = 0; i < restoreCount; i++)
        {
            if (targets[i] != null && startParents[i] != null)
            {
                targets[i].transform.SetParent(startParents[i]);
            }
        }

    }

    public void ClearTargetConfiguration()
    {
        RestoreTargets();
        isTargetConfigured = false;
    }

    // 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"드래그 종료. 현재 위치: {eventData.pointerCurrentRaycast.gameObject?.name}");
        numberOfTargets = targets != null ? targets.Length : 0;
        RestoreTargets();

        FeedDrag feedDrag = GetComponent<FeedDrag>();
        if (feedDrag != null && feedDrag.LastRackNumber >= 0)
        {
            objectDraged = true;
            return;
        }

        GameObject droppedObject = eventData.pointerCurrentRaycast.gameObject;
        for (int i = 0; i < numberOfTargets; i++)
        {
            if (IsTargetOrChild(droppedObject, targets[i]))
            {
                objectDraged = true;
                break;
            }
        }

        if (numberOfTargets == 0)
        {
            Debug.LogError("타겟이 존재하지 않습니다.");
        }
    }

    private bool IsTargetOrChild(GameObject droppedObject, GameObject target)
    {
        if (droppedObject == null || target == null) return false;

        Transform droppedTransform = droppedObject.transform;
        Transform targetTransform = target.transform;
        return droppedTransform == targetTransform
            || droppedTransform.IsChildOf(targetTransform)
            || targetTransform.IsChildOf(droppedTransform);
    }
}

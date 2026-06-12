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
    private Transform startParent;
    private int numberOfTargets;

    // Start is called before the first frame update
    void Start()
    {
        // 변수 초기화
        objectDraged = false;
    }


    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"OnBeginDrag 호출. startParent: {startParent?.name}");

        objectDraged = false; // 초기화

        // Tutorial 오브젝트들이 활성화 되어 있지 않으면 코드 비활성화
        if (startParent == null)
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
        Debug.Log($"[SetTargetParent 호출] arrowTransform: {arrowTransform?.name}, targets.Length: {targets.Length}");

        numberOfTargets = targets.Length;
        for (int i = 0; i < numberOfTargets; i++)
        {
            startParent = targets[i].transform.parent;
            targets[i].transform.SetParent(arrowTransform);
        }
    }

    public void SetObjctDraged(bool dragSet)
    {
        objectDraged = dragSet;
    }

    public bool GetObjectDraged()
    {
        return objectDraged;
    }

    // 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {

        Debug.Log($"드래그 종료. 놓은 위치: {eventData.pointerCurrentRaycast.gameObject?.name}");
        Debug.Log($"targets[0]: {targets[0]?.name}");

        numberOfTargets = targets.Length;
        for (int i=0; i < numberOfTargets; i++) 
        {
            targets[i].transform.SetParent(startParent);
            Debug.Log("<color=red>드래그 타겟 제자리로 돌아감 :  "+startParent+"</color>");
        }
        Debug.Log("<color=red>" + eventData.pointerCurrentRaycast.gameObject + "</color>");

        // 올바른 곳에 드래그 되었다면
        switch(numberOfTargets)
        {
            case 0:
                Debug.LogError("타겟이 없습니다.");
                break;
            case 1:
                if (eventData.pointerCurrentRaycast.gameObject == targets[0])
                {
                    objectDraged = true; // 드래그 완료 표시
                }
                break;
            case 2:
                if (eventData.pointerCurrentRaycast.gameObject == targets[0] || eventData.pointerCurrentRaycast.gameObject == targets[1])
                {
                    objectDraged = true; // 드래그 완료 표시
                }
                break;
            case 3:
                if (eventData.pointerCurrentRaycast.gameObject == targets[0] || eventData.pointerCurrentRaycast.gameObject == targets[1] || eventData.pointerCurrentRaycast.gameObject == targets[2])
                {
                    objectDraged = true; // 드래그 완료 표시
                }
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class TutorialDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool objectDraged;
    [SerializeField] private GameObject[] targets;
    private Transform startParent;
    private int numberOfTargets;

    // Start is called before the first frame update
    void Start()
    {
        numberOfTargets = targets.Length;
        Debug.Log("드래그 컴포넌트");
        objectDraged = false;
    }


    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        objectDraged = false;
        Debug.Log("드래그 시작");
        for(int i = 0; i < numberOfTargets; i++)
        {
            startParent = targets[i].transform.parent;
            targets[i].transform.SetParent(GameObject.FindGameObjectWithTag("UI Canvas").transform);
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

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 중");
    }

    // 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        for(int i=0; i < numberOfTargets; i++) 
        {
            targets[i].transform.SetParent(startParent);
        }

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
                    Debug.Log("드래그 성공");
                }
                break;
            case 2:
                if (eventData.pointerCurrentRaycast.gameObject == targets[0] || eventData.pointerCurrentRaycast.gameObject == targets[1])
                {
                    objectDraged = true; // 드래그 완료 표시
                    Debug.Log("드래그 성공");
                }
                break;
            case 3:
                if (eventData.pointerCurrentRaycast.gameObject == targets[0] || eventData.pointerCurrentRaycast.gameObject == targets[1] || eventData.pointerCurrentRaycast.gameObject == targets[2])
                {
                    objectDraged = true; // 드래그 완료 표시
                    Debug.Log("드래그 성공");
                }
                break;
        }
    }
}

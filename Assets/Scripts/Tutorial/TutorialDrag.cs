using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool objectDraged;
    [SerializeField] private GameObject target;

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        objectDraged = false;
        Debug.Log("드래그 시작");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 중");
    }

    // 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 끝");
        // 올바른 곳에 드래그 되었다면
        if (eventData.pointerCurrentRaycast.gameObject==target)
        {
            objectDraged = true; // 드래그 완료 표시
            Debug.Log("드래그 성공");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("드래그 컴포넌트");
        objectDraged = false;
    }
}

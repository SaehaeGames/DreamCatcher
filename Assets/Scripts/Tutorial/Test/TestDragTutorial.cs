using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestDragTutorial : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Vector2 defaultPosition;  // 드롭하면 다시 보낼 원위치 변수    

    public void OnBeginDrag(PointerEventData eventData)
    {
        //드래그를 시작할 때의 함수
        defaultPosition = this.transform.position;  // 처음 위치 저장
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중일 때의 함수

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 5f;
        gameObject.transform.position = mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 끝났을 때의 함수
        this.transform.position = defaultPosition;      // 원위치로 돌아가기
    }
}

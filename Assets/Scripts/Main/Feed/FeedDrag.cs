using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FeedDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // 드래그할 먹이 오브젝트에 넣을 클래스
    // 자신의 먹이 정보와 횃대 충돌 정보를 가짐

    [Header("[Feed Drag]")]
    [SerializeField] private int feedNumber;    // 자신의 먹이 고유 번호 변수
    [SerializeField] private bool isDragging;    // 먹이를 드래그 하고 있는지 여부 변수
    [SerializeField] private int selectRackNumber;    // 드래그한 횃대 번호 변수
    private Vector2 defaultPosition;  // 드롭하면 다시 보낼 원위치 변수

    public void Start()
    {
        isDragging = false;   //드래그중 여부 변수 초기화
    }

    public int FeedNumber
    {
        // 먹이 번호 프로퍼티 함수
        // 먹이 번호를 설정하거나 반환함

        get { return feedNumber; }
        set { feedNumber = value; }
    }

    public bool IsDragging
    {
        // 드래그중인지 프로퍼티 함수
        // 드래그중 여부를 설정하거나 반환함

        get { return isDragging; }
        set { isDragging = value; }
    }

    public int SelectRackNumber
    {
        // 드래그한 횃대 번호 프로퍼티 함수
        // 드래그한 횃대 번호를 설정하거나 반환함

        get { return selectRackNumber; }
        set { selectRackNumber = value; }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //드래그를 시작할 때의 함수

        this.isDragging = true;    // 드래그중으로 상태 변경
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

        this.isDragging = false;    // 드래그중 아님으로 상태 변경
        this.transform.position = defaultPosition;      // 원위치로 돌아가기
    }
}

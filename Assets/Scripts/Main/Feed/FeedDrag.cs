using UnityEngine;
using UnityEngine.EventSystems;

public class FeedDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // 횃대에 놓을 먹이를 드래그하는 스크립트

    [SerializeField] private FeedType feed;              // 자신의 먹이 종류
    [SerializeField] private int lastRackNumber;         // 마지막으로 닿은 횃대 번호
    private bool isDragging = false;                     // 먹이를 드래그 하고 있는지 여부
    private Vector2 defaultPosition;                     // 드롭하면 다시 보낼 원위치
    private RectTransform rectTransform;                 // RectTransform 캐시
    private Canvas canvas;                               // 부모 Canvas 참조

    public FeedType Feed
    {
        get => feed;
        set => feed = value;
    }

    public bool IsDragging
    {
        get => isDragging;
        set => isDragging = value;
    }

    public int LastRackNumber
    {
        get => lastRackNumber;
        set => lastRackNumber = value;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        defaultPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //드래그를 시작할 때의 함수

        isDragging = true;                                   // 드래그중으로 상태 변경
        defaultPosition = rectTransform.anchoredPosition;    // 처음 위치 저장
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중일 때의 함수

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 끝났을 때의 함수

        isDragging = false;                                  // 드래그중 아님으로 상태 변경
        rectTransform.anchoredPosition = defaultPosition;    // 원위치로 돌아가기
    }
}

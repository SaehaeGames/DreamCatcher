using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 튜토리얼 드래그 입력을 단독으로 확인하기 위한 테스트 스크립트입니다.
/// 드래그 중 포인터를 따라가고 종료 시 원래 위치로 돌아갑니다.
/// </summary>
public class TestDragTutorial : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private const float TestDepth = 5f;

    private Camera mainCamera;
    private Vector2 defaultPosition;

    /// <summary>
    /// 드래그 중 반복 검색하지 않도록 테스트에 사용할 Main Camera를 캐시합니다.
    /// </summary>
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// 드래그가 끝난 뒤 복구할 수 있도록 시작 위치를 저장합니다.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        defaultPosition = transform.position;
    }

    /// <summary>
    /// 현재 포인터 화면 좌표를 월드 좌표로 변환하여 테스트 오브젝트를 이동합니다.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // 1. 카메라가 준비되지 않았다면 씬 변경 가능성을 고려해 한 번 더 찾습니다.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }

        // 2. 이벤트가 전달한 포인터 위치를 월드 좌표로 바꾸고 테스트 깊이를 적용합니다.
        Vector3 pointerPosition = eventData.position;
        pointerPosition.z = TestDepth;
        transform.position = mainCamera.ScreenToWorldPoint(pointerPosition);
    }

    /// <summary>
    /// 드래그 테스트가 끝나면 오브젝트를 저장한 시작 위치로 복구합니다.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = defaultPosition;
    }
}
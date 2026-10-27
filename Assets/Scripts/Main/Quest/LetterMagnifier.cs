using UnityEngine;
using UnityEngine.EventSystems;

public class LetterMagnifier : MonoBehaviour, IDragHandler
{
    [Header("UI References")]
    [SerializeField] private RectTransform letterPanel;
    [SerializeField] private RectTransform magnifier;
    [SerializeField] private RectTransform translationText;

    private Vector2 initialMagnifierPosition;
    private Vector2 initialTranslationPosition;

    private void Awake()
    {
        // 초기 위치 저장
        initialMagnifierPosition = magnifier.anchoredPosition;
        initialTranslationPosition = translationText.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 화면 좌표를 편지 패널의 로컬 좌표로 변환
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                letterPanel,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
        {
            return;
        }

        // 초기 위치를 기준으로 돋보기 이동량 계산
        Vector2 delta = localPoint - initialMagnifierPosition;

        // 돋보기 이동
        magnifier.anchoredPosition = localPoint;

        // 번역 텍스트는 반대 방향으로 보정
        translationText.anchoredPosition =
            initialTranslationPosition - delta;
    }
}
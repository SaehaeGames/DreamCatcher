using UnityEngine;

/// <summary>인테리어 패널의 열기·닫기와 재열기 시 표시 갱신을 담당합니다.</summary>
public class InteriorManager : MonoBehaviour
{
    [SerializeField] private GameObject interiorPanel;
    [SerializeField] private InteriorCategory category;

    private void OnEnable()
    {
        // 최초 OnEnable은 GameManager.Awake보다 빠를 수 있으므로 데이터가 준비된 경우에만 갱신합니다.
        // 메뉴 초기화가 이전에 실패했어도 재열기 시 다시 검증할 수 있으며, 최초 Main 외형은 MainProducts가 따로 준비합니다.
        if (category != null && GameManager.instance != null && GameManager.instance.IsInitialized)
            category.RefreshPanel();
    }

    /// <summary>패널을 열면 OnEnable에서 표시를 갱신하고 기본 탭과 스크롤 상단으로 돌아갑니다.</summary>
    public void OpenInteriorPanel()
    {
        if (interiorPanel != null) interiorPanel.SetActive(true);
    }

    /// <summary>패널을 닫습니다. 저장된 외형은 유지하고 탭과 스크롤은 다음에 열 때 초기화합니다.</summary>
    public void CloseInteriorPanel()
    {
        if (interiorPanel != null) interiorPanel.SetActive(false);
    }
}

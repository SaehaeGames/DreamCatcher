using UnityEngine;

/// <summary>인테리어 패널의 열기·닫기와 재열기 시 표시 갱신을 담당합니다.</summary>
public class InteriorManager : MonoBehaviour
{
    [SerializeField] private GameObject interiorPanel;
    [SerializeField] private InteriorCategory category;

    private void OnEnable()
    {
        // 최초 씬 활성화는 데이터 준비보다 빠를 수 있습니다.
        // 최초 외형은 InteriorCategory.Start에서 적용하고, 이후 패널 재열기만 여기서 갱신합니다.
        if (category != null && category.IsInitialized) category.RefreshPanel();
    }

    /// <summary>패널을 열면 OnEnable에서 표시와 선택 탭의 스크롤 위치를 갱신합니다.</summary>
    public void OpenInteriorPanel()
    {
        if (interiorPanel != null) interiorPanel.SetActive(true);
    }

    /// <summary>패널을 닫되 선택한 탭과 저장된 외형은 유지합니다.</summary>
    public void CloseInteriorPanel()
    {
        if (interiorPanel != null) interiorPanel.SetActive(false);
    }
}

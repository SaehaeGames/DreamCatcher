using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Inspector에 연결된 인테리어 버튼 이벤트를 담당 객체로 전달합니다.
/// 탭은 번호를, 상품은 고정 ID를 사용하며 별도의 런타임 클릭 리스너를 추가하지 않습니다.
/// </summary>
public class InteriorButton : MonoBehaviour
{
    public int buttonNumber;
    public string itemID;

    private Button buttonComponent;
    private InteriorCategory interiorCategory;

    /// <summary>비활성 탭에도 담당 객체를 전달하고 Button 참조를 한 번 캐싱합니다.</summary>
    public void Initialize(InteriorCategory owner)
    {
        interiorCategory = owner;
        if (buttonComponent == null) buttonComponent = GetComponent<Button>();
    }

    /// <summary>씬의 카테고리 OnClick이 호출하는 진입점입니다.</summary>
    public void SelectInteriorButton()
    {
        if (interiorCategory != null) interiorCategory.UpdateCategoryPanel(buttonNumber);
        else Debug.LogWarning("[InteriorButton] 카테고리 담당 객체가 연결되지 않았습니다.", this);
    }

    /// <summary>씬의 상품 OnClick이 호출하는 진입점이며 번호 대신 상품 ID로 선택합니다.</summary>
    public void SelectInteriorItemButton()
    {
        if (interiorCategory != null) interiorCategory.SelectInteriorItem(itemID);
        else Debug.LogWarning("[InteriorButton] 인테리어 담당 객체가 연결되지 않았습니다.", this);
    }

    /// <summary>데이터에서 확인한 보유 상태로 클릭 가능 여부만 갱신합니다.</summary>
    public void SetInteractable(bool available)
    {
        if (buttonComponent != null) buttonComponent.interactable = available;
    }
}

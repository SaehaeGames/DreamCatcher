using UnityEngine;
using UnityEngine.UI;

/// <summary>상품 버튼의 ID와 담당 객체를 보관하고 클릭 시 선택 처리만 요청합니다.</summary>
public class BuyButtonInfo : MonoBehaviour
{
    [SerializeField] private string selectGoddsId; // 기존 Inspector의 저장 이름을 유지합니다.
    private Button buyButton;
    private BuyCheck buyCheck;

    private void Awake()
    {
        // 비활성 상품은 초기화 시점보다 늦게 Awake가 실행될 수 있으므로 캐시가 있으면 재사용합니다.
        if (buyButton == null) buyButton = GetComponent<Button>();
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    /// <summary>초기화에서 등록한 클릭 리스너만 제거합니다.</summary>
    private void OnDestroy()
    {
        if (buyButton != null) buyButton.onClick.RemoveListener(OnBuyButtonClick);
    }

    /// <summary>상품 번호나 탭 상태에 의존하지 않고 현재 연결된 상품 ID의 구매 선택을 요청합니다.</summary>
    public void OnBuyButtonClick()
    {
        if (buyCheck != null && !string.IsNullOrEmpty(selectGoddsId))
            buyCheck.SelectBuyingGoods(selectGoddsId);
    }

    /// <summary>상품 화면을 준비할 때 구매 담당 객체와 상품 ID를 한 번 연결합니다.</summary>
    public void BindProduct(BuyCheck owner, string id)
    {
        buyCheck = owner;
        selectGoddsId = id;
        if (buyButton == null) buyButton = GetComponent<Button>();
    }

    /// <summary>업그레이드 후 다음 상품 ID와 구매 가능 상태만 갱신하며 담당 객체는 유지합니다.</summary>
    public void UpdateProduct(string id, bool available)
    {
        selectGoddsId = id;
        if (buyButton == null) buyButton = GetComponent<Button>();
        if (buyButton != null)
            buyButton.interactable = available && buyCheck != null && !string.IsNullOrEmpty(selectGoddsId);
    }
}

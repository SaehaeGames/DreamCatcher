using UnityEngine;
using UnityEngine.UI;

/// <summary>상품 버튼의 ID와 담당 객체를 보관하고 클릭 시 선택 처리만 요청합니다.</summary>
public class BuyButtonInfo : MonoBehaviour
{
    [SerializeField] private int selectGoodsNumber;
    [SerializeField] private string selectGoddsId; // 기존 Inspector의 저장 이름을 유지합니다.
    private Button buyButton;
    private BuyCheck buyCheck;

    private void Awake()
    {
        buyButton = GetComponent<Button>();
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    private void OnDestroy()
    {
        if (buyButton != null) buyButton.onClick.RemoveListener(OnBuyButtonClick);
    }

    public void OnBuyButtonClick()
    {
        if (buyCheck != null && !string.IsNullOrEmpty(selectGoddsId))
            buyCheck.SelectBuyingGoods(selectGoodsNumber, selectGoddsId);
    }

    public void Initialize(BuyCheck owner)
    {
        buyCheck = owner;
        if (buyButton == null) buyButton = GetComponent<Button>();
    }

    public void SetAvailable(bool available)
    {
        if (buyButton != null) buyButton.interactable = available && !string.IsNullOrEmpty(selectGoddsId);
    }

    public void SetSelectGoodsNumber(int number) => selectGoodsNumber = number;
    public void SetSelectGoodsId(string id) => selectGoddsId = id;
    public int GetSelectGoodsNumber() => selectGoodsNumber;
    public string GetSelectGoodsId() => selectGoddsId;
}

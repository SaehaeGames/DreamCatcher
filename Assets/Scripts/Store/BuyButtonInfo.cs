using UnityEngine;
using UnityEngine.UI;

public class BuyButtonInfo : MonoBehaviour
{
    //상품 구매 버튼 정보 클래스

    [SerializeField] private int selectGoodsNumber;    // 선택한 상품 번호
    [SerializeField] private string selectGoddsId;
    private Button buyButton;
    private BuyCheck buyCheck;

    private void Awake()
    {
        buyButton = GetComponent<Button>();
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClick);
        }
        else
        {
            Debug.LogWarning("[WARNING] 버튼 컴포넌트가 없음");
        }
    }

    public void OnBuyButtonClick()
    {
        // 상품 구매 버튼을 눌렀을 때 동작

        if (buyCheck == null)
        {
            GameObject storeManager = GameObject.FindGameObjectWithTag("StoreManager");
            if (storeManager != null) buyCheck = storeManager.GetComponent<BuyCheck>();
        }

        if (buyCheck != null)
        {
            buyCheck.SelectBuyingGoods(selectGoodsNumber, selectGoddsId);
        }
        else
        {
            Debug.LogWarning("[WARNING] BuyCheck 컴포넌트를 찾을 수 없음");
        }
    }

    public void Initialize(BuyCheck owner)
    {
        buyCheck = owner;
    }

    public void SetSelectGoodsNumber(int number)
    {
        selectGoodsNumber = number;
    }

    public void SetSelectGoodsId(string id)
    {
        selectGoddsId = id; // 상품 카테고리 설정
    }

    public int GetSelectGoodsNumber()
    {
        return selectGoodsNumber;
    }

    public string GetSelectGoodsId()
    {
        return selectGoddsId; // ✅ 카테고리 반환 함수 추가
    }
}

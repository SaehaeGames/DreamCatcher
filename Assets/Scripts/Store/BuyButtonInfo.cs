using UnityEngine;
using UnityEngine.UI;

public class BuyButtonInfo : MonoBehaviour
{
    //상품 구매 버튼 정보 클래스

    [SerializeField] private int selectGoodsNumber;    // 선택한 상품 번호
    private StoreItemCategory goodsCategory;  // ✅ 상품 고유 카테고리 추가
    private Button buyButton;

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

        GameObject storeManager = GameObject.FindGameObjectWithTag("StoreManager");
        if (storeManager != null)
        {
            var buyCheck = storeManager.GetComponent<BuyCheck>();
            if (buyCheck != null)
            {
                buyCheck.SelectBuyingGoods(selectGoodsNumber, goodsCategory);  // ✅ 상품 번호와 카테고리 전달
            }
            else
            {
                Debug.LogWarning("[WARNING] BuyCheck 컴포넌트를 찾을 수 없음");
            }
        }
        else
        {
            Debug.LogWarning("[WARNING] StoreManager 게임 오브젝트를 찾을 수 없음");
        }
    }

    public void SetSelectGoodsNumber(int number)
    {
        selectGoodsNumber = number;
    }

    public void SetSelectGoodsCategory(StoreItemCategory category)
    {
        goodsCategory = category; // ✅ 상품 카테고리 설정
    }

    public int GetSelectGoodsNumber()
    {
        return selectGoodsNumber;
    }

    public StoreItemCategory GetSelectGoodsCategory()
    {
        return goodsCategory; // ✅ 카테고리 반환 함수 추가
    }
}

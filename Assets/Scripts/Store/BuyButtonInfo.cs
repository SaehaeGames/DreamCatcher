using UnityEngine;
using UnityEngine.UI;

public class BuyButtonInfo : MonoBehaviour
{
    //상품 구매 버튼 정보 클래스

    [SerializeField] private int selectGoodsNumber;    //선택한 상품 번호
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
            Debug.LogWarning("버튼 컴포넌트가 없음");
        }
    }

    public void OnBuyButtonClick()
    {
        //상품 구매 버튼을 누르면(상품 구매 골드를 누르면 == 이 버튼을 누르면)

        GameObject storeManager = GameObject.FindGameObjectWithTag("StoreManager");
        if (storeManager != null)
        {
            var buyCheck = storeManager.GetComponent<BuyCheck>();
            if (buyCheck != null)
            {
                buyCheck.SelectBuyingGoods(selectGoodsNumber);
            }
            else
            {
                Debug.LogWarning("버튼 컴포넌트가 없음");
            }
        }
        else
        {
            Debug.LogWarning("StoreManager 게임 오브젝트를 찾을 수 없음");
        }
    }

    public void SetSelectGoodsNumber(int number)
    {
        selectGoodsNumber = number;
    }

    public int GetSelectGoodsNumber()
    {
        return selectGoodsNumber;
    }
}

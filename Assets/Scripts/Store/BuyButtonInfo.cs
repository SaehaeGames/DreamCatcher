using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButtonInfo : MonoBehaviour
{
    //상품 구매 버튼 정보 클래스
    [Header("[Buy Button Information]")]
    [SerializeField] private int SelectGoodsNumber;    //선택한 상품 번호

    private void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(TouchBuyButton); //버튼 이벤트 추가
    }

    public void TouchBuyButton()
    {
        //상품 구매 버튼을 누르면(상품 구매 골드를 누르면 == 이 버튼을 누르면)

        GameObject.FindGameObjectWithTag("StoreManager").GetComponent<BuyCheck>().SelectBuyingGoods(SelectGoodsNumber); //자신의 버튼 번호를 전달함
    }

    public void SetSelectGoodsNumber(int num)
    {
        this.SelectGoodsNumber = num;
    }

    public int GetSelectGoodsNumber()
    {
        return this.SelectGoodsNumber;
    }
}

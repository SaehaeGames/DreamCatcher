using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyCheck : MonoBehaviour
{
    [Header("[Buy Check]")]
    [SerializeField] private int selectGoods;  //구매하려고 선택한 상품 번호
    public GameObject[] ButtonObj;  //상품 구매 오브젝트 배열(순서대로 횃대, 꽃병, 상자, 실, 벽지, 특제먹이, 벽지 순)
    public GameObject[] StarButtonObj;  //별 가구 구매
    public GameObject[] SeaButtonObj;  //바다 가구 구매

    [Space]
    public GameObject panel_buyCheck;   //상품 구매 확인 팝업
    public GameObject panel_AdjustCheck;   //상품 적용 확인 팝업

    private void Start()
    {
        int numberOfGoods = ButtonObj.Length;   //상품 총 개수
        for (int i = 0; i < numberOfGoods; i++)
        {
            ButtonObj[i].gameObject.GetComponent<BuyButtonInfo>().SetSelectGoodsNumber(i);    //상품 구매 버튼 오브젝트에 고유 번호 지정
        }
        for (int i = 0; i < 9; i++)
        {
            StarButtonObj[i].gameObject.GetComponent<BuyButtonInfo>().SetSelectGoodsNumber(i);    //상품 구매 버튼 오브젝트에 고유 번호 지정
            SeaButtonObj[i].gameObject.GetComponent<BuyButtonInfo>().SetSelectGoodsNumber(i);    //상품 구매 버튼 오브젝트에 고유 번호 지정
        }
    }

    public void SelectBuyingGoods(int buttonNumber)
    {
        //상품 구매 버튼을 눌러서 버튼 인덱스를 받은 함수

        selectGoods = buttonNumber; //선택한 상품 번호 갱신
        SetActiveBuyCheckPanel();   //상품 구매 확인 팝업 활성화
    }

    public void SelectYesButton()
    {
        //상품 구매 확인 패널에서 확인 버튼(구매 버튼)을 눌러서 구매하는 함수

        this.gameObject.GetComponent<StoreData>().BuyGoods(selectGoods);   //선택한 상품 구매
        SetInActiveBuyCheckPanel(); //상품 구매 확인 팝업 비활성화


        //이부분 수정하기
        StoreType curCategoryIdx = this.GetComponent<CategorySelect>().GetSelectedCategory();
        if (curCategoryIdx == StoreType.Interior)    //인테리어 구매라면
        {
            SetActiveAdjustCheckPanel();
        }
    }

    /*public void SelectYesButton_Adjust()
    {
        //상품 적용 확인 패널에서 확인 버튼을 눌러서 가구를 적용하는 함수

        switch (selectGoods)
        {
            case 0 :
                GameManager.instance.wallpaperIndex = 1;
                break;
            case 1:
                GameManager.instance.garlandIndex = 1;
                break;
            case 2:
                GameManager.instance.fillpenIndex = 1;
                break;
            case 3:
                GameManager.instance.telescopeIndex = 1;
                break;
            case 4:
                GameManager.instance.startdropIndex = 1;
                break;
            case 5:
                GameManager.instance.crystalballIndex = 1;
                break;
            case 6:
                GameManager.instance.musicboxIndex = 1;
                break;
            case 7:
                GameManager.instance.mapIndex = 1;
                break;
            //책상은 보류
            default:
                break;

        }

        SetInActiveAdjustCheckPanel();
    }*/

    public void SetActiveBuyCheckPanel()
    {
        //상품 구매 확인 패널을 활성화하는 함수

        panel_buyCheck.gameObject.SetActive(true);
    }

    public void SetInActiveBuyCheckPanel()
    {
        //상품 구매 확인 패널을 비활성화하는 함수

        panel_buyCheck.gameObject.SetActive(false);
    }

    public void SetActiveAdjustCheckPanel()
    {
        //상품 적용 확인 패널을 활성화하는 함수

        panel_AdjustCheck.gameObject.SetActive(true);
    }

    public void SetInActiveAdjustCheckPanel()
    {
        //상품 적용 확인 패널을 비활성화하는 함수

        panel_AdjustCheck.gameObject.SetActive(false);
    }
}

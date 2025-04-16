using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyCheck : MonoBehaviour
{
    [Header("[Buy Check]")]
    [SerializeField] private int selectGoods;  //구매하려고 선택한 상품 번호
    [SerializeField] private int selectId;

    public GameObject[] ButtonObj;  //상품 구매 오브젝트 배열(순서대로 횃대, 꽃병, 상자, 실, 벽지, 특제먹이, 벽지 순)
    public GameObject[] StarButtonObj;  //별 가구 구매
    public GameObject[] SeaButtonObj;  //바다 가구 구매

    [Space]
    public GameObject panel_buyCheck;   //상품 구매 확인 팝업
    public GameObject panel_AdjustCheck;   //상품 적용 확인 팝업

    private void Start()
    {
        List<int> defaultList = GameManager.instance.storeinfo_data.GetSortedIDsByTheme(ItemTheme.Default);
        List<int> StarList = GameManager.instance.storeinfo_data.GetSortedIDsByTheme(ItemTheme.Star);
        List<int> SeaList = GameManager.instance.storeinfo_data.GetSortedIDsByTheme(ItemTheme.Sea);

        int numberOfGoods = ButtonObj.Length;   //상품 총 개수
        for (int i = 0; i < numberOfGoods; i++)
        {
            BuyButtonInfo buttonInfo = ButtonObj[i].GetComponent<BuyButtonInfo>();
            buttonInfo.SetSelectGoodsNumber(i); //상품 고유 번호 설정
            buttonInfo.SetSelectGoodsId(defaultList[i]); //상품 ID 설정
        }
        for (int i = 0; i < 9; i++)
        {
            StarButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsNumber(i);
            StarButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsId(StarList[i]);

            SeaButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsNumber(i);
            SeaButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsId(SeaList[i]);
        }
    }

    public void SelectBuyingGoods(int buttonNumber, int id)
    {
        // 상품 구매 버튼을 눌러서 버튼 인덱스와 상품 id를 받는 함수
        selectGoods = buttonNumber; // 선택한 상품 번호 갱신
        selectId = id;
        SetActiveBuyCheckPanel();   // 상품 구매 확인 팝업 활성화
    }

    public void SelectYesButton()
    {
        // 상품 구매 확인 패널에서 확인 버튼(구매 버튼)을 눌러서 구매하는 함수
        this.gameObject.GetComponent<StoreData>().BuyGoods(selectId);
        SetInActiveBuyCheckPanel(); // 상품 구매 확인 팝업 비활성화

        // 인테리어 카테고리일 경우 적용 확인 패널 띄우기
        StoreType curCategoryIdx = this.GetComponent<CategorySelect>().GetSelectedCategory();
        if (curCategoryIdx == StoreType.Interior)
        {
            SetActiveAdjustCheckPanel();
        }
    }

    public void SelectYesButton_Adjust()
    {
        //상품 적용 확인 패널에서 확인 버튼을 눌러서 가구를 적용하는 함수

        JsonManager jsonManager = GameManager.instance.jsonManager;
        InteriorData item = GameManager.instance.interiorDataManager.GetInteriorData(selectId);

        item.isAdjusting = true;
        jsonManager.SaveData(Constants.InteriorDataFile, GameManager.instance.interiorDataManager);

        SetInActiveAdjustCheckPanel();
    }

    public void SelectNoButton_Adjust()
    {
        //상품 적용 확인 패널에서 취소 버튼을 눌러서 가구를 적용하지 않고 구매만 하는 함수

        SetInActiveAdjustCheckPanel();
    }


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

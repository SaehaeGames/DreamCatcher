using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuyCheck : MonoBehaviour
{
    [Header("[Buy Check]")]
    [SerializeField] private int selectGoods;  //구매하려고 선택한 상품 번호
    [SerializeField] private string selectId;

    public GameObject[] ButtonObj;  //상품 구매 오브젝트 배열(순서대로 횃대, 꽃병, 상자, 실, 벽지, 특제먹이, 벽지 순)
    public GameObject[] StarButtonObj;  //별 가구 구매
    public GameObject[] SeaButtonObj;  //바다 가구 구매

    [Space]
    public GameObject panel_buyCheck;   //상품 구매 확인 팝업
    public GameObject panel_AdjustCheck;   //상품 적용 확인 팝업

    private string[] developCategory = { Constants.GoodsData_Rack, Constants.GoodsData_Vase, Constants.GoodsData_Box, Constants.GoodsData_Thread };

    private void Start()
    {
        // 보조도구 버튼: 현재 레벨+1 기반으로 ID 설정
        for (int i = 0; i < ButtonObj.Length && i < developCategory.Length; i++)
        {
            BuyButtonInfo buttonInfo = ButtonObj[i].GetComponent<BuyButtonInfo>();
            buttonInfo.SetSelectGoodsNumber(i);

            GoodsData goodsData = GameManager.instance.goodsDataManager.GetGoodsDataByCategory(developCategory[i]);
            int level = goodsData != null ? goodsData.level : 0;
            string nextId = GameManager.instance.storeinfo_data.GetIDByCategoryAndLevel(developCategory[i], level + 1);
            if (!string.IsNullOrEmpty(nextId))
                buttonInfo.SetSelectGoodsId(nextId);
        }

        // 인테리어 버튼 ID 설정
        List<string> StarList = GameManager.instance.storeinfo_data.GetSortedIDsByTheme(ItemTheme.Star);
        List<string> SeaList = GameManager.instance.storeinfo_data.GetSortedIDsByTheme(ItemTheme.Sea);

        for (int i = 0; i < 9; i++)
        {
            StarButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsNumber(i);
            StarButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsId(StarList[i]);

            SeaButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsNumber(i);
            SeaButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsId(SeaList[i]);
        }
    }

    public void SetDevelopmentButtonId(int index, string id)
    {
        if (index >= 0 && index < ButtonObj.Length)
            ButtonObj[index].GetComponent<BuyButtonInfo>().SetSelectGoodsId(id);
    }

    public void SelectBuyingGoods(int buttonNumber, string id)
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

        var interiorDataManager = GameManager.instance.interiorDataManager;
        var storeInfoData = GameManager.instance.storeinfo_data;

        // 같은 카테고리 기존 적용 아이템 초기화
        StoreItemCategory category = storeInfoData.GetCategoryByItemID(selectId);
        foreach (var d in interiorDataManager.dataList)
        {
            var si = storeInfoData.dataList.FirstOrDefault(s => s.id == d.storeinfo_id);
            if (si != null && si.category == category)
                d.isAdjusting = false;
        }

        // 선택 아이템 적용
        InteriorData item = interiorDataManager.GetInteriorDataByStoreInfoId(selectId);
        item.isAdjusting = true;

        GameManager.instance.jsonManager.SaveData(Constants.InteriorDataFile, interiorDataManager);

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

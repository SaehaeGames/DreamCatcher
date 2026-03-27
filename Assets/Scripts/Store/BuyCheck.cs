using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuyCheck : MonoBehaviour
{
    [Header("[Buy Check]")]
    [SerializeField] private int selectGoods;   // 현재 선택된 상품 버튼 번호
    [SerializeField] private string selectId;   // 현재 선택된 상품 ID

    public GameObject[] ButtonObj;       // 보조도구 구매 버튼 오브젝트 배열 (Rack, Vase, Box, Thread 순)
    public GameObject[] StarButtonObj;   // Star 인테리어 구매 버튼 오브젝트 배열
    public GameObject[] SeaButtonObj;    // Sea 인테리어 구매 버튼 오브젝트 배열

    [Space]
    public GameObject panel_buyCheck;     // "구매하시겠습니까?" 확인 팝업
    public GameObject panel_AdjustCheck; // "지금 배치하시겠습니까?" 확인 팝업 (인테리어 구매 후)

    private string[] developCategory = { Constants.GoodsData_Rack, Constants.GoodsData_Vase, Constants.GoodsData_Box, Constants.GoodsData_Thread };

    /// <summary>
    /// 각 구매 버튼에 buttonNumber와 storeinfo ID를 초기 할당
    /// - 보조도구: 현재 레벨 + 1에 해당하는 다음 구매 가능 아이템 ID를 설정
    /// - Star/Sea 인테리어: storeinfo를 테마별로 정렬된 순서로 ID를 할당
    /// </summary>
    private void Start()
    {
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

        List<string> starList = GameManager.instance.storeinfo_data.GetSortedIDsByTheme(ItemTheme.Star);
        List<string> seaList = GameManager.instance.storeinfo_data.GetSortedIDsByTheme(ItemTheme.Sea);

        // 버튼 수와 storeinfo 아이템 수 중 작은 쪽에 맞춰 할당 (배열 초과 방지)
        int starCount = Mathf.Min(StarButtonObj.Length, starList.Count);
        int seaCount = Mathf.Min(SeaButtonObj.Length, seaList.Count);

        for (int i = 0; i < starCount; i++)
        {
            StarButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsNumber(i);
            StarButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsId(starList[i]);
        }

        for (int i = 0; i < seaCount; i++)
        {
            SeaButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsNumber(i);
            SeaButtonObj[i].GetComponent<BuyButtonInfo>().SetSelectGoodsId(seaList[i]);
        }
    }

    /// <summary>
    /// 보조도구 버튼의 구매 대상 ID를 외부(StoreData)에서 갱신할 때 호출
    /// 구매 완료 후 다음 레벨 아이템으로 버튼 ID를 교체할 때 사용
    /// </summary>
    public void SetDevelopmentButtonId(int index, string id)
    {
        if (index >= 0 && index < ButtonObj.Length)
            ButtonObj[index].GetComponent<BuyButtonInfo>().SetSelectGoodsId(id);
    }

    /// <summary>
    /// 구매 버튼을 눌렀을 때 호출
    /// 선택된 상품 정보를 저장하고 구매 확인 팝업을 표시
    /// </summary>
    public void SelectBuyingGoods(int buttonNumber, string id)
    {
        selectGoods = buttonNumber;
        selectId = id;
        panel_buyCheck.SetActive(true);
    }

    /// <summary>
    /// 구매 확인 팝업에서 "예"를 눌렀을 때 호출
    /// 실제 구매 처리(BuyGoods)를 실행하고, 인테리어 카테고리라면 배치 확인 팝업을 추가로 표시
    /// </summary>
    public void SelectYesButton()
    {
        GetComponent<StoreData>().BuyGoods(selectId);
        panel_buyCheck.SetActive(false);

        if (GetComponent<CategorySelect>().GetSelectedCategory() == StoreType.Interior)
            panel_AdjustCheck.SetActive(true);
    }

    /// <summary>
    /// 배치 확인 팝업에서 "지금 배치"를 눌렀을 때 호출
    /// 같은 카테고리의 기존 배치 아이템을 isAdjusting = false로 초기화한 뒤,
    /// 방금 구매한 아이템만 isAdjusting = true로 설정하고 JSON 저장
    /// </summary>
    public void SelectYesButton_Adjust()
    {
        var interiorDataManager = GameManager.instance.interiorDataManager;
        var storeInfoData = GameManager.instance.storeinfo_data;

        StoreItemCategory category = storeInfoData.GetCategoryByItemID(selectId);
        // 같은 카테고리의 기존 적용 아이템을 모두 해제
        foreach (var d in interiorDataManager.dataList)
        {
            var si = storeInfoData.dataList.FirstOrDefault(s => s.id == d.storeinfo_id);
            if (si != null && si.category == category)
                d.isAdjusting = false;
        }

        // 새로 구매한 아이템을 적용 중으로 설정
        InteriorData item = interiorDataManager.GetInteriorDataByStoreInfoId(selectId);
        item.isAdjusting = true;

        GameManager.instance.jsonManager.SaveData(Constants.InteriorDataFile, interiorDataManager);
        panel_AdjustCheck.SetActive(false);
    }

    /// <summary>
    /// 배치 확인 팝업에서 "나중에"를 눌렀을 때 팝업을 닫음
    /// </summary>
    public void SelectNoButton_Adjust()
    {
        panel_AdjustCheck.SetActive(false);
    }

    public void SetActiveBuyCheckPanel() => panel_buyCheck.SetActive(true);
    public void SetInActiveBuyCheckPanel() => panel_buyCheck.SetActive(false);
    public void SetActiveAdjustCheckPanel() => panel_AdjustCheck.SetActive(true);
    public void SetInActiveAdjustCheckPanel() => panel_AdjustCheck.SetActive(false);
}

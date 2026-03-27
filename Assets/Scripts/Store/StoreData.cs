using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoreData : MonoBehaviour
{
    [Header("[Store Product]")]
    public GameObject[] goodsContents;       // 보조도구 상품 UI 오브젝트 배열 (Rack, Vase, Box, Thread 순)
    public GameObject[] interiorContetns;    // 인테리어 카테고리별 컨테이너 (0: Sea, 1: Star)
    public SpriteArray[] goodsImages;        // 보조도구별 레벨에 따른 스프라이트 배열

    private GoodsDataManager goodsDataManager;
    public StoreInfo_Data storeinfo_data;

    [Space]
    public GameObject[] soldOut;             // 보조도구 품절 오버레이 (최대 레벨 도달 시 활성화)
    public GameObject[] StarSoldOut;         // Star 인테리어 품절 오버레이 (이미 보유 시 활성화)
    public GameObject[] SeaSoldOut;          // Sea 인테리어 품절 오버레이 (이미 보유 시 활성화)

    private ItemTheme curInteriorCategory;   // 현재 선택된 인테리어 테마 (Sea / Star)

    public string[] developCategory = { Constants.GoodsData_Rack, Constants.GoodsData_Vase, Constants.GoodsData_Box, Constants.GoodsData_Thread };

    private Text[] effectTexts;      // 보조도구 효과 설명 텍스트 컴포넌트 캐시
    private Text[] goldTexts;        // 보조도구 가격 텍스트 컴포넌트 캐시
    private Image[] productImages;   // 보조도구 상품 이미지 컴포넌트 캐시

    /// <summary>
    /// 데이터 매니저 참조를 초기화하고, 각 상품 UI 컴포넌트(이미지, 가격, 효과 텍스트)를 캐싱한 뒤
    /// 보조도구 탭의 초기 UI를 갱신
    /// </summary>
    public void Start()
    {
        goodsDataManager = GameManager.instance.goodsDataManager;
        storeinfo_data = GameManager.instance.storeinfo_data;

        effectTexts = new Text[goodsContents.Length];
        goldTexts = new Text[goodsContents.Length];
        productImages = new Image[goodsContents.Length];

        // 각 상품 UI의 자식 계층에서 Image / Text 컴포넌트를 인덱스로 가져와 캐싱
        for (int i = 0; i < goodsContents.Length; i++)
        {
            Transform t = goodsContents[i].transform;
            productImages[i] = t.GetChild(1).GetComponent<Image>();
            goldTexts[i] = t.GetChild(6).GetChild(2).GetComponent<Text>();

            // 효과 텍스트 오브젝트가 비활성화 상태라면 null로 처리 (Rack은 효과 텍스트 없음)
            var effectObj = t.GetChild(4);
            effectTexts[i] = effectObj.gameObject.activeSelf ? effectObj.GetComponent<Text>() : null;
        }

        UpdateDevelopmentGoodsData();
    }

    /// <summary>
    /// 외부(CategorySelect)에서 인테리어 탭의 테마를 전환할 때 호출됨
    /// 현재 테마를 저장하고 인테리어 UI를 갱신
    /// </summary>
    public void UpdateStoreInteriorData(ItemTheme theme)
    {
        curInteriorCategory = theme;
        UpdateInteriorGoodsData();
    }

    /// <summary>
    /// 보조도구(Rack/Vase/Box/Thread) 탭 UI를 갱신
    /// 각 카테고리의 현재 레벨을 조회해 다음 구매 가능한 아이템 ID를 storeinfo에서 찾고,
    /// 최대 레벨이면 SoldOut 오버레이를 표시하고, 아니면 이미지/가격/효과 텍스트를 업데이트
    /// </summary>
    private void UpdateDevelopmentGoodsData()
    {
        BuyCheck buyCheck = GetComponent<BuyCheck>();

        for (int i = 0; i < goodsContents.Length; i++)
        {
            GoodsData goodsData = goodsDataManager.GetGoodsDataByCategory(developCategory[i]);
            int goodsLevel = goodsData != null ? goodsData.level : 0;
            // 현재 레벨 + 1에 해당하는 다음 구매 아이템 ID를 조회
            string id = storeinfo_data.GetIDByCategoryAndLevel(developCategory[i], goodsLevel + 1);

            // ID가 없으면 최대 레벨 → 품절 표시 후 건너뜀
            soldOut[i].SetActive(string.IsNullOrEmpty(id));
            if (string.IsNullOrEmpty(id)) continue;

            buyCheck?.SetDevelopmentButtonId(i, id);
            productImages[i].sprite = goodsImages[i].imageList[goodsLevel + 1];
            if (effectTexts[i] != null)
                effectTexts[i].text = storeinfo_data.GetEffectByID(id);
            goldTexts[i].text = storeinfo_data.GetGoldByID(id).ToString();
        }
    }

    /// <summary>
    /// 인테리어(Sea / Star) 탭 UI를 갱신한
    /// 현재 테마에 해당하는 storeinfo 아이템을 순서대로 순회하며
    /// 설명/가격 텍스트를 채우고, 이미 보유 중인 아이템은 SoldOut 오버레이를 표시하고
    /// 구매 버튼을 비활성화
    /// </summary>
    private void UpdateInteriorGoodsData()
    {
        BuyCheck buyCheck = GetComponent<BuyCheck>();
        bool isSea = curInteriorCategory == ItemTheme.Sea;
        // 테마에 따라 사용할 SoldOut 배열과 버튼 배열을 선택
        GameObject[] soldOutArr = isSea ? SeaSoldOut : StarSoldOut;
        GameObject[] buttonArr = isSea ? buyCheck.SeaButtonObj : buyCheck.StarButtonObj;
        int interiorIndex = isSea ? 1 : 0;

        int index = 0;
        foreach (var item in storeinfo_data.dataList.Where(x => x.theme == curInteriorCategory))
        {
            // UI 텍스트 갱신 (설명, 가격)
            if (index < interiorContetns[interiorIndex].transform.childCount)
            {
                var child = interiorContetns[interiorIndex].transform.GetChild(index);
                child.GetChild(3).GetComponent<Text>().text = item.contents.Replace("nn", "\n");
                child.GetChild(4).GetChild(2).GetComponent<Text>().text = item.gold.ToString();
            }

            // 보유 여부 확인 → 보유 중이면 SoldOut 활성화 + 구매 버튼 비활성화
            InteriorData interiorItem = GameManager.instance.interiorDataManager.GetInteriorDataByStoreInfoId(item.id);
            bool isHaving = interiorItem != null && interiorItem.isHaving;

            if (index < soldOutArr.Length)
                soldOutArr[index].SetActive(isHaving);
            if (index < buttonArr.Length)
                buttonArr[index].GetComponent<Button>().interactable = !isHaving;

            index++;
        }
    }

    /// <summary>
    /// PlayerData에서 골드를 차감함
    /// </summary>
    public void SpendGold(int cost)
    {
        GameManager.instance.playerDataManager.GetPlayerDataByDataName(Constants.PlayerData_Gold).dataNumber -= cost;
    }

    /// <summary>
    /// 상품 구매를 처리함
    /// 1) 골드 차감
    /// 2) 현재 카테고리(Development/Interior)에 따라 분기
    ///    - Development: 해당 GoodsData의 level을 1 올리고 JSON 저장, 관련 InteriorData도 isHaving = true
    ///    - Interior: InteriorData의 isHaving = true로 설정하고 JSON 저장
    /// 3) UI 갱신 및 상단 바 골드 텍스트 업데이트
    /// </summary>
    public void BuyGoods(string id)
    {
        JsonManager jsonManager = GameManager.instance.jsonManager;
        StoreType curCategory = GetComponent<CategorySelect>().GetSelectedCategory();

        SpendGold(storeinfo_data.GetGoldByID(id));

        if (curCategory == StoreType.Development)
        {
            StoreItemCategory itemCategory = storeinfo_data.GetCategoryByItemID(id);

            // Rack은 두 개의 GoodsData(왼쪽/오른쪽)가 함께 레벨업되어야 함
            if (itemCategory == StoreItemCategory.Rack)
            {
                List<GoodsData> rackList = GameManager.instance.goodsDataManager.GetGoodsDataList("Rack");
                if (rackList != null && rackList.Count == 2)
                {
                    rackList[0].level++;
                    rackList[1].level++;
                    jsonManager.SaveData(Constants.GoodsDataFile, GameManager.instance.goodsDataManager);
                }
            }
            else
            {
                GoodsData item = GameManager.instance.goodsDataManager.GetGoodsDataByCategory(itemCategory.ToString());
                if (item != null)
                {
                    item.level++;
                    jsonManager.SaveData(Constants.GoodsDataFile, GameManager.instance.goodsDataManager);
                }
                else
                {
                    Debug.LogError($"[ERROR] {itemCategory}에 해당하는 GoodsData를 찾을 수 없음");
                }
            }

            // 보조도구 구매 시 연관된 인테리어 아이템 보유 처리 + 자동 적용
            var interiorDataManager = GameManager.instance.interiorDataManager;
            // 같은 카테고리의 기존 적용 아이템 해제
            foreach (var d in interiorDataManager.dataList)
            {
                var si = storeinfo_data.dataList.FirstOrDefault(s => s.id == d.storeinfo_id);
                if (si != null && si.category == itemCategory && si.theme == ItemTheme.Default)
                    d.isAdjusting = false;
            }
            InteriorData interiorItem = interiorDataManager.GetInteriorDataByStoreInfoId(id);
            if (interiorItem != null)
            {
                interiorItem.isHaving = true;
                interiorItem.isAdjusting = true;
            }
            jsonManager.SaveData(Constants.InteriorDataFile, interiorDataManager);

            UpdateDevelopmentGoodsData();
        }
        else if (curCategory == StoreType.Interior)
        {
            InteriorData item = GameManager.instance.interiorDataManager.GetInteriorDataByStoreInfoId(id);
            if (item != null)
            {
                item.isHaving = true;
                jsonManager.SaveData(Constants.InteriorDataFile, GameManager.instance.interiorDataManager);
            }

            UpdateInteriorGoodsData();
        }

        // 상단 바의 골드 표시 업데이트
        GameObject.FindGameObjectWithTag(Constants.Tag_TopBar).GetComponent<TopBarText>().UpdateText();
    }

    [System.Serializable]
    public class SpriteArray
    {
        public Sprite[] imageList;
    }
}

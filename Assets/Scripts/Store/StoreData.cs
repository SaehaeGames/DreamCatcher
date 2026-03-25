using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class StoreData : MonoBehaviour
{
    //상점
    //파일의 데이터를 가져오는 클래스

    [Header("[Store Product]")]
    public GameObject[] goodsContents;   //보조 도구 상품 배열
    public GameObject[] interiorContetns;   //인테리어 상품 배열
    public SpriteArray[] goodsImages; //상품 이미지 배열  

    private GoodsDataManager goodsDataManager;   //상품 정보

    public StoreInfo_Data storeinfo_data;

    [Space]
    public GameObject[] soldOut;  //판매 완료 이미지 오브젝트 배열
    public GameObject[] StarSoldOut;
    public GameObject[] SeaSoldOut;

    public string curCategory;
    public string curInteriorCategory;

    public string[] developCategory = { Constants.GoodsData_Rack, Constants.GoodsData_Vase, Constants.GoodsData_Box, Constants.GoodsData_Thread};


    private Text[] contentsTexts;
    private Text[] effectTexts;
    private Text[] goldTexts;
    private Image[] productImages;

    public void Start()
    {
        goodsDataManager = GameManager.instance.goodsDataManager;
        storeinfo_data = GameManager.instance.storeinfo_data;

        contentsTexts = new Text[goodsContents.Length];
        effectTexts = new Text[goodsContents.Length];
        goldTexts = new Text[goodsContents.Length];
        productImages = new Image[goodsContents.Length];

        for (int i = 0; i < goodsContents.Length; i++)
        {
            Transform t = goodsContents[i].transform;

            productImages[i] = t.GetChild(1).GetComponent<Image>();       // Image_Product
            goldTexts[i] = t.GetChild(6).GetChild(2).GetComponent<Text>(); // Cost

            // Text_EffectNumber (Thread는 없으니 null 허용)
            var effectObj = t.GetChild(4);
            effectTexts[i] = effectObj.gameObject.activeSelf
                ? effectObj.GetComponent<Text>()
                : null;
        }

        UpdateStoreData(StoreType.Development);

        Debug.Log("=== goodsDataManager.dataList 순서 ===");
        for (int i = 0; i < goodsDataManager.dataList.Count; i++)
        {
            Debug.Log($"[{i}] id: {goodsDataManager.dataList[i].id}, category: {goodsDataManager.dataList[i].category}, level: {goodsDataManager.dataList[i].level}");
        }
    }

    public void UpdateStoreData(StoreType category)
    {
        //상점 데이터를 가져오는 함수

        UpdateDevelopmentGoodsData();
    }

    public void UpdateStoreInteriorData(ItemTheme theme)
    {
        Debug.Log("인테리어 업데이트 함수 호출");
        curInteriorCategory = theme.ToString();
        UpdateInteriorGoodsData(curInteriorCategory);
    }


    private void UpdateDevelopmentGoodsData()
    {
        BuyCheck buyCheck = this.GetComponent<BuyCheck>();

        for (int i = 0; i < goodsContents.Length; i++)
        {
            GoodsData goodsData = goodsDataManager.GetGoodsDataByCategory(developCategory[i]);
            int goodsLevel = goodsData != null ? goodsData.level : 0;
            string id = storeinfo_data.GetIDByCategoryAndLevel(developCategory[i], goodsLevel + 1);

            soldOut[i].SetActive(false);

            if (string.IsNullOrEmpty(id))
            {
                soldOut[i].SetActive(true);
                continue;
            }

            buyCheck?.SetDevelopmentButtonId(i, id);

            productImages[i].sprite = goodsImages[i].imageList[goodsLevel + 1];
            if (effectTexts[i] != null)
                effectTexts[i].text = storeinfo_data.GetEffectByID(id);
            goldTexts[i].text = storeinfo_data.GetGoldByID(id).ToString();
        }
    }

    private void UpdateInteriorGoodsData(string curCategory)
    {
        // 인테리어 상품을 업데이트 하는 함수

        int index = 0;
        int interiorIndex = 0;
        Debug.Log(curInteriorCategory);
        if (curInteriorCategory == ItemTheme.Sea.ToString())
        {
            interiorIndex = 1;
        }
        else
        {
            interiorIndex = 0;
        }

        for (int i = 0; i < storeinfo_data.dataList.Count; i++)
        {
            var item = storeinfo_data.dataList[i];

            if (item.theme.ToString() == curInteriorCategory)
            {
                // UI 업데이트
                interiorContetns[interiorIndex].transform.GetChild(index).GetChild(3).GetComponent<Text>().text = item.contents.Replace("nn", "\n");
                interiorContetns[interiorIndex].transform.GetChild(index).GetChild(4).GetChild(2).GetComponent<Text>().text = item.gold.ToString();

                index++;
            }
        }

        // 보유중인 아이템일 경우 soldOut 표시 + 버튼 비활성화
        BuyCheck buyCheck = this.GetComponent<BuyCheck>();
        bool isSea = curInteriorCategory == ItemTheme.Sea.ToString();
        GameObject[] soldOutArr = isSea ? SeaSoldOut : StarSoldOut;
        GameObject[] buttonArr = isSea ? buyCheck.SeaButtonObj : buyCheck.StarButtonObj;

        index = 0;
        for (int i = 0; i < storeinfo_data.dataList.Count; i++)
        {
            var item = storeinfo_data.dataList[i];

            if (item.theme.ToString() == curInteriorCategory)
            {
                InteriorData interiorItem = GameManager.instance.interiorDataManager.GetInteriorDataByStoreInfoId(item.id);
                bool isHaving = interiorItem != null && interiorItem.isHaving;

                if (soldOutArr.Length > index)
                    soldOutArr[index].SetActive(isHaving);

                if (buttonArr.Length > index)
                    buttonArr[index].GetComponent<Button>().interactable = !isHaving;

                index++;
            }
        }
    }

    private bool IsItemSoldOut(string category, int level)
    {
        // 각 카테고리별 다음 상품이 있는지 확인하는 함수

        bool result = false;

        switch (category)
        {
            case Constants.GoodsData_Rack :
                if (level == Constants.GoodsData_Rack_MaxLevel)
                    result = true;
                break;
            case Constants.GoodsData_Vase:
                if (level == Constants.GoodsData_Vase_MaxLevel)
                    result = true;
                break;
            case Constants.GoodsData_Box:
                if (level == Constants.GoodsData_Box_MaxLevel)
                    result = true;
                break;
            case Constants.GoodsData_Thread:
                if (level == Constants.GoodsData_Thread_MaxLevel)
                    result = true;
                break;
            default: result = false; break;

        }

        return result;
    }


    public void AddGoodsLevel(int goodsIndex)
    {
        goodsDataManager.dataList[goodsIndex].level++;    //상품 레벨 증가
    }

    public void SpendGold(int cost)
    {
        GameManager.instance.playerDataManager.GetPlayerDataByDataName(Constants.PlayerData_Gold).dataNumber -= cost;   //보유 골드 감소
    }

    public void BuyGoods(string id)
    {
        JsonManager jsonManager = GameManager.instance.jsonManager;
        StoreType curCategory = this.GetComponent<CategorySelect>().GetSelectedCategory();

        SpendGold(storeinfo_data.GetGoldByID(id));  // 골드 차감

        if (curCategory == StoreType.Development)   // 보조도구 상품이라면
        {
            StoreItemCategory itemCategory = storeinfo_data.GetCategoryByItemID(id);

            if (itemCategory == StoreItemCategory.Rack)  // 횃대는 두 개 동시에 레벨업
            {
                List<GoodsData> rackList = GameManager.instance.goodsDataManager.GetGoodsDataList("Rack");
                if (rackList != null && rackList.Count == 2)
                {
                    rackList[0].level++;
                    rackList[1].level++;
                    Debug.Log($"[SUCCESS] RackFront & RackBack 레벨업 완료!");
                    jsonManager.SaveData(Constants.GoodsDataFile, GameManager.instance.goodsDataManager);
                }
            }
            else  // 일반 아이템 레벨업
            {
                GoodsData item = GameManager.instance.goodsDataManager.GetGoodsDataByCategory(itemCategory.ToString());
                if (item != null)
                {
                    item.level++;
                    Debug.Log($"[SUCCESS] {itemCategory} 레벨업 완료! 현재 레벨: {item.level}");
                    jsonManager.SaveData(Constants.GoodsDataFile, GameManager.instance.goodsDataManager);
                }
                else
                {
                    Debug.LogError($"[ERROR] {itemCategory}에 해당하는 GoodsData를 찾을 수 없음");
                }
            }

            UpdateStoreData(StoreType.Development);
        }
        else if (curCategory == StoreType.Interior)  //인테리어 상품이라면
        {

            InteriorData item = GameManager.instance.interiorDataManager.GetInteriorDataByStoreInfoId(id);
            Debug.Log("현재 아이템 보유 상태: " + item.isHaving);
            if (item != null)
            {
                item.isHaving = true; //정확한 아이템 보유 상태 업데이트

                jsonManager.SaveData(Constants.InteriorDataFile, GameManager.instance.interiorDataManager);
            }

            UpdateInteriorGoodsData(curInteriorCategory);
        }

        GameObject.FindGameObjectWithTag("TopBar").GetComponent<TopBarText>().UpdateText();

        // UI 즉시 반영
        MainProducts mainProducts = FindObjectOfType<MainProducts>();
        if (mainProducts != null)
        {
            mainProducts.ResetMainProducts();
        }
    }

    public List<GoodsData> GetGoodsDataListByIndex(int goodsNumber)
    {
        string category = GameManager.instance.storeinfo_data.dataList[goodsNumber].category.ToString();
        return GameManager.instance.goodsDataManager.GetGoodsDataList(category);
    }

    public int GetCategoryStartNumber(StoreItemCategory category)
    {
        // 각 카테고리별 시작 번호를 반환하는 함수

        int startNumber = -1;   // 초기값으로 -1을 설정

        for (int i = 0; i < storeinfo_data.dataList.Count; i++)
        {
            if (storeinfo_data.dataList[i].category == category)
            {
                startNumber = i; 
                break;             
            }
        }

        return startNumber;
    }

    int GetCostForSpecialGoods(int goodsNumber)
    {
        switch (goodsNumber)
        {
            case 0: return 1000; // Wallpaper
            case 1: return 2000; // Garland
            case 2: return 2000; // Fillpen
            case 3: return 5000; // Telescope
            case 4: return 7000; // Stardrop
            default: return 0;
        }
    }

    [System.Serializable]
    public class SpriteArray //상품 이미지 배열의 행
    {
        //하이어라키 화면에 이차원 배열로 보이기 위해 만든 상품 이미지 클래스

        public Sprite[] imageList;
    }
}
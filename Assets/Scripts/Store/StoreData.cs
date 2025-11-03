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
    
    public void Start()
    {
        goodsDataManager = GameManager.instance.goodsDataManager;
        storeinfo_data = GameManager.instance.storeinfo_data;

        UpdateStoreData(StoreType.Development);
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
        // 보조도구 상품을 업데이트 하는 함수

        /*        for (int i = 0; i < goodsContents.Length; i++)
                {
                    int goodsLevel = goodsDataManager.dataList[i].level;
                    int id = storeinfo_data.GetIDByCategoryAndLevel(developCategory[i], goodsLevel);

                    Debug.Log(developCategory[i]  + goodsLevel + "ID : " + id);

                    goodsContents[i].transform.GetChild(1).GetComponent<Image>().sprite = goodsImages[i].imageList[goodsLevel + 1];
                    goodsContents[i].transform.GetChild(3).GetChild(0).GetComponent<Text>().text = storeinfo_data.GetContentsByID(id + 1);
                    goodsContents[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = storeinfo_data.GetEffectByID(id + 1);
                    goodsContents[i].transform.GetChild(6).GetChild(2).GetComponent<Text>().text = storeinfo_data.GetGoldByID(id + 1).ToString();

                    if (IsItemSoldOut(developCategory[i], goodsLevel))
                        soldOut[i].SetActive(true);
                }*/

        for (int i = 0; i < goodsContents.Length; i++)
        {
            int goodsLevel = goodsDataManager.dataList[i].level;
            int dataOffset = goodsLevel + i + 1;


            goodsContents[i].transform.GetChild(1).GetComponent<Image>().sprite = goodsImages[i].imageList[goodsLevel + 1];
            goodsContents[i].transform.GetChild(3).GetComponent<Text>().text = storeinfo_data.dataList[dataOffset].contents;
            goodsContents[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = storeinfo_data.dataList[dataOffset].effect;
            goodsContents[i].transform.GetChild(6).GetChild(2).GetComponent<Text>().text = storeinfo_data.dataList[dataOffset].gold.ToString();
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

        // 보유중인 아이템일 경우 soldOut 표시
        index = 0;
        for (int i = 0; i < storeinfo_data.dataList.Count; i++)
        {
            var item = storeinfo_data.dataList[i];

            if (item.theme.ToString() == curInteriorCategory)
            {
                InteriorData interiorItem = GameManager.instance.interiorDataManager.GetInteriorData(item.id);

                if (interiorItem != null && interiorItem.isHaving)
                {
                    if (curInteriorCategory == ItemTheme.Sea.ToString())
                    {
                        if (SeaSoldOut.Length > index)
                            SeaSoldOut[index].SetActive(true);
                    }
                    else
                    {
                        if (StarSoldOut.Length > index)
                            StarSoldOut[index].SetActive(true);
                    }
                }

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
        GameManager.instance.playerDataManager.GetPlayerData(Constants.PlayerData_Gold).dataNumber -= cost;   //보유 골드 감소
    }

    public void BuyGoods(string id)
    {
        JsonManager jsonManager = GameManager.instance.jsonManager;
        StoreType curCategory = this.GetComponent<CategorySelect>().GetSelectedCategory();

        if (curCategory == StoreType.Development)   // 보조도구 상품이라면
        {
            int idNum = int.Parse(id.Split('_')[1]);
            if (idNum >= 4000 && idNum <= 4002) // 횃대는 두 개 동시에 레벨업
            {
                List<GoodsData> rackList = GameManager.instance.goodsDataManager.GetGoodsDataList(curCategory.ToString());
                if (rackList != null && rackList.Count == 2)
                {
                    rackList[0].level++;
                    rackList[1].level++;
                    Debug.Log($"[SUCCESS] RackFront & RackBack 레벨업 완료!");

                    jsonManager.SaveData(Constants.GoodsDataFile, GameManager.instance.goodsDataManager);
                }
            }
            else  //일반 아이템 레벨업
            {
                GoodsData item = GameManager.instance.goodsDataManager.GetGoodsDataByCategory(curCategory.ToString());
                if (item != null)
                {
                    item.level++;

                    jsonManager.SaveData(Constants.GoodsDataFile, GameManager.instance.goodsDataManager);
                }
            }

            UpdateStoreData(StoreType.Development);
        }
        else if (curCategory == StoreType.Interior)  //인테리어 상품이라면
        {

            InteriorData item = GameManager.instance.interiorDataManager.GetInteriorData(id);
            Debug.Log("현재 아이템 보유 상태: " + item.isHaving);
            if (item != null)
            {
                item.isHaving = true; //정확한 아이템 보유 상태 업데이트

                jsonManager.SaveData(Constants.InteriorDataFile, GameManager.instance.interiorDataManager);
            }

            UpdateStoreData(StoreType.Interior);
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

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
    
    public void Start()
    {
        goodsDataManager = GameManager.instance.goodsDataManager;
        storeinfo_data = GameManager.instance.storeinfo_data;

        //curCategory = this.GetComponent<StoreItemCategory>()
        UpdateStoreData(StoreType.Development);
    }

    public void UpdateStoreData(StoreType category)
    {
        //상점 데이터를 가져오는 함수

        /*        for (int i = 0; i < storeinfo_data.dataList.Count; i++)
                {
                    string category = storeinfo_data.dataList[i].category.ToString();

                    if (!string.IsNullOrEmpty(category))
                    {
                        UpdateGoodsInfo(i, category);
                    }
                }*/

        UpdateDevelopmentGoodsData();
        //UpdateInteriorGoodsData(curCategory);
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

        for (int i = 0; i < goodsContents.Length; i++)
        {
            int goodsLevel = goodsDataManager.dataList[i].level;
            int dataOffset = goodsLevel + i + 1;

            Debug.Log("goodsLevel : " + i + ", " + goodsLevel + ", " + dataOffset);

            goodsContents[i].transform.GetChild(1).GetComponent<Image>().sprite = goodsImages[i].imageList[goodsLevel + 1];
            goodsContents[i].transform.GetChild(3).GetChild(0).GetComponent<Text>().text = storeinfo_data.dataList[dataOffset].contents;
            goodsContents[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = storeinfo_data.dataList[dataOffset].effect;
            goodsContents[i].transform.GetChild(6).GetChild(2).GetComponent<Text>().text = storeinfo_data.dataList[dataOffset].gold.ToString();

            /*if (IsItemSoldOut(i))
                soldOut[i].SetActive(true);*/
        }
    }

    private void UpdateInteriorGoodsData(string curCategory)
    {
        // 인테리어 상품을 업데이트 하는 함수

        //this.GetComponent<CategorySelect>().SetStarInteriorSelect();

        int index = 0;
        int interiorIndex = 0;
        Debug.Log(curInteriorCategory);
        if (curInteriorCategory == ItemTheme.Sea.ToString())
        {
            interiorIndex = 1;
        }

        for (int i = 0; i < storeinfo_data.dataList.Count; i++)
        {
            var item = storeinfo_data.dataList[i];

            if (item.theme.ToString() == curInteriorCategory)
            {
                // UI 업데이트
                interiorContetns[interiorIndex].transform.GetChild(index).GetChild(3).GetComponent<Text>().text = item.contents.Replace("nn", "\n");
                interiorContetns[interiorIndex].transform.GetChild(index).GetChild(4).GetChild(2).GetComponent<Text>().text = item.gold.ToString();

                // 품절 여부 확인 (Optional)
                /*
                if (IsItemSoldOut(i))
                {
                    soldOut[index].SetActive(true);
                }
                */

                index++;
            }
        }
    }

    private bool IsItemSoldOut(int index)
    {
        // 각 카테고리별 다음 상품이 있는지 확인하는 함수

        int nextItemIndex = index + 1;
        return nextItemIndex < storeinfo_data.dataList.Count && storeinfo_data.dataList[nextItemIndex].category == storeinfo_data.dataList[index].category;
    }

    /*
        public void AddGoodsLevel(int goodsIndex)
        {
            curGoodsData.goodsList[goodsIndex].goodsLevel++;    //상품 레벨 증가
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GoodsJSON>().DataSaveText(curGoodsData);   //변경사항 json으로 저장
        }

        public void SpendGold(int cost)
        {
            curPlayerData.dataList[1].dataNumber -= cost;   //보유 골드 감소
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
        }
    */
    public void BuyGoods(int goodsNumber)
    {
        StoreType curCategory = this.GetComponent<CategorySelect>().GetSelectedCategory();

        if (curCategory == StoreType.Development)   // 보조도구 상품이라면
        {
            int goodsLevel = goodsDataManager.dataList[goodsNumber].level;    // 플레이어의 상품 레벨
            int dataOffset = goodsLevel + goodsNumber + 1;
            int goodsCost = storeinfo_data.dataList[dataOffset].gold;

            if (GameManager.instance.playerDataManager.GetPlayerData(Constants.PlayerData_Gold).dataNumber >= goodsCost)    // 구매 가능하다면(돈이 충분하다면)
            {
                Debug.Log(GameManager.instance.playerDataManager.GetPlayerData(Constants.PlayerData_Gold).dataNumber);
                GameManager.instance.playerDataManager.GetPlayerData(Constants.PlayerData_Gold).dataNumber -= goodsCost;    // 골드 감소
                Debug.Log(GameManager.instance.playerDataManager.GetPlayerData(Constants.PlayerData_Gold).dataNumber);

                GameManager.instance.jsonManager.SaveData(Constants.PlayerDataFile, GameManager.instance.playerDataManager);  // 변동된 플레이어 데이터 저장

                goodsDataManager.dataList[goodsNumber].level++;
                //GameManager.instance.GetComponent<GoodsJSON>().DataSaveText(curGoodsData);
                UpdateStoreData(StoreType.Development);
            }
        }
        else if (curCategory == StoreType.Interior)  // 인테리어 상품이라면
        {
            int goodsCost = GetCostForSpecialGoods(goodsNumber);

            if (GameManager.instance.playerDataManager.GetPlayerData(Constants.PlayerData_Gold).dataNumber >= goodsCost)
            {
                GameManager.instance.playerDataManager.GetPlayerData(Constants.PlayerData_Gold).dataNumber -= goodsCost;
                GameManager.instance.jsonManager.SaveData(Constants.PlayerDataFile, GameManager.instance.playerDataManager);  // 변동된 플레이어 데이터 저장

                //GameManager.instance.GetComponent<GoodsJSON>().DataSaveText(curGoodsData);
                UpdateStoreData(StoreType.Interior);
            }
        }

        GameObject.FindGameObjectWithTag("TopBar").GetComponent<TopBarText>().UpdateText();
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
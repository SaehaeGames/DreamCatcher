using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreData : MonoBehaviour
{
    //상점
    //파일의 데이터를 가져오는 클래스

    [Header("[Store Product]")]
    public GameObject[] goodsContents;   //상품 UI 배열(횃대, 화분, 상자, 실)
    public SpriteArray[] goodsImages; //상품 이미지 배열   //* 각 상품이 각각의 스크립트로 이미지를 다 가지고 있고, 거기서 바꾸는 걸로 변경하기 (상품 프리팹 만들기)
    //보조도구, 인벤토리 모두 이 방식대로로 변경하기.. 상점 코드 뜯어고치기..

    private GoodsContainer curGoodsData;   //상품 정보(각 상품별 레벨이 어느정도인지.. 이거 헷갈리니까 정의서에 적기)
    private PlayerDataContainer curPlayerData;   //플레이어 데이터 정보(돈이랑 꿈구슬, 특제먹이 몇 개 가지고 있는지.. 이것도 정의서에 적기)

    [Space]
    public GameObject[] soldOut;  //판매 완료 이미지 오브젝트 배열
    public GameObject[] StarSoldOut;
    public GameObject[] SeaSoldOut;


    public StoreInfo_Data _storeinfo_data;

    public void Start()
    {
        UpdateStoreData();
    }

    public void UpdateStoreData()
    {
        curGoodsData = GameManager.instance.loadGoodsData;   //플레이어의 상품 정보를 가져옴
/*
        //상점 데이터를 가져오는 함수
        //int goodCnt = 0;    //상품 인덱스(횃대: 0, 꽃병: 1, 박스: 2, 실: 3)
        string leftCategoty = "";  //확인한 카테고리
        int cnt = _storeinfo_data.endId - _storeinfo_data.startId + 1;
        int checkCnt = 0;
        for (int goodCnt = 0; goodCnt < cnt; goodCnt++)
        {
            string curCategory = _storeinfo_data.dataList[goodCnt].category;  //현재 카테고리
            if (curCategory.Equals(leftCategoty))   //만약 이미 확인했던 카테고리라면
            {
                continue;   //넘어감
            }
            else    //처음 확인하는 카테고리라면
            {
                leftCategoty = _storeinfo_data.dataList[goodCnt].category;  //현재 카테고리를 확인한 카테고리로 설정

                //상품 정보 가져옴
                int goodsLevel = curGoodsData.goodsList[checkCnt].goodsLevel;  //플레이어의 상품 레벨 데이터
                goodsContents[checkCnt].transform.GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = _storeinfo_data.datalist[goodCnt + goodsLevel + 1].effect;   //상품 효과 불러옴
                goodsContents[checkCnt].transform.GetChild(6).GetChild(2).gameObject.GetComponent<Text>().text = _storeinfo_data.datalist[goodCnt + goodsLevel + 1].gold.ToString();     //상품 가격 불러옴
                goodsContents[checkCnt].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = goodsImages[checkCnt].imageList[goodsLevel + 1]; //상품 이미지 불러옴

                checkCnt++;
                //goodCnt++;
            }
        }

        //만약 상품 레벨이 최고 레벨이라면 해당 상품 sold out 표시(**소프트코딩으로 바꿀 수 있을지 고민..)
        if (curGoodsData.goodsList[0].goodsLevel == 2)    //횃대 마지막 레벨을 구매했다면
        {
            goodsContents[0].transform.GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = _storeinfo_data.datalist[2].effect;   //상품 효과 불러옴
            soldOut[0].SetActive(true);   //구매 막기
        }
        if (curGoodsData.goodsList[1].goodsLevel == 3)    //꽃병 마지막 레벨을 구매했다면
        {
            goodsContents[1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = _storeinfo_data.datalist[6].effect;   //상품 효과 불러옴
            soldOut[1].SetActive(true);   //구매 막기
        }
        if (curGoodsData.goodsList[2].goodsLevel == 3)    //상자 마지막 레벨을 구매했다면
        {
            goodsContents[2].transform.GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = _storeinfo_data.datalist[10].effect;   //상품 효과 불러옴
            soldOut[2].SetActive(true);   //구매 막기
        }
        if (curGoodsData.goodsList[3].goodsLevel == 4)    //실 마지막 레벨을 구매했다면
        {
            goodsContents[3].transform.GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = _storeinfo_data.datalist[15].effect;   //상품 효과 불러옴
            //**실 구매시 마지막 번호라 오류 뜬 것 해결하기
            soldOut[3].SetActive(true);   //구매 막기
        }
*/
        //변동사항 저장
        GameManager.instance.loadGoodsData = curGoodsData;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GoodsJSON>().DataSaveText(curGoodsData);
    }

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

    public void BuyGoods(int goodsNumber)
    {
        //상품 구매 함수

        curGoodsData = GameManager.instance.loadGoodsData;   //플레이어의 상품 정보를 가져옴
        curPlayerData = GameManager.instance.loadPlayerData;

        int goodsLevel = 0;
        int goodsCost;

        int curCategoty = this.GetComponent<CategorySelect>().GetSelectedCategory();

        if (curCategoty == 0)
        {
            goodsLevel = curGoodsData.goodsList[goodsNumber].goodsLevel;  //플레이어의 상품 레벨

            //나중에 카테고리 이름 인덱스 가져와서 시작하는 걸로 바꾸기
            switch (goodsNumber)
            {
                case 0:
                    goodsCost = _storeinfo_data.dataList[goodsNumber + goodsLevel + 1].gold;
                    break;
                case 1:
                    goodsCost = _storeinfo_data.dataList[goodsNumber + goodsLevel + 3].gold;
                    break;
                case 2:
                    goodsCost = _storeinfo_data.dataList[goodsNumber + goodsLevel + 6].gold;
                    break;
                case 3:
                    goodsCost = _storeinfo_data.dataList[goodsNumber + goodsLevel + 9].gold;
                    break;
                default:
                    //goodsCost = _storeinfo_data.datalist[goodsNumber + goodsLevel + 1].gold;
                    goodsCost = 1000;   //벽지를 위해 임시 설정
                    break;
            }

            if (curPlayerData.dataList[1].dataNumber >= goodsCost)      //플레이어 골드가 구매하려는 상품 금액보다 크다면(임시)
            {
                //상품 구매
                SpendGold(goodsCost);  //보유 골드 감소

                if (goodsNumber < 4)    //구매한 아이템이 가구라면(횃대, 꽃병, 상자, 실)
                {
                    AddGoodsLevel(goodsNumber);   //상품 레벨 증가

                    //만약 해당 상품의 마지막 레벨을 구매했다면(*이부분 어떻게 소프트코딩으로 바꿀 수 있을지 고민..)
                    switch (goodsNumber)
                    {
                        case 0:
                            if (curGoodsData.goodsList[goodsNumber].goodsLevel == 2)    //횃대 마지막 레벨을 구매했다면
                            {
                                soldOut[goodsNumber].SetActive(true);   //구매 막기
                            }
                            break;
                        case 1:
                            if (curGoodsData.goodsList[goodsNumber].goodsLevel == 3)    //꽃병 마지막 레벨을 구매했다면
                            {
                                soldOut[goodsNumber].SetActive(true);   //구매 막기
                            }
                            break;
                        case 2:
                            if (curGoodsData.goodsList[goodsNumber].goodsLevel == 3)    //상자 마지막 레벨을 구매했다면
                            {
                                soldOut[goodsNumber].SetActive(true);   //구매 막기
                            }
                            break;
                        case 3:
                            if (curGoodsData.goodsList[goodsNumber].goodsLevel == 4)    //실 마지막 레벨을 구매했다면
                            {
                                soldOut[goodsNumber].SetActive(true);   //구매 막기
                            }
                            break;
                    }
                }
            }

        } //기본 가구 버튼

        if (curCategoty == 1)   //인테리어 가구라면(현재 별과 바다 통합되어있음)
        {
            if (goodsNumber == 0)
            {
                //만약 구매한 상품이 벽지라면
                //GameManager.instance.wallpaperIndex = 1;
                SpendGold(1000);
                StarSoldOut[goodsNumber].SetActive(true);   //구매 막기
                //GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
            }
            else if (goodsNumber == 1)
            {
                //만약 구매한 상품이 가랜드라면
                //GameManager.instance.garlandIndex = 1;
                SpendGold(2000);
                StarSoldOut[goodsNumber].SetActive(true);   //구매 막기
                //GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
            }
            else if (goodsNumber == 2)
            {
                //만약 구매한 상품이 깃펜이라면
                //GameManager.instance.fillpenIndex = 1;
                SpendGold(2000);
                StarSoldOut[goodsNumber].SetActive(true);   //구매 막기
                //GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
            }
            else if (goodsNumber == 3)
            {
                //만약 구매한 상품이 망원경이라면
                //GameManager.instance.telescopeIndex = 1;
                SpendGold(5000);
                StarSoldOut[goodsNumber].SetActive(true);   //구매 막기
                //GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
            }
            else if (goodsNumber == 4)
            {
                //만약 구매한 상품이 별조각이라면
                //GameManager.instance.startdropIndex = 1;
                SpendGold(7000);
                StarSoldOut[goodsNumber].SetActive(true);   //구매 막기
                //GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
            }
            else if (goodsNumber == 51)
            {
                //만약 구매한 상품이 수정구슬이라면
                //GameManager.instance.crystalballIndex = 1;
                SpendGold(5000);
                StarSoldOut[goodsNumber].SetActive(true);   //구매 막기
                //GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
            }
            else if (goodsNumber == 6)
            {
                //만약 구매한 상품이 오르골이라면
                //GameManager.instance.musicboxIndex = 1;
                SpendGold(7000);
                StarSoldOut[goodsNumber].SetActive(true);   //구매 막기
                //GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
            }
            else if (goodsNumber == 7)
            {
                //만약 구매한 상품이 지도라면
                //GameManager.instance.mapIndex = 1;
                SpendGold(3000);
                StarSoldOut[goodsNumber].SetActive(true);   //구매 막기
                //GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
            }
            else if (goodsNumber == 8)
            {
                //만약 구매한 상품이 창틀이라면
                //GameManager.instance. = 1;
                SpendGold(3000);
                //GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
            }
        }

        if (goodsNumber == 5 && curCategoty == 2)
        {
            //만약 구매한 상품이 특제먹이라면
            SpendGold(2000);
            curPlayerData.dataList[2].dataNumber++; //특제먹이 개수 증가
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장
        }

        GameObject.FindGameObjectWithTag("TopBar").GetComponent<TopBarText>().UpdateText();   //상단바 업데이트
        UpdateStoreData();    //상점 업데이트
        }
    }

[System.Serializable]
public class SpriteArray //상품 이미지 배열의 행
{
    //하이어라키 화면에 이차원 배열로 보이기 위해 만든 상품 이미지 클래스

    public Sprite[] imageList;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedManager : MonoBehaviour
{
    // 먹이 기능 총괄 스크립트

    [Header("[Feed Objects]")]
    public GameObject[] FeedObjects;     // Feed Panel에서 드래그 하는 먹이 프리팹 배열
    public GameObject[] RackFeedObjects;     // 각 레벨별 횃대의 먹이 프리팹 배열
    public GameObject[] RackTriggerObjects; // 횃대 트리거 프리팹 배열
    public GameObject[] RackBirdObjects;    // 횃대에 나타나는 새 오브젝트 배열

    int rackLevel;  // 플레이어 횃대 레벨

    void Start()
    {
        rackLevel = GameManager.instance.loadGoodsData.goodsList[0].goodsLevel; // 현재 플레이어의 횃대 레벨


        for (int i = 0; i < FeedObjects.Length; i++)    // 먹이 종류만큼 반복
        {
            FeedObjects[i].gameObject.GetComponent<FeedDrag>().FeedNumber = i;    // 먹이 오브젝트에 고유 번호 지정
        }

        Debug.Log("먹이 고유 번호 지정함");

        for (int j = 0; j < RackTriggerObjects[rackLevel].gameObject.transform.childCount; j++)     // 현재 레벨의 횃대 트리거 수만큼 반복
        {
            RackTriggerObjects[rackLevel].gameObject.transform.GetChild(j).GetComponent<RackTrigger>().TriggerNumber = j;    // 횃대 트리거 오브젝트에 고유 번호 지정
            RackBirdObjects[rackLevel].gameObject.transform.GetChild(j).GetComponent<BirdTouch>().BirdIdx = j;  // 횃대 새 오브젝트에 고유 인덱스 지정
        }

        UpdateRack();   // 횃대 정보 업데이트

        // 등장한 새 정보 업데이트
        BirdContainer saveBirds = GameManager.instance.loadBirdData;    // 저장된 새 데이터를 가져옴
        for (int i = 0; i < RackBirdObjects[rackLevel].gameObject.transform.childCount; i++)    // 새 개수만큼 반복
        {
            bool isFed = saveBirds.birdList[i].isFed;   // 세이브 파일의 먹이 여부를 가져옴
            if (isFed)  // 먹이가 있다면 (== 먹이 둔 것이 있고, 시간이 남아있다면)
            {
                SetActiveRackFeed(i, saveBirds.birdList[i].feedNumber); // 해당 먹이 활성화
            }

            bool isAppeared = saveBirds.birdList[i].isAppeared;    // 세이브 파일의 등장 여부 가져옴
            if (isAppeared)     // 등장한 새가 있다면 (== 새가 등장하였으나 클릭하지 않은 새가 있다면)
            {
                int birdNumber = saveBirds.birdList[i].birdNumber;  // 해당 새의 고유 번호 가져옴
                ArriveRackBird(i, birdNumber);  // 새 등장시킴
            }
        }
    }

    public void UpdateRack()
    {
        // 횃대 정보 업데이트 함수
        // 현재 횃대 레벨에 맞게 트리거 오브젝트, 먹이 오브젝트 활성화 / 비활성화

        for (int i = 0; i < RackTriggerObjects.Length; i++)     // 횃대 트리거 수만큼 반복
        {
            if (i == rackLevel)     // 현재 횃대 레벨의
            {
                RackFeedObjects[i].gameObject.SetActive(true);  // 횃대 먹이 프리팹 활성화
                RackTriggerObjects[i].gameObject.SetActive(true);   // 횃대 트리거 활성화
                RackBirdObjects[i].gameObject.SetActive(true);  // 레벨별 새 부모 오브젝트 활성화
            }
            else
            {
                // 이외 레벨은 모두 비활성화

                RackFeedObjects[i].gameObject.SetActive(false);
                RackTriggerObjects[i].gameObject.SetActive(false);
                RackBirdObjects[i].gameObject.SetActive(false);
            }
        }
    }

    public void ArriveRackBird(int rackNumber, int birdNumber)
    {
        // 먹이 시간이 다 된 새가 나타나는 함수

        this.GetComponent<BirdSelect>().ChangeBirdImage(RackBirdObjects[rackLevel].transform.GetChild(rackNumber).gameObject, birdNumber);    // 새 이미지 변경
        SetActiveRackBird(rackNumber);   // 새 오브젝트 활성화
    }

    public void SelectFeed(int rackNumber, int feedNumber)
    {
        // 먹이를 선택하는 함수

        Debug.Log(feedNumber + " 번 먹이 선택함");
        // 새 종류 설정 (먹이에 따라 랜덤 설정)
        BirdSelect birdSelector = this.GetComponent<BirdSelect>();
        int randomBird = birdSelector.SelectBirdType(feedNumber); // 확률에 따라 랜덤으로 해당 먹이의 새를 정함

        // 먹이 시간 랜덤 설정
        BirdInfo_Data birdinfo_data = GameManager.instance.birdinfo_data;   // 새 도감 데이터를 가져옴
        int birdStartTime = birdinfo_data.dataList[randomBird].startTime;  // 도감에서 해당 새의 시작 시간 데이터를 가져옴
        int birdEndTime = birdinfo_data.dataList[randomBird].endTime;  // 도감에서 해당 새의 끝 시간 데이터를 가져옴
        int randomTime = UnityEngine.Random.Range(birdStartTime, birdEndTime + 1);    // 랜덤으로 소요 시간을 정함

        // 꽃병 레벨별 먹이 시간 감소치 적용
        // ** 수정 필요 (3은 상점 데이터의 꽃병 시작 인덱스. 해당 부분을 수정하기)
        int goodsLevel = GameManager.instance.loadGoodsData.goodsList[1].goodsLevel;  // 꽃병의 레벨을 가져옴
        int vaseEffect = int.Parse(GameManager.instance.storeinfo_data.dataList[goodsLevel + 3].effect.ToString());    // 감소 효과를 가져옴
        double decreaseTime = (double)vaseEffect * 0.01 * randomTime;    // 퍼센트를 적용하여 감소 시간 계산


        // 랜덤 새 정보와 랜덤 시간 정보 저장 및 타이머 시작
        FeedTimer feedTimer = this.GetComponent<FeedTimer>();
        feedTimer.SaveTimerData(rackNumber, randomTime, (float)decreaseTime, feedNumber, randomBird);   //타이머 데이터 저장(위에서 설정한 데이터 저장)
        feedTimer.UpdateTimerSetting(); // 타이머 상태 업데이트


        SetActiveRackFeed(rackNumber, feedNumber);     // 횃대 먹이 활성화
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_SelectFeed(); // 먹이두기 효과음
        this.gameObject.GetComponent<FeedPanel>().OpenOrCloseFeedPanel(false);    // 먹이 선택 패널을 닫음
    }

    public void TouchBirdGetFeather(int birdIdx)
    {
        // 새를 터치하여 깃털을 얻는 함수

        // 인벤토리 용량 확인
        InventoryData inventoryData = GameObject.FindGameObjectWithTag("GoodsManager").GetComponent<InventoryData>();  // 상품 데이터를 가져옴
        int inventoryMax = inventoryData.CheckItemMaximum();   // 인벤토리 최대 개수
        int inventoryCnt = inventoryData.GetInventoryItemCnt();   // 인벤토리 현재 아이템 개수

        BirdContainer saveBirds = GameManager.instance.loadBirdData;    // 저장된(먹이준) 새 데이터를 가져옴
        int birdNumber = saveBirds.birdList[birdIdx].birdNumber;  // 터치한 새 고유 번호를 가져옴
        MyFeatherNumber featherData = GameManager.instance.loadFeatherData; // 깃털 데이터를 가져옴
        int featherCnt = featherData.featherList[birdNumber].feather_number;  // 해당 새의 현재 깃털 개수 가져옴


        if (featherCnt != 0 || inventoryCnt < inventoryMax)    // 이미 인벤토리에 가지고 있는 깃털이거나, 인벤토리가 꽉차지 않았다면 깃털 추가
        {
            if (featherData.featherList[birdNumber].appear == 0)     // 한 번도 나타나지 않은 새라면
            {
                featherData.featherList[birdNumber].appear = 1; // 도감에 증가한 등장 여부 수정
            }

            featherData.featherList[birdNumber].feather_number += 1;   // 얻은 깃털 개수 증가
            GameManager.instance.GetComponent<FeatherNumDataManager>().DataSaveText(featherData);   // 증가한 깃털 개수 저장
            inventoryData.AddInventory(birdNumber);  // 얻은 깃털을 인벤토리에 추가  -> ** 이걸 개수 증가 함수가 아니라 그냥 인벤토리 업데이트하는 걸로 바꿀까?


            RackBirdObjects[rackLevel].gameObject.transform.GetChild(birdIdx).gameObject.SetActive(false);       // 새 오브젝트 비활성화
            this.GetComponent<FeedTimer>().SaveTimerData(birdIdx, false, false);    // 새 획득 정보 저장 -> ** BirdContainer에 저장 함수를 넣는 걸로 바꿀까?


            GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_MakingOrFeather(); //깃털 수확 효과음
        }
        else
        {
            // 가지고 있지 않은 깃털이고, 인벤토리가 꽉찼을 때

            Debug.Log("인벤토리에 자리가 없어 깃털을 획득할 수 없음");
        }
        // 현재 가지고 있는 깃털이고, 인벤토리에 자리가 있을 때 -> 기존 깃털에 개수 추가
        // 현재 가지고 있는 깃털이고, 인벤토리에 자리가 없을 때 -> 기존 깃털에 개수 추가
        // 현재 가지고 있지 않은 깃털이고, 인벤토리에 자리가 있을 때 -> 인벤토리에 새로운 깃털 추가
        // 현재 가지고 있지 않은 깃털이고, 인벤토리에 자리가 없을 때 -> 새를 클릭하여도 깃털 획득되지 않음
    }

    public void SetActiveRackFeed(int rackNumber, int feedNumber)
    {
        // 횃대의 먹이를 활성화하는 함수

        Debug.Log(feedNumber + " 번 먹이 활성화 함");

        int rackLevel = GameManager.instance.loadGoodsData.goodsList[0].goodsLevel; //횃대 레벨
        RackFeedObjects[rackLevel].transform.GetChild(rackNumber).GetChild(feedNumber).gameObject.SetActive(true);
    }

    public void SetInactiveRackFeed(int rackNumber, int feedNumber)
    {
        // 횃대의 먹이를 비활성화 하는 함수

        int rackLevel = GameManager.instance.loadGoodsData.goodsList[0].goodsLevel; //횃대 레벨
        RackFeedObjects[rackLevel].transform.GetChild(rackNumber).GetChild(feedNumber).gameObject.SetActive(false);
    }

    public void SetActiveRackBird(int rackNumber)
    {
        // 횃대의 새를 활성화하는 함수

        RackBirdObjects[rackLevel].transform.GetChild(rackNumber).gameObject.SetActive(true);
    }

    public void SetInactiveRackBird(int rackNumber)
    {
        // 횃대의 새를 비활성화 하는 함수

        RackBirdObjects[rackLevel].transform.GetChild(rackNumber).gameObject.SetActive(false);
    }
}

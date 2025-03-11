using System.Collections.Generic;
using UnityEngine;

public class FeedManager : MonoBehaviour
{
    // 먹이 기능 총괄 스크립트

    [Header("[Feed Objects]")]
    public GameObject[] FeedObjects;         // 드래그 하는 먹이 프리팹 배열
    public GameObject[] RackFeedObjects;     // 각 레벨별 횃대의 먹이 프리팹 배열
    public GameObject[] RackTriggerObjects;  // 횃대 트리거 프리팹 배열
    public GameObject[] RackBirdObjects;     // 횃대에 나타나는 새 오브젝트 배열

    private int rackLevel;

    void Start()
    {
        var dataList = GameManager.instance.goodsDataManager.dataList;
        if (dataList == null || dataList.Count == 0)
        {
            Debug.LogError("goodsDataManager.dataList is null or empty!");
        }

        var rackData = GameManager.instance.goodsDataManager.GetGoodsData(Constants.GoodsData_Rack);
        if (rackData == null)
        {
            Debug.LogError($"No data found for category: {Constants.GoodsData_Rack}");
        }
        foreach (var item in GameManager.instance.goodsDataManager.dataList)
        {
            Debug.Log($"GoodsData Item: ID={item.id}, Category={item.category}, Level={item.level}");
        }

        rackLevel = GameManager.instance.goodsDataManager.GetGoodsData(Constants.GoodsData_Rack).level;   // 플레이어의 횃대 레벨

        InitializeFeedObjects();
        InitializeRackObjects();
        UpdateRackSetting();   // 횃대 정보 업데이트
    }

    public void InitializeFeedObjects()
    {
        for (int i = 0; i < FeedObjects.Length; i++)
        {
            FeedObjects[i].GetComponent<FeedDrag>().Feed = (FeedType)i;
        }
    }

    public void InitializeRackObjects()
    {
        for (int j = 0; j < RackTriggerObjects[rackLevel].transform.childCount; j++)
        {
            var rackTrigger = RackTriggerObjects[rackLevel].transform.GetChild(j).GetComponent<RackTrigger>();
            var rackBird = RackBirdObjects[rackLevel].transform.GetChild(j).GetComponent<RackBird>();
            rackTrigger.TriggerNumber = j;
            rackBird.RackNumber = j;
        }
    }

    public void UpdateRackSetting()
    {
        // 횃대 상태 업데이트 함수 (트리거, 먹이 활성화 / 비활성화)

        for (int i = 0; i < RackTriggerObjects.Length; i++)        // 횃대 트리거 수만큼 반복
        {
            if (i == rackLevel)     // 현재 횃대 레벨이라면
            {
                RackFeedObjects[i].gameObject.SetActive(true);      // 횃대 먹이 프리팹 활성화
                RackTriggerObjects[i].gameObject.SetActive(true);   // 횃대 트리거 활성화
                RackBirdObjects[i].gameObject.SetActive(true);      // 레벨별 새 부모 오브젝트 활성화
            }
            else                    // 이외 레벨은 모두 비활성화
            {
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

        SetActiveRackBird(rackNumber);      // 새 오브젝트 활성화
    }

    public void SelectFeed(int rackNumber, FeedType feed)
    {
        // 먹이를 선택하는 함수

        BirdInfo_Data birdinfo_data = GameManager.instance.birdinfo_data;               // 새 도감 데이터를 가져옴
        FeedTimer feedTimer = this.GetComponent<FeedTimer>();

        int randomBird = this.GetComponent<BirdSelect>().SelectBirdType(feed);          // 랜덤으로 먹이의 새를 정함
        int randomTime = Random.Range(birdinfo_data.dataList[randomBird].startTime,
            birdinfo_data.dataList[randomBird].endTime + 1);                            // 랜덤으로 소요 시간을 정함

        List<RackData> datalist = GameManager.instance.rackDataList;    // 플레이어 데이터를 가져옴

        // 먹이를 추가하는 경우
        // 1. 리스트가 비었을 때 (null일 때)
        // 2. 리스트에 값이 존재하긴 하지만, 선택한 횃대 번호인 rackNumber 번째 데이터는 없을 때
        // 3. 리스트에 값이 존재하고 선택한 횃대 번호인 rackNumber에도 데이터가 존재하지만, datalist[rackNumber].isFed가 False일 때
        // 위 경우만 함수가 실행되고, 이외에는 함수 종료
        if (datalist == null || datalist.Count <= rackNumber)
        {
            int vaseLevel = GameManager.instance.goodsDataManager.GetGoodsData(Constants.GoodsData_Vase).level;  // 꽃병의 레벨을 가져옴
            int vaseStartNumber = GetCategoryStartNumber(StoreItemCategory.Vase);// 꽃병의 시작 번호를 가져옴
            int vaseEffect = int.Parse(GameManager.instance.storeinfo_data.dataList[vaseLevel + vaseStartNumber].effect.ToString());    // 감소 효과를 가져옴
            double decreaseTime = (double)vaseEffect * 0.01 * randomTime;    // 퍼센트를 적용하여 감소 시간 계산

            feedTimer.SaveTimerData(rackNumber, randomTime, (float)decreaseTime, feed, randomBird);   //타이머 데이터 저장
            feedTimer.UpdateTimerSetting(); // 타이머 상태 업데이트


            SetActiveRackFeed(rackNumber, feed);     // 횃대 먹이 활성화
            GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_SelectFeed(); // 먹이두기 효과음
            this.gameObject.GetComponent<FeedPanel>().SetFeedPanelActive(false);    // 먹이 선택 패널을 닫음
        }
        else
        {
            if (datalist[rackNumber].isFed)
            {
                return;
            }
            else
            {
                int vaseLevel = GameManager.instance.goodsDataManager.GetGoodsData(Constants.GoodsData_Vase).level;  // 꽃병의 레벨을 가져옴
                int vaseStartNumber = GetCategoryStartNumber(StoreItemCategory.Vase);// 꽃병의 시작 번호를 가져옴
                int vaseEffect = int.Parse(GameManager.instance.storeinfo_data.dataList[vaseLevel + vaseStartNumber].effect.ToString());    // 감소 효과를 가져옴
                double decreaseTime = (double)vaseEffect * 0.01 * randomTime;    // 퍼센트를 적용하여 감소 시간 계산

                feedTimer.SaveTimerData(rackNumber, randomTime, (float)decreaseTime, feed, randomBird);   //타이머 데이터 저장
                feedTimer.UpdateTimerSetting(); // 타이머 상태 업데이트


                SetActiveRackFeed(rackNumber, feed);     // 횃대 먹이 활성화
                GameObject.FindGameObjectWithTag(Constants.Tag_AudioManager).GetComponent<EffectChange>().PlayEffect_SelectFeed(); // 먹이두기 효과음
                this.gameObject.GetComponent<FeedPanel>().SetFeedPanelActive(false);    // 먹이 선택 패널을 닫음
            }
        }
    }

    public void TouchBirdGetFeather(int rackNumber, int birdNumber)
    {
        // 새를 터치하여 깃털을 얻는 함수

        // 인벤토리 용량 확인
        // if(인벤토리 용량 꽉찼는지) 해서 꽉찼으면 바로 return 하는 코드 추가하기
        InventoryManager inventoryData = GameObject.FindGameObjectWithTag(Constants.Tag_GoodsManager).GetComponent<InventoryManager>();  // 상품 데이터를 가져옴
        int inventoryMax = inventoryData.CheckItemMaximum();   // 인벤토리 최대 개수
        int inventoryCnt = inventoryData.GetInventoryItemCnt();   // 인벤토리 현재 아이템 개수

        MyFeatherNumber featherData = GameManager.instance.featherDataManager; // 깃털 데이터를 가져옴
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


            RackBirdObjects[rackLevel].gameObject.transform.GetChild(rackNumber).gameObject.SetActive(false);       // 새 오브젝트 비활성화
            this.GetComponent<FeedTimer>().SaveTimerData(rackNumber, false, false);    // 새 획득 정보 저장 -> ** BirdContainer에 저장 함수를 넣는 걸로 바꿀까?


            GameObject.FindGameObjectWithTag(Constants.Tag_AudioManager).GetComponent<EffectChange>().PlayEffect_MakingOrFeather(); //깃털 수확 효과음
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

    public void SetActiveRackFeed(int rackNumber, FeedType feed)
    {
        // 횃대의 먹이를 활성화하는 함수

        int rackLevel = GameManager.instance.goodsDataManager.GetGoodsData(Constants.GoodsData_Rack).level; //횃대 레벨
        RackFeedObjects[rackLevel].transform.GetChild(rackNumber).GetChild((int)feed).gameObject.SetActive(true);
    }

    public void SetInactiveRackFeed(int rackNumber, FeedType feed)
    {
        // 횃대의 먹이를 비활성화 하는 함수

        int rackLevel = GameManager.instance.goodsDataManager.GetGoodsData(Constants.GoodsData_Rack).level; //횃대 레벨
        RackFeedObjects[rackLevel].transform.GetChild(rackNumber).GetChild((int)feed).gameObject.SetActive(false);
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


    // 밑에 이거 storeData.cs 함수인데, 어떻게 가져올지, 스크립트 분리할지 고민하기
    public int GetCategoryStartNumber(StoreItemCategory category)
    {
        // 각 카테고리별 시작 번호를 반환하는 함수

        StoreInfo_Data storeInfo_Data = GameManager.instance.storeinfo_data; ;

        int startNumber = -1;   // 초기값으로 -1을 설정

        for (int i = 0; i < storeInfo_Data.dataList.Count; i++)
        {
            if (storeInfo_Data.dataList[i].category == category)
            {
                startNumber = i;
                break;
            }
        }

        return startNumber;
    }
}

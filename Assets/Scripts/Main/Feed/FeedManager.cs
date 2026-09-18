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
    [SerializeField] private FeedTimer feedTimer;
    [SerializeField] private BirdSelect birdSelect;
    [SerializeField] private FeedPanel feedPanel;
    [SerializeField] private EffectChange effectChange;

    private void Awake()
    {
        if (feedTimer == null) feedTimer = GetComponent<FeedTimer>();
        if (birdSelect == null) birdSelect = GetComponent<BirdSelect>();
        if (feedPanel == null) feedPanel = GetComponent<FeedPanel>();

        if (effectChange == null)
        {
            GameObject audioObject = GameObject.FindGameObjectWithTag(Constants.Tag_AudioManager);
            if (audioObject != null) effectChange = audioObject.GetComponent<EffectChange>();
        }
    }

    void Start()
    {
        var dataList = GameManager.instance.goodsDataManager.dataList;
        if (dataList == null || dataList.Count == 0)
        {
            Debug.LogError("goodsDataManager.dataList is null or empty!");
            return;
        }

        var rackData = GameManager.instance.goodsDataManager.GetValidatedGoodsData(Constants.GoodsData_Rack);
        if (rackData == null)
        {
            Debug.LogError($"No data found for category: {Constants.GoodsData_Rack}");
            return;
        }

        rackLevel = rackData.level;   // 플레이어의 횃대 레벨
        if (rackLevel < 0 || rackLevel >= RackFeedObjects.Length
            || rackLevel >= RackTriggerObjects.Length || rackLevel >= RackBirdObjects.Length)
        {
            Debug.LogError($"[FeedManager] 횃대 레벨에 맞는 오브젝트가 없습니다. level: {rackLevel}");
            return;
        }

        if (IsTutorialInProgress())
        {
            ClearTutorialRackState();
        }

        InitializeFeedObjects();
        InitializeRackObjects();
        UpdateRackSetting();   // 횃대 정보 업데이트
        RestoreRackState();    // 저장된 횃대 시각 상태 복원
    }

    private bool IsTutorialInProgress()
    {
        PlayerDataManager playerDataManager = GameManager.instance.playerDataManager;
        return !playerDataManager.GetIsQuestActinoPlaying()
            && TutorialManager.IsTutorialScene(playerDataManager.GetCurrentScene());
    }

    private void ClearTutorialRackState()
    {
        List<RackData> dataList = GameManager.instance.rackDataList;
        if (dataList == null)
        {
            dataList = new List<RackData>();
            GameManager.instance.rackDataList = dataList;
        }
        else
        {
            dataList.Clear();
        }

        GameManager.instance.RackDataManager.SetData(dataList);
        foreach (GameObject rackFeedObject in RackFeedObjects)
        {
            for (int rackIndex = 0; rackIndex < rackFeedObject.transform.childCount; rackIndex++)
            {
                Transform rackFeed = rackFeedObject.transform.GetChild(rackIndex);
                for (int feedIndex = 0; feedIndex < rackFeed.childCount; feedIndex++)
                {
                    rackFeed.GetChild(feedIndex).gameObject.SetActive(false);
                }
            }
        }

        foreach (GameObject rackBirdObject in RackBirdObjects)
        {
            for (int rackIndex = 0; rackIndex < rackBirdObject.transform.childCount; rackIndex++)
            {
                rackBirdObject.transform.GetChild(rackIndex).gameObject.SetActive(false);
            }
        }

        FeedTimer feedTimer = GetComponent<FeedTimer>();
        if (feedTimer != null)
        {
            feedTimer.ClearTimerObjects();
        }
    }

    public void RestoreRackState()
    {
        // 앱 재시작 시 저장된 데이터로 횃대 먹이·새 시각 상태를 복원하는 함수

        List<RackData> dataList = GameManager.instance.rackDataList;
        if (dataList == null || dataList.Count == 0) return;

        for (int i = 0; i < dataList.Count; i++)
        {
            RackData data = dataList[i];

            if (data.isFed && !data.isAppeared)
            {
                // 먹이가 놓여 있고 새가 아직 안 왔음 → 먹이 오브젝트 표시
                SetActiveRackFeed(i, data.feed);
            }
            else if (!data.isFed && data.isAppeared)
            {
                // 타이머 만료, 새가 등장했지만 아직 수확 안 됨 → 새 표시
                ArriveRackBird(i, data.birdNumber);
            }
        }
    }

    public void InitializeFeedObjects()
    {
        for (int i = 0; i < FeedObjects.Length; i++)
        {
            FeedDrag[] allFeedDrags = FeedObjects[i].GetComponentsInChildren<FeedDrag>(true);
            if (allFeedDrags.Length == 0)
            {
                Debug.LogError($"[FeedManager] FeedObjects[{i}] ({FeedObjects[i].name})에 FeedDrag가 없습니다!");
                continue;
            }
            foreach (var feedDrag in allFeedDrags)
            {
                feedDrag.Feed = (FeedType)i;
                feedDrag.SetFeedManager(this);
            }
        }
    }

    public void InitializeRackObjects()
    {
        int rackCount = Mathf.Min(RackTriggerObjects[rackLevel].transform.childCount,
            RackBirdObjects[rackLevel].transform.childCount);
        for (int j = 0; j < rackCount; j++)
        {
            var rackTrigger = RackTriggerObjects[rackLevel].transform.GetChild(j).GetComponent<RackTrigger>();
            var rackBird = RackBirdObjects[rackLevel].transform.GetChild(j).GetComponent<RackBird>();
            if (rackTrigger != null)
            {
                rackTrigger.TriggerNumber = j;
                rackTrigger.SetDependencies(feedPanel);
            }
            if (rackBird != null) rackBird.SetDependencies(j, this);
        }
    }

    public void UpdateRackSetting()
    {
        // 횃대 상태 업데이트 함수 (트리거, 먹이 활성화 / 비활성화)

        int levelObjectCount = Mathf.Min(RackFeedObjects.Length,
            Mathf.Min(RackTriggerObjects.Length, RackBirdObjects.Length));
        for (int i = 0; i < levelObjectCount; i++)        // 횃대 트리거 수만큼 반복
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

        birdSelect.ChangeBirdImage(RackBirdObjects[rackLevel].transform.GetChild(rackNumber).gameObject, birdNumber);    // 새 이미지 변경

        SetActiveRackBird(rackNumber);      // 새 오브젝트 활성화
    }

    public void SelectFeed(int rackNumber, FeedType feed)
    {
        TrySelectFeed(rackNumber, feed);
    }

    public bool TrySelectFeed(int rackNumber, FeedType feed)
    {
        // 먹이 배치 가능 여부를 검증한 뒤 타이머·시각 상태를 함께 적용합니다.

        if ((int)feed > GameManager.instance.playerDataManager.GetFoodUnlockLevel())
        {
            Debug.LogWarning($"[FeedManager] 잠긴 먹이는 사용할 수 없습니다. feed: {feed}");
            return false;
        }

        List<RackData> datalist = GameManager.instance.rackDataList;
        if (rackNumber < 0 || (datalist != null && rackNumber < datalist.Count
            && (datalist[rackNumber].isFed || datalist[rackNumber].isAppeared)))
        {
            return false;
        }

        BirdInfo_Data birdinfo_data = GameManager.instance.birdinfo_data;               // 새 도감 데이터를 가져옴
        bool isInTutorial = TutorialManager.IsTutorialScene(
            GameManager.instance.playerDataManager.GetCurrentScene());
        int randomBird = isInTutorial && feed == FeedType.PigeonBeans
            ? birdSelect.SelectTutorialBirdType(feed)
            : birdSelect.SelectBirdType(feed);
        int randomTime = Random.Range(birdinfo_data.dataList[randomBird].startTime,
            birdinfo_data.dataList[randomBird].endTime + 1);                            // 랜덤으로 소요 시간을 정함

        // 튜토리얼 중 비둘기콩은 30초 고정
        if (isInTutorial && feed == FeedType.PigeonBeans)
            randomTime = 30;

        int vaseLevel = GameManager.instance.goodsDataManager.GetValidatedGoodsData(Constants.GoodsData_Vase).level;
        int vaseStartNumber = GetCategoryStartNumber(StoreItemCategory.Vase);
        int vaseDataIndex = vaseLevel + vaseStartNumber;
        if (vaseStartNumber < 0 || vaseDataIndex < 0
            || vaseDataIndex >= GameManager.instance.storeinfo_data.dataList.Count)
        {
            Debug.LogError("[FeedManager] 꽃병 효과 데이터를 찾을 수 없습니다.");
            return false;
        }

        int vaseEffect = int.Parse(GameManager.instance.storeinfo_data.dataList[vaseDataIndex].effect.ToString());
        float decreaseTime = vaseEffect * 0.01f * randomTime;

        feedTimer.SaveTimerData(rackNumber, randomTime, decreaseTime, feed, randomBird);
        feedTimer.UpdateTimerSetting();
        SetActiveRackFeed(rackNumber, feed);
        if (effectChange != null) effectChange.PlayEffect_SelectFeed();
        if (feedPanel != null) feedPanel.SetFeedPanelActive(false);
        return true;
    }

    public void TouchBirdGetFeather(int rackNumber, int birdNumber)
    {
        // 새를 터치하여 깃털을 얻는 함수

        BirdInfo_Data birdInfoData = GameManager.instance.birdinfo_data;
        if (birdNumber >= 0 && birdNumber < birdInfoData.dataList.Count &&
            birdInfoData.dataList[birdNumber].isSpecial)
        {
            // 특별 새는 깃털이 없으므로 도감 등장 처리만 유지하고 횃대에서 돌려보낸다.
            SetInactiveRackBird(rackNumber);
            this.GetComponent<FeedTimer>().SaveTimerData(rackNumber, false, false);
            return;
        }

        // 인벤토리 용량 확인
        // if(인벤토리 용량 꽉찼는지) 해서 꽉찼으면 바로 return 하는 코드 추가하기
        InventoryManager inventoryData = GameObject.FindGameObjectWithTag(Constants.Tag_GoodsManager).GetComponent<InventoryManager>();  // 상품 데이터를 가져옴
        int inventoryMax = inventoryData.CheckItemMaximum();   // 인벤토리 최대 개수
        int inventoryCnt = inventoryData.GetInventoryItemCnt();   // 인벤토리 현재 아이템 개수

        FeatherDataManager featherData = GameManager.instance.featherDataManager; // 깃털 데이터를 가져옴
        int featherCnt = featherData.GetFeatherCount(birdNumber); // 해당 새의 현재 깃털 개수 가져옴


        if (featherCnt != 0 || inventoryCnt < inventoryMax)    // 이미 인벤토리에 가지고 있는 깃털이거나, 인벤토리가 꽉차지 않았다면 깃털 추가
        {
            featherData.AddFeather(birdNumber, 1); // 얻은 깃털 개수 증가


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

        int rackLevel = GameManager.instance.goodsDataManager.GetValidatedGoodsData(Constants.GoodsData_Rack).level; //횃대 레벨
        RackFeedObjects[rackLevel].transform.GetChild(rackNumber).GetChild((int)feed).gameObject.SetActive(true);
    }

    public void SetInactiveRackFeed(int rackNumber, FeedType feed)
    {
        // 횃대의 먹이를 비활성화 하는 함수

        int rackLevel = GameManager.instance.goodsDataManager.GetValidatedGoodsData(Constants.GoodsData_Rack).level; //횃대 레벨
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

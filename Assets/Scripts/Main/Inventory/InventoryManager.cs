using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //인벤토리 데이터를 가져오는 클래스

    [Header("[Inventory Product]")]
    public GameObject ItemPrefab;   // 아이템 프리팹
    public GameObject ItemContent;  //아이템 오브젝트를 넣을 부모 오브젝트
    public Sprite[] ItemImages; //아이템 이미지 배열
    public GameObject dreamCatcherPrefab;   //드림캐쳐 프리팹

    [Space]
    [Header("[Inventory Data]")]
    public int itemMaxCnt;  //인벤토리 최대 용량
    public int itemCurCnt;  //인벤토리 현재 아이템 수
    public List<GameObject> itemList;   //인벤토리 아이템 리스트    
    FeatherDataManager featherData;    //플레이어 깃털 정보
    private DreamCatcherDataManager dreamCatcherDataManager;
    private DreamCatcherInventoryDataManager dreamCatcherInventoryDataManager;
    private BirdInfo_Data birdInfo_Data;

    private void Awake()
    {
        featherData = GameManager.instance.featherDataManager;
        birdInfo_Data = GameManager.instance.birdinfo_data;
        dreamCatcherDataManager = GameManager.instance.dreamCatcherDataManager;
        dreamCatcherInventoryDataManager = GameManager.instance.dreamCatcherInventoryDataManager;
    }
    void Start()
    {
        
    }

    public void OpenInventory()
    {
        //인벤토리 열 때마다 세팅하는 함수
        int maxCount = CheckItemMaximum();
        FillItemSlotIfNeeded(10);//나중에 매개변수 maxCount로 수정
        UpdateInventory();
    }

    public void FillItemSlotIfNeeded(int maxItemCount)
    {
        //상자가 업그레이드되면 용량만큼 인벤토리 리스트 요소를 추가하는 함수
        int curCnt = itemList.Count; // 현재 아이템 슬롯(빈것 포함) 수

        // 최대 갯수보다 현재 아이템 갯수가 많거나 같으면 작동 X
        if (curCnt >= maxItemCount)
        {
            return;
        }

        int addCnt = maxItemCount - curCnt;   //추가해야할 요소 수

        // 추가 해야하는 요소 수 만큼 빈 아이템 슬롯 생성
        for (int i = 0; i < addCnt; i++)
        {
            GameObject obj = Instantiate(ItemPrefab, ItemContent.transform, false);
            obj.SetActive(false);
            itemList.Add(obj);
        }
    }

    public int CheckItemMaximum()
    {
        //인벤토리 최대 용량을 반환하는 함수

        int PocketLevel = GameManager.instance.goodsDataManager.GetValidatedGoodsData(Constants.GoodsData_Box).level;  //주머니의 레벨을 가져옴

        StoreInfo_Data _storeinfo_data = GameManager.instance.storeinfo_data;  
        int boxEffect = int.Parse(_storeinfo_data.dataList[PocketLevel + 8].effect);    //상자 효과를 가져옴(12는 상점 데이터의 박스 시작 인덱스)
        //위 이거 말고 그냥 플레이어 데이터 저장할 때 상품 레벨(0,1,2,3)이 아니라 아이템 id로 저장하기.. 그래서 아이템 id 바로 가져와서 그걸로 조회하기?

        int maxCnt = boxEffect;

        return maxCnt;
    }

    public int GetInventoryItemCnt()
    {
        //현재 인벤토리의 요소 개수를 반환하는 함수

        FeatherDataManager featherData = GameManager.instance.featherDataManager;
        int itemCurCnt = 0;
        int featherCnt = featherData.GetFeatherDataListCount();
        Debug.Log("featherData.Count : " + featherCnt);
        for (int i = 0; i < featherCnt; i++)     //깃털 개수만큼 반복(**나중에 드림캐쳐 개수도 추가해야함)
        {
            if (featherData.GetFeatherCount(i) > 0)
            {
                itemCurCnt++;   //인벤토리 요소 카운트 증가
            }
        }

        //드림캐쳐 개수도 추가
        DreamCatcherDataManager dreamCatcherDataManager=GameManager.instance.dreamCatcherDataManager;
        int dreamCatcherCnt = dreamCatcherDataManager.GetDreamCatcherCount();
        itemCurCnt += dreamCatcherCnt;

        return itemCurCnt;
    }

    public void UpdateInventory()
    {
        List<BirdInfo_Object> birdInfoDatalist = birdInfo_Data.dataList;

        int listCnt = 0;

        // 깃털
        for (int i = 0; i < featherData.GetFeatherDataListCount(); i++)
        {
            // 특별새라면 스킵
            if (birdInfoDatalist[i].isSpecial)
            {
                continue;
            }

            int count = featherData.GetFeatherCount(i);

            if (count > 0)
            {
                GameObject slot=itemList[listCnt];
                slot.SetActive(true);
                slot.GetComponent<InventoryItemSlot>().SetSlotFeather(ItemImages[i], birdInfoDatalist[i].name, "", count);
                listCnt++;
            }
        }

        //드림캐쳐
        for(int i = 0; i<dreamCatcherInventoryDataManager.GetDreamCatcherInventoryDataListCount(); i++)
        {
            GameObject slot = itemList[listCnt];
            slot.SetActive(true);
            slot.GetComponent<InventoryItemSlot>().SetSlotDreamCatcher(null, "드림 캐쳐", dreamCatcherInventoryDataManager.dreamCatcherInventoryDataList[i].Description, dreamCatcherInventoryDataManager.dreamCatcherInventoryDataList[i].Number);
            listCnt ++;
        }
    }

    public void AddFeatherInventory(int featherIndexNumber, int cnt = 1)
    {
        //증가할 아이템 번호로 해당 인덱스의 깃털을 개수를 추가하는 함수
        featherData = GameManager.instance.featherDataManager;   //깃털 정보를 가져옴

        featherData.AddFeather(featherIndexNumber, cnt); //깃털 개수 증가
        if (!featherData.IsFeatherAppeared(featherIndexNumber))
        {
            featherData.UnlockFeather(featherIndexNumber);
        }
        Debug.Log("인벤토리에 추가됨");
    }

    public void DeleteFeatherInventory(int featherIndexNumber, int cnt)
    {
        //인벤토리 아이템을 삭제하는(사용&판매하는) 함수
        featherData = GameManager.instance.featherDataManager;   //깃털 정보를 가져옴

        featherData.RemoveFeather(featherIndexNumber, cnt);
        Debug.Log("인벤토리에서 삭제됨");
    }
}

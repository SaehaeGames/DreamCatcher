using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class InteriorCategory : MonoBehaviour
{
    public GameObject Panel_Interior;    GameObject ScrollViewPort;
    public GameObject[] Button_InteriorCategory;
    public GameObject[] Button_InteriorItem;
    public GameObject[] horLines;

    private List<GameObject> interiorItemArray;
    private bool[] currentAdjusting;
    public int currentCategoryIndex;
    private InteriorDataManager interiorDataManager;
    private GoodsDataManager goodsDataManager;
    private StoreInfo_Data storeInfoData;

    void Start()
    {
        currentCategoryIndex = 0;
        ScrollViewPort = Panel_Interior.transform.GetChild(2).GetChild(0).gameObject;
        for (int i = 0; i < Button_InteriorCategory.Length; i++)
            Button_InteriorCategory[i].GetComponent<InteriorButton>().buttonNumber = i;

        interiorDataManager = GameManager.instance.interiorDataManager;
        goodsDataManager = GameManager.instance.goodsDataManager;
        storeInfoData = GameManager.instance.storeinfo_data;

        GetBtnItemCount();
        currentAdjusting = new bool[interiorItemArray.Count];

        // 초기화 & 적용 중인 아이템 UI 반영
        for (int i = 0; i < interiorItemArray.Count; i++)
        {
            currentAdjusting[i] = interiorDataManager.dataList[i].isAdjusting;
            if (currentAdjusting[i])
            {
                string id = interiorDataManager.dataList[i].storeinfo_id;
                UpdateInteriorImage(i, id);
                UpdateButtonAdjusting();
            }
        }

        // 보유/비보유 상태 반영
        for (int i = 0; i < interiorItemArray.Count; i++)
            SettingItemHide(i);

        //DefineButtonNumber();
        LoadSaveData();
        UpdateCatrgoryPanel(currentCategoryIndex);
    }

    public void UpdateCatrgoryPanel(int panelIdx)
    {
        OpenIndexPanel(panelIdx);
        SetHorLine(panelIdx);
        DefineButtonNumberAndID();
    }

    void OpenIndexPanel(int idx)
    {
        int panelCnt = ScrollViewPort.transform.childCount;
        for (int i = 0; i < panelCnt; i++)
            ScrollViewPort.transform.GetChild(i).gameObject.SetActive(i == idx);
    }

    void SetHorLine(int curCateNum)
    {
        horLines[0].SetActive(curCateNum != 0);
        horLines[1].SetActive(curCateNum != 1);
        horLines[2].SetActive(curCateNum != 2);
    }

    int GetBtnItemCount()
    {
        interiorItemArray = new List<GameObject>();
        foreach (var cat in Button_InteriorItem)
        {
            Transform parent = cat.transform.GetChild(1);
            for (int j = 0; j < parent.childCount; j++)
            {
                var go = parent.GetChild(j).gameObject;
                if (go.GetComponent<InteriorButton>() != null)
                    interiorItemArray.Add(go);
            }
        }
        return interiorItemArray.Count;
    }

    /*    void DefineButtonNumber()
        {
            List<int> combinedIDs = new List<int>();
            combinedIDs.AddRange(storeInfoData.GetSortedIDsByTheme(ItemTheme.Default));
            combinedIDs.AddRange(storeInfoData.GetSortedIDsByTheme(ItemTheme.Sea));
            combinedIDs.AddRange(storeInfoData.GetSortedIDsByTheme(ItemTheme.Star));

            int idx = 0;
            foreach (var cat in Button_InteriorItem)
            {
                var parent = cat.transform.GetChild(cat.transform.childCount - 1);
                for (int j = 0; j < parent.childCount; j++)
                {
                    var btn = parent.GetChild(j).GetComponent<InteriorButton>();
                    btn.SetButtonNumber(idx);
                    btn.SetButtonItemID(combinedIDs[idx]);
                    idx++;
                }
            }
        }*/

    /* void DefineButtonNumber()
     {
         List<int> combinedIDs = new List<int>();
         combinedIDs.AddRange(storeInfoData.GetSortedIDsByTheme(ItemTheme.Default));
         combinedIDs.AddRange(storeInfoData.GetSortedIDsByTheme(ItemTheme.Sea));
         combinedIDs.AddRange(storeInfoData.GetSortedIDsByTheme(ItemTheme.Star));

         int idx = 0;
         foreach (var cat in Button_InteriorItem)
         {
             var parent = cat.transform.GetChild(cat.transform.childCount - 1);
             for (int j = 0; j < parent.childCount; j++)
             {
                 var btn = parent.GetChild(j).GetComponent<InteriorButton>();

                 if (idx >= combinedIDs.Count)
                 {
                     btn.SetButtonNumber(idx);
                     btn.SetButtonItemID(1);
                 }
                 else
                 {
                     btn.SetButtonNumber(idx);
                     btn.SetButtonItemID(combinedIDs[idx]);
                 }
                 idx++;
             }
         }
     }*/

    void DefineButtonNumberAndID()
    {
        // 각 탭에 대응되는 Theme와 패널
        var tabMappings = new (ItemTheme theme1, StoreItemCategory theme2, Transform tab)[]
        {
            (ItemTheme.Default, StoreItemCategory.Vase, Button_InteriorItem[0].transform.GetChild(1)), // Default 탭의 버튼 컨테이너
            (ItemTheme.Default, StoreItemCategory.Box, Button_InteriorItem[1].transform.GetChild(1)), // Sea 탭의 버튼 컨테이너
            (ItemTheme.Default, StoreItemCategory.Thread, Button_InteriorItem[2].transform.GetChild(1)), // Star 탭의 버튼 컨테이너
        };


        foreach (var (theme1, theme2, parent) in tabMappings)
        {
            List<string> themeIDs = storeInfoData.GetSortedIDsByTheme(theme1, theme2);
            int loopCount = Mathf.Min(parent.childCount, themeIDs.Count);

            for (int i = 0; i < loopCount; i++)
            {
                var btn = parent.GetChild(i).GetComponent<InteriorButton>();
                btn.SetButtonNumber(i);
                btn.SetButtonItemID(themeIDs[i]);
            }
        }
    }

    /*    public void UpdateButtonAdjusting(int itemIdx)
        {
            // 적용 중 UI 토글
            for (int i = 0; i < currentAdjusting.Length; i++)
            {
                interiorItemArray[i].transform.GetChild(1).gameObject
                    .SetActive(currentAdjusting[i]);
            }
        }*/

    public void UpdateButtonAdjusting()
    {
        for (int i = 0; i < interiorItemArray.Count; i++)
        {
            bool uiState = interiorItemArray[i].transform.GetChild(1).gameObject.activeSelf;
            bool dataState = interiorDataManager.dataList[i].isAdjusting;

            if (uiState != dataState)
                interiorItemArray[i].transform.GetChild(1).gameObject.SetActive(dataState);
        }
    }



    void LoadSaveData()
    {
        interiorDataManager = GameManager.instance.interiorDataManager;
        for (int i = 0; i < currentAdjusting.Length; i++)
        {
            currentAdjusting[i] = interiorDataManager.dataList[i].isAdjusting;
            if (currentAdjusting[i]) SettingItemHide(i);
        }
    }

    /// <summary>
    /// itemIdx: 버튼 배열 내 순서
    /// itemID: 실제 아이템 고유 ID
    /// </summary>
    public void UpdateInteriorImage(int itemIdx, string itemID)
    {

        int objectNum = ChangeItemIDToObjectNum(itemID);
        Debug.Log("objectNum : " + objectNum + ", itemID : " + itemID);
        var goodsImages = GetComponent<MainProducts>().goodsImages;
        var imageList = goodsImages[objectNum].imageList;
        Sprite spr = null;

        // 0~4 보조도구
        if (objectNum <= 4)
        {
            if (itemIdx < imageList.Length)
            {
                Debug.Log("여기 들어감");
                spr = imageList[itemIdx];
            }
            else
            {
                Debug.Log("인덱스 범위를 벗어남 : " + itemIdx + ", " + imageList.Length);
            }
        }
        else // 인테리어 아이템
        {
            ItemTheme theme = storeInfoData.GetThemeByID(itemID);
            int ti = ThemeToIndex(theme);
            if (ti >= 0 && ti < imageList.Length)
                spr = imageList[ti];
        }

        if (spr != null)
        {
            var go = GetComponent<MainProducts>().goodsContents[objectNum];
            go.GetComponent<Image>().sprite = spr;

            // 벽지(5번) 일 때만 추가 처리
            if (objectNum == 5)
                UpdateRelatedWallpaperImages(
                    storeInfoData.GetThemeByID(itemID));
        }
    }

    int ThemeToIndex(ItemTheme theme)
    {
        switch (theme)
        {
            case ItemTheme.Default: return 0;
            case ItemTheme.Sea: return 1;
            case ItemTheme.Star: return 2;
            default: return 0;
        }
    }

    void UpdateRelatedWallpaperImages(ItemTheme theme)
    {
        int ti = ThemeToIndex(theme);
        var mp = GetComponent<MainProducts>();
        for (int j = 5; j < 9; j++)
        {
            var imgs = mp.goodsImages[j].imageList;
            if (ti >= 0 && ti < imgs.Length)
                mp.goodsContents[j].GetComponent<Image>().sprite = imgs[ti];
        }
    }

    public int ChangeItemIDToObjectNum(string itemID)
    {
        // 아이디의 숫자 부분만 추출
        int itemIDNum = int.Parse(itemID.Split('_')[1]);

        if (itemIDNum < 4007) return 2;  // 꽃병
        if (itemIDNum < 4011) return 3;  // 인벤토리
        if (itemIDNum < 4016) return 4;  // 실
        if (itemIDNum < 4019) return 5;  // 벽지
        if (itemIDNum < 4022) return 9;  // 가랜드
        if (itemIDNum < 4025) return 10; // 창틀
        if (itemIDNum < 4028) return 11; // 패드
        if (itemIDNum < 4031) return 12; // 깃펜
        if (itemIDNum < 4034) return 13; // 스타드롭
        if (itemIDNum < 4037) return 14; // 수정구슬
        if (itemIDNum < 4040) return 15; // 망원경
        if (itemIDNum < 4042) return 16; // 오르골
        return -1;
    }

    public void SelectInteriorDafaultItem(int itemIdx, string itemID)
    {
        // 이 함수는 일단 기본 아이템을 클릭하면 작동하는 버튼 이벤트....
        // 클릭한 아이템을 인식해서 이미지 바꾸기를 실행하는 곳

        interiorDataManager = GameManager.instance.interiorDataManager;
        StoreItemCategory currentCategory = storeInfoData.GetCategoryByItemID(itemID);    // 아이템 ID로 선택한 아이템 카테고리 조회

        // 선택한 아이템 찾기
        var categoryItems = interiorDataManager.dataList
        .Where(d =>
        {
            var storeInfo = storeInfoData.dataList.FirstOrDefault(s => s.id == d.storeinfo_id);
            return storeInfo != null && storeInfo.category == currentCategory;
        })
        .ToList();
        

        var selectedData = categoryItems[itemIdx];
        int selectedIdx = 0;


        if (selectedData.isAdjusting)
        {
            // 이미 적용 중인 아이템을 다시 클릭 → 카테고리의 첫 아이템으로 되돌리기
            foreach (var d in categoryItems) d.isAdjusting = false;
            categoryItems[0].isAdjusting = true;

            selectedIdx = interiorDataManager.dataList.IndexOf(categoryItems[0]);
            itemID = categoryItems[0].storeinfo_id;
            itemIdx = 0; // 이미지도 첫 번째로 변경
        }
        else
        {
            // 같은 카테고리의 다른 아이템 해제
            foreach (var d in categoryItems) d.isAdjusting = false;
            selectedData.isAdjusting = true;
            selectedIdx = itemIdx;
        }

        // UI & 이미지 갱신
        currentAdjusting = interiorDataManager.dataList
            .Select(d => d.isAdjusting).ToArray();

        // id는 맞게 들어감. idx 수정 필요
        UpdateInteriorImage(selectedIdx, itemID);
        UpdateButtonAdjusting();
        SettingItemHide(selectedIdx);

        // 저장
        GameManager.instance.jsonManager
            .SaveData(Constants.InteriorDataFile, interiorDataManager);
    }

    public void SettingItemHide(int itemIdx)
    {
        var it = interiorDataManager.dataList[itemIdx];
        var bt = interiorItemArray[itemIdx].transform;
        var black = bt.Find("BlackImage")?.gameObject;
        if (black != null) black.SetActive(!it.isHaving);
    }

    /*    public string GetItemCategory(int itemIdx)
        {
            int itemId = interiorDataManager.dataList[itemIdx].id;
            var matchedItem = GameManager.instance.storeinfo_data.dataList.FirstOrDefault(data => data.id == itemId);
            return matchedItem != null ? matchedItem.category.ToString() : string.Empty;
        }*/

    public StoreItemCategory GetItemCategory(int itemIdx)
    {
        //if (itemIdx < 0 || itemIdx >= interiorDataManager.dataList.Count)
        //{
        //    Debug.LogWarning($"[GetItemCategory] 잘못된 itemIdx: {itemIdx}");
        //    return StoreItemCategory.Rack;
        //}

        string itemId = interiorDataManager.dataList[itemIdx].storeinfo_id;

        var matchedItem = GameManager.instance.storeinfo_data.dataList
            .FirstOrDefault(data => data.id == itemId);

        if (matchedItem == null)
        {
            Debug.LogWarning($"[GetItemCategory] itemId {itemId}에 해당하는 storeInfo 데이터를 찾을 수 없음");
            return StoreItemCategory.Rack;
        }

        return matchedItem.category; // enum 그대로 반환
    }
}

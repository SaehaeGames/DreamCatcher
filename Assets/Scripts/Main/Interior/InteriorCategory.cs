using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class InteriorCategory : MonoBehaviour
{
    /*
    스크립트 목적 : 인테리어 화면에서 카테고리 선택 및 카테고리 
    
    */
    public GameObject Panel_Interior;    GameObject ScrollViewPort;
    public GameObject[] Button_InteriorCategory;
    public GameObject[] Button_InteriorItem;
    public GameObject[] Button_SeaItem;
    public GameObject[] Button_StarItem;
    public GameObject[] horLines;

    private List<GameObject> interiorItemArray;
    private bool[] currentAdjusting;
    public int currentCategoryIndex;
    private InteriorDataManager interiorDataManager;
    private GoodsDataManager goodsDataManager;
    private StoreInfo_Data storeInfoData;

    void Start()
    {
        currentCategoryIndex = 0;               // 현재 카테고리 인덱스
        ScrollViewPort = Panel_Interior.transform.GetChild(2).GetChild(0).gameObject;
        for (int i = 0; i < Button_InteriorCategory.Length; i++)        // 버튼 번호 설정
            Button_InteriorCategory[i].GetComponent<InteriorButton>().buttonNumber = i;

        interiorDataManager = GameManager.instance.interiorDataManager;
        goodsDataManager = GameManager.instance.goodsDataManager;
        storeInfoData = GameManager.instance.storeinfo_data;


        GetBtnItemCount();
        currentAdjusting = new bool[interiorItemArray.Count];

        LoadSaveData();
        UpdateCatrgoryPanel(currentCategoryIndex);  // DefineButtonNumberAndID 호출됨

        // 보조도구 레벨 기반 isHaving 동기화 (버튼 ID 설정 이후에 호출)
        SyncDevelopmentItemsHaving();

        // 카테고리별 기본 적용 아이템이 없으면 첫 번째 보유 아이템을 자동 설정
        SyncDefaultAdjusting();

        // 보유/비보유 상태 반영
        for (int i = 0; i < interiorItemArray.Count; i++)
            SettingItemHide(i);

        // 적용중 표시 반영 (버튼 ID 설정 이후에 호출)
        UpdateButtonAdjusting();

        // 적용중 아이템 메인 화면 이미지 반영 - MainProducts.Start()가 이미지를 리셋하므로 1프레임 뒤에 실행
        StartCoroutine(ApplyAdjustingImagesDelayed());
    }

    private void SyncDefaultAdjusting()
    {
        // 각 Default 카테고리에서 적용중 아이템이 없으면 첫 번째 보유 아이템을 자동으로 적용중으로 설정
        var cats = new StoreItemCategory[] { StoreItemCategory.Vase, StoreItemCategory.Box, StoreItemCategory.Thread };

        bool changed = false;
        foreach (var cat in cats)
        {
            var categoryItems = interiorDataManager.dataList
                .Where(d =>
                {
                    var si = storeInfoData.dataList.FirstOrDefault(s => s.id == d.storeinfo_id);
                    return si != null && si.category == cat && si.theme == ItemTheme.Default;
                })
                .ToList();

            if (categoryItems.Any(d => d.isAdjusting)) continue;

            var firstOwned = categoryItems.FirstOrDefault(d => d.isHaving);
            if (firstOwned != null)
            {
                firstOwned.isAdjusting = true;
                changed = true;
            }
        }

        if (changed)
            GameManager.instance.jsonManager.SaveData(Constants.InteriorDataFile, interiorDataManager);
    }

    private void SyncDevelopmentItemsHaving()
    {
        // goodsData 레벨에 따라 인테리어 아이템 isHaving 자동 동기화
        var devCats = new (string name, StoreItemCategory cat)[]
        {
            (Constants.GoodsData_Vase, StoreItemCategory.Vase),
            (Constants.GoodsData_Box, StoreItemCategory.Box),
            (Constants.GoodsData_Thread, StoreItemCategory.Thread),
        };

        foreach (var (name, cat) in devCats)
        {
            GoodsData gd = goodsDataManager.GetGoodsDataByCategory(name);
            int ownedLevel = gd != null ? gd.level : 0;

            foreach (var si in storeInfoData.dataList)
            {
                if (si.category != cat || si.theme != ItemTheme.Default) continue;
                if (si.level > ownedLevel) continue;

                InteriorData interior = interiorDataManager.GetInteriorDataByStoreInfoId(si.id);
                if (interior != null)
                    interior.isHaving = true;
            }
        }
    }

    private IEnumerator ApplyAdjustingImagesDelayed()
    {
        yield return null; // MainProducts.Start()가 이미지를 리셋한 뒤에 적용
        ApplyAdjustingImages();
    }

    private void ApplyAdjustingImages()
    {
        for (int i = 0; i < interiorItemArray.Count; i++)
        {
            var btn = interiorItemArray[i].GetComponent<InteriorButton>();
            if (btn == null || string.IsNullOrEmpty(btn.itemID)) continue;

            InteriorData data = interiorDataManager.GetInteriorDataByStoreInfoId(btn.itemID);
            if (data != null && data.isAdjusting)
                UpdateInteriorImage(btn.buttonNumber, btn.itemID);
        }
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
        foreach (var btn in Button_SeaItem)
            if (btn != null && btn.GetComponent<InteriorButton>() != null)
                interiorItemArray.Add(btn);
        foreach (var btn in Button_StarItem)
            if (btn != null && btn.GetComponent<InteriorButton>() != null)
                interiorItemArray.Add(btn);
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
        // Default 탭: Vase / Box / Thread
        var tabMappings = new (ItemTheme theme1, StoreItemCategory theme2, Transform tab)[]
        {
            (ItemTheme.Default, StoreItemCategory.Vase,   Button_InteriorItem[0].transform.GetChild(1)),
            (ItemTheme.Default, StoreItemCategory.Box,    Button_InteriorItem[1].transform.GetChild(1)),
            (ItemTheme.Default, StoreItemCategory.Thread, Button_InteriorItem[2].transform.GetChild(1)),
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

        // Sea 탭
        List<string> seaIDs = storeInfoData.GetSortedIDsByTheme(ItemTheme.Sea);
        for (int i = 0; i < Mathf.Min(Button_SeaItem.Length, seaIDs.Count); i++)
        {
            var btn = Button_SeaItem[i]?.GetComponent<InteriorButton>();
            if (btn == null) continue;
            btn.SetButtonNumber(i);
            btn.SetButtonItemID(seaIDs[i]);
        }

        // Star 탭
        List<string> starIDs = storeInfoData.GetSortedIDsByTheme(ItemTheme.Star);
        for (int i = 0; i < Mathf.Min(Button_StarItem.Length, starIDs.Count); i++)
        {
            var btn = Button_StarItem[i]?.GetComponent<InteriorButton>();
            if (btn == null) continue;
            btn.SetButtonNumber(i);
            btn.SetButtonItemID(starIDs[i]);
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
            var btn = interiorItemArray[i].GetComponent<InteriorButton>();
            if (btn == null || string.IsNullOrEmpty(btn.itemID)) continue;

            InteriorData data = interiorDataManager.GetInteriorDataByStoreInfoId(btn.itemID);
            bool dataState = data != null && data.isAdjusting;

            bool uiState = interiorItemArray[i].transform.GetChild(1).gameObject.activeSelf;
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
        if (itemIDNum <= 4042) return 16; // 오르골
        return -1;
    }

    public void SelectInteriorDafaultItem(int itemIdx, string itemID)
    {
        interiorDataManager = GameManager.instance.interiorDataManager;
        StoreItemCategory currentCategory = storeInfoData.GetCategoryByItemID(itemID);

        // 같은 카테고리의 InteriorData 목록
        var categoryItems = interiorDataManager.dataList
            .Where(d =>
            {
                var si = storeInfoData.dataList.FirstOrDefault(s => s.id == d.storeinfo_id);
                return si != null && si.category == currentCategory;
            })
            .ToList();

        // itemID로 직접 selectedData 조회 (인덱스 불일치 방지)
        var selectedData = categoryItems.FirstOrDefault(d => d.storeinfo_id == itemID);
        if (selectedData == null) return;

        int selectedIdx = categoryItems.IndexOf(selectedData);

        if (selectedData.isAdjusting)
        {
            // 이미 적용 중 → 카테고리의 Default 아이템으로 되돌리기
            foreach (var d in categoryItems) d.isAdjusting = false;

            // Default 테마 아이템이 categoryItems 안에 있으면 첫 번째 사용
            var defaultItem = categoryItems.FirstOrDefault(d =>
            {
                var si = storeInfoData.dataList.FirstOrDefault(s => s.id == d.storeinfo_id);
                return si != null && si.theme == ItemTheme.Default;
            });

            if (defaultItem != null)
            {
                defaultItem.isAdjusting = true;
                itemID = defaultItem.storeinfo_id;
                selectedIdx = categoryItems.IndexOf(defaultItem);
            }
            else
            {
                // Star/Sea 전용 카테고리 → storeinfo에서 Default 테마 ID 찾아 이미지 복원
                var defaultSI = storeInfoData.dataList.FirstOrDefault(s =>
                    s.category == currentCategory && s.theme == ItemTheme.Default);
                if (defaultSI != null)
                {
                    itemID = defaultSI.id;
                    selectedIdx = 0;
                }
            }
        }
        else
        {
            // 같은 카테고리의 다른 아이템 해제 후 선택
            foreach (var d in categoryItems) d.isAdjusting = false;
            selectedData.isAdjusting = true;
        }

        currentAdjusting = interiorDataManager.dataList.Select(d => d.isAdjusting).ToArray();
        UpdateInteriorImage(selectedIdx, itemID);
        UpdateButtonAdjusting();
        GameManager.instance.jsonManager.SaveData(Constants.InteriorDataFile, interiorDataManager);
    }

    public void SettingItemHide(int itemIdx)
    {
        if (itemIdx < 0 || itemIdx >= interiorItemArray.Count) return;

        var btn = interiorItemArray[itemIdx].GetComponent<InteriorButton>();
        if (btn == null || string.IsNullOrEmpty(btn.itemID)) return;

        InteriorData it = interiorDataManager.GetInteriorDataByStoreInfoId(btn.itemID);
        if (it == null) return;

        var bt = interiorItemArray[itemIdx].transform;
        if (bt.childCount > 2)
            bt.GetChild(2).gameObject.SetActive(!it.isHaving);

        // 미보유 아이템은 클릭 불가
        var button = interiorItemArray[itemIdx].GetComponent<Button>();
        if (button != null)
            button.interactable = it.isHaving;
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

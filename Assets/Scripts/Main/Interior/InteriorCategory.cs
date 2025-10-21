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

        // �ʱ�ȭ & ���� ���� ������ UI �ݿ�
        for (int i = 0; i < interiorItemArray.Count; i++)
        {
            currentAdjusting[i] = interiorDataManager.dataList[i].isAdjusting;
            if (currentAdjusting[i])
            {
                int id = interiorDataManager.dataList[i].id;
                UpdateInteriorImage(i, id);
                UpdateButtonAdjusting();
            }
        }

        // ����/���� ���� �ݿ�
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
        // �� �ǿ� �����Ǵ� Theme�� �г�
        var tabMappings = new (ItemTheme theme1, StoreItemCategory theme2, Transform tab)[]
        {
            (ItemTheme.Default, StoreItemCategory.Vase, Button_InteriorItem[0].transform.GetChild(1)), // Default ���� ��ư �����̳�
            (ItemTheme.Default, StoreItemCategory.Box, Button_InteriorItem[1].transform.GetChild(1)), // Sea ���� ��ư �����̳�
            (ItemTheme.Default, StoreItemCategory.Thread, Button_InteriorItem[2].transform.GetChild(1)), // Star ���� ��ư �����̳�
        };


        foreach (var (theme1, theme2, parent) in tabMappings)
        {
            List<int> themeIDs = storeInfoData.GetSortedIDsByTheme(theme1, theme2);
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
            // ���� �� UI ���
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
    /// itemIdx: ��ư �迭 �� ����
    /// itemID: ���� ������ ���� ID
    /// </summary>
    public void UpdateInteriorImage(int itemIdx, int itemID)
    {

        int objectNum = ChangeItemIDToObjectNum(itemID);
        Debug.Log("objectNum : " + objectNum + ", itemID : " + itemID);
        var goodsImages = GetComponent<MainProducts>().goodsImages;
        var imageList = goodsImages[objectNum].imageList;
        Sprite spr = null;

        // 0~4 ��������
        if (objectNum <= 4)
        {
            if (itemIdx < imageList.Length)
            {
                Debug.Log("���� ��");
                spr = imageList[itemIdx];
            }
            else
            {
                Debug.Log("�ε��� ������ ��� : " + itemIdx + ", " + imageList.Length);
            }
        }
        else // ���׸��� ������
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

            // ����(5��) �� ���� �߰� ó��
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

    public int ChangeItemIDToObjectNum(int itemID)
    {
        if (itemID < 4007) return 2;  // �ɺ�
        if (itemID < 4011) return 3;  // �κ��丮
        if (itemID < 4016) return 4;  // ��
        if (itemID < 4019) return 5;  // ����
        if (itemID < 4022) return 9;  // ������
        if (itemID < 4025) return 10; // âƲ
        if (itemID < 4028) return 11; // �е�
        if (itemID < 4031) return 12; // ����
        if (itemID < 4034) return 13; // ��Ÿ���
        if (itemID < 4037) return 14; // ��������
        if (itemID < 4040) return 15; // ������
        if (itemID < 4042) return 16; // ������
        return -1;
    }

    public void SelectInteriorDafaultItem(int itemIdx, int itemID)
    {
        // �� �Լ��� �ϴ� �⺻ �������� Ŭ���ϸ� �۵��ϴ� ��ư �̺�Ʈ....
        // Ŭ���� �������� �ν��ؼ� �̹��� �ٲٱ⸦ �����ϴ� ��

        interiorDataManager = GameManager.instance.interiorDataManager;
        StoreItemCategory currentCategory = storeInfoData.GetCategoryByItemID(itemID);    // ������ ID�� ������ ������ ī�װ� ��ȸ

        // ������ ������ ã��
        var categoryItems = interiorDataManager.dataList
        .Where(d =>
        {
            var storeInfo = storeInfoData.dataList.FirstOrDefault(s => s.id == d.id);
            return storeInfo != null && storeInfo.category == currentCategory;
        })
        .ToList();
        

        var selectedData = categoryItems[itemIdx];
        int selectedIdx = 0;


        if (selectedData.isAdjusting)
        {
            // �̹� ���� ���� �������� �ٽ� Ŭ�� �� ī�װ��� ù ���������� �ǵ�����
            foreach (var d in categoryItems) d.isAdjusting = false;
            categoryItems[0].isAdjusting = true;

            selectedIdx = interiorDataManager.dataList.IndexOf(categoryItems[0]);
            itemID = categoryItems[0].id;
            itemIdx = 0; // �̹����� ù ��°�� ����
        }
        else
        {
            // ���� ī�װ��� �ٸ� ������ ����
            foreach (var d in categoryItems) d.isAdjusting = false;
            selectedData.isAdjusting = true;
            selectedIdx = itemIdx;
        }

        // UI & �̹��� ����
        currentAdjusting = interiorDataManager.dataList
            .Select(d => d.isAdjusting).ToArray();

        // id�� �°� ��. idx ���� �ʿ�
        UpdateInteriorImage(selectedIdx, itemID);
        UpdateButtonAdjusting();
        SettingItemHide(selectedIdx);

        // ����
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
        if (itemIdx < 0 || itemIdx >= interiorDataManager.dataList.Count)
        {
            Debug.LogWarning($"[GetItemCategory] �߸��� itemIdx: {itemIdx}");
            return StoreItemCategory.Rack;
        }

        int itemId = interiorDataManager.dataList[itemIdx].id;

        var matchedItem = GameManager.instance.storeinfo_data.dataList
            .FirstOrDefault(data => data.id == itemId);

        if (matchedItem == null)
        {
            Debug.LogWarning($"[GetItemCategory] itemId {itemId}�� �ش��ϴ� storeInfo �����͸� ã�� �� ����");
            return StoreItemCategory.Rack;
        }

        return matchedItem.category; // enum �״�� ��ȯ
    }
}

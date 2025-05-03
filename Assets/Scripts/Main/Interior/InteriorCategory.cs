using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class InteriorCategory : MonoBehaviour
{
    // 인테리어 카테고리 스크립트

    public GameObject Panel_Interior; // 인테리어 패널
    GameObject ScrollViewPort;  // 카테고리 뷰 포트
    public GameObject[] Button_InteriorCategory;   //인테리어 패널 버튼
    public GameObject[] Button_InteriorItem;    //인테리어 아이템 버튼

    public GameObject[] horLines;   //인테리어 UI의 카테고리 세로선 배열

    private List<GameObject> interiorItemArray; //인테리어 아이템 버튼 목록
    private bool[] currentAdjusting;  //현재 적용중인 아이템 여부
    public int currentCategoryIndex;   //현재 활성화중인 인테리어 카테고리

    private InteriorDataManager interiorDataManager;  //현재 플레이어 인테리어 데이터 정보

    public void Start()
    {
        currentCategoryIndex = 0;
        ScrollViewPort = Panel_Interior.gameObject.transform.GetChild(2).GetChild(0).gameObject;  
        for (int i = 0; i < Button_InteriorCategory.Length; i++)
        {
            Button_InteriorCategory[i].GetComponent<InteriorButton>().buttonNumber = i;
        }

        interiorDataManager = GameManager.instance.interiorDataManager;
        currentAdjusting = new bool[GetBtnItemCount()];

        //SetAllItemsHavingTrue();
        // isAdjusting 상태를 초기화하고 UI 업데이트
        for (int i = 0; i < interiorItemArray.Count; i++)
        {
            currentAdjusting[i] = interiorDataManager.dataList[i].isAdjusting;
            if (currentAdjusting[i])
            {
                UpdateInteriorImage(i, i);
                UpdateButtonAdjusting(i, i); // 적용 중 UI 업데이트
            }
        }

        for (int i = 0; i < interiorItemArray.Count; i++)
        {
            SettingItemHide(i);
        }

        DefineButtonNumber();   //카테고리 버튼 고유 번호 지정
        LoadSaveData(); // 저장 데이터 가져옴
        UpdateCatrgoryPanel(currentCategoryIndex); //카테고리 번호에 맞게 노출되는 패널 업데이트

    }

    public void UpdateCatrgoryPanel(int panelIdx)
    {
        //인테리어 카테고리 버튼 업데이트 함수)

        OpenIndexPanel(panelIdx);
        SetHorLine(panelIdx);
    }

    public void OpenIndexPanel(int idx)
    {
        //해당하는 인덱스의 인테리어 패널을 활성화하는 함수

        int panelCnt = ScrollViewPort.gameObject.transform.childCount;   //패널 오브젝트 개수
        for (int i = 0; i < panelCnt; i++)  //패널 오브젝트 개수만큼 반복
        {
            if (i == idx)
            {
                ScrollViewPort.gameObject.transform.GetChild(i).gameObject.SetActive(true);    //해당하는 인덱스의 패널 활성화
            }
            else
            {
                ScrollViewPort.gameObject.transform.GetChild(i).gameObject.SetActive(false);    //패널 비활성화
            }
        }
    }

    public void SetHorLine(int curCateNum)
    {
        //현재 실행중인 탭에 따라 세로선 배치하는 함수
        //나중엔 이차원 배열로 한 번에 적용되게 해야하나?

        switch (curCateNum)
        {
            case 0:
                horLines[0].SetActive(false);
                horLines[1].SetActive(true);
                horLines[2].SetActive(true);
                break;
            case 1:
                horLines[0].SetActive(true);
                horLines[1].SetActive(false);
                horLines[2].SetActive(true);
                break;
            case 2:
                horLines[0].SetActive(true);
                horLines[1].SetActive(true);
                horLines[2].SetActive(false);
                break;
            default:
                horLines[0].SetActive(false);
                horLines[1].SetActive(true);
                horLines[2].SetActive(true);
                break;
        }
    }

    public int GetBtnItemCount()
    {
        interiorItemArray = new List<GameObject>();
        for (int i = 0; i < Button_InteriorItem.Length; i++)
        {
            for (int j = 0; j < Button_InteriorItem[i].transform.GetChild(1).childCount; j++)
            {
                var child = Button_InteriorItem[i].transform.GetChild(1).GetChild(j).gameObject;
                if (child.GetComponent<InteriorButton>() != null)
                {
                    interiorItemArray.Add(child);
                }
            }
        }

        return interiorItemArray.Count;
    }

    public void DefineButtonNumber()
    {
        // 인테리어 버튼들에 고유 번호를 부여하는 함수

        // storeInfo에서 버튼 id를 가져옴
        StoreInfo_Data storeInfo_data = GameManager.instance.storeinfo_data;

        List<int> defaultItemIDList = storeInfo_data.GetSortedIDsByTheme(ItemTheme.Default);
        List<int> SeaItemIDList = storeInfo_data.GetSortedIDsByTheme(ItemTheme.Sea);
        List<int> StarItemIDList = storeInfo_data.GetSortedIDsByTheme(ItemTheme.Star);

        List<int> combinedItemIDList = new List<int>();
        combinedItemIDList.AddRange(defaultItemIDList);
        combinedItemIDList.AddRange(SeaItemIDList);
        combinedItemIDList.AddRange(StarItemIDList);

        int itemNumber = 0;

        /*for (int i = 0; i < Button_InteriorCategory.Length; i++)
        {
            Button_InteriorCategory[i].gameObject.GetComponent<InteriorButton>().SetButtonNumber(i);
        }*/

        for (int j = 0; j < Button_InteriorItem.Length; j++)
        {
            //버튼 이벤트 설정

            int itemsIndex = Button_InteriorItem[j].transform.childCount;
            for (int k = 0; k < Button_InteriorItem[j].transform.GetChild(itemsIndex - 1).childCount; k++)
            {
                var button = Button_InteriorItem[j].transform.GetChild(itemsIndex - 1).GetChild(k).gameObject.GetComponent<InteriorButton>();
                button.SetButtonNumber(itemNumber);
                button.SetButtonItemID(combinedItemIDList[itemNumber]);
                itemNumber++;
            }
        }
    }


    public string CheckItemCategory2(int itemIdx)
    {
        // 데이터 테이블에서 아이템의 카테고리2를 가져오는 함수

        int itemId = interiorDataManager.dataList[itemIdx].id;
        var matchedItem = GameManager.instance.storeinfo_data.dataList.FirstOrDefault(data => data.id == itemId);
        return matchedItem != null ? matchedItem.category.ToString() : string.Empty;
    }

    public void UpdateButtonAdjusting(int itemIdx, int imgIdx)
    {
        // 현재 적용중인 아이템에 적용중 표시하는 함수

        for (int j = 0; j < currentAdjusting.Length; j++)
        {
            //적용중인 아이템만 적용중 표시
            bool curAdjusting = currentAdjusting[j];
            int itemsIndex = interiorItemArray[j].transform.childCount;
            if (curAdjusting)
            {
                interiorItemArray[j].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                interiorItemArray[j].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void LoadSaveData()
    {
        interiorDataManager = GameManager.instance.interiorDataManager;
        StoreInfo_Data storeInfo_Data = GameManager.instance.storeinfo_data;

        for (int i = 0; i < currentAdjusting.Length; i++)
        {
            if (i >= interiorDataManager.dataList.Count)
            {
                Debug.LogError($"[ERROR] 인덱스 {i}가 존재하지 않음.");
                break;
            }

            currentAdjusting[i] = interiorDataManager.dataList[i].isAdjusting;

            if (currentAdjusting[i])
            {
                SettingItemHide(i); // ✅ 보유 상태 반영
            }
        }
    }

    public void UpdateInteriorImage(int itemIdx, int imgIdx)
    {
        // 가구 이미지를 업데이트하는 함수

        int objectNum = ChangeItemIdxToObjectNum(itemIdx);  // 아이템 인덱스를 카테고리 번호로 변경
        var goodsImages = this.GetComponent<MainProducts>().goodsImages;
        var imageList = goodsImages[objectNum].imageList;

        // 기본 이미지 설정
        GameObject itemObj = this.GetComponent<MainProducts>().goodsContents[objectNum].gameObject;
        itemObj.GetComponent<Image>().sprite = imageList[imgIdx];

        // Wallpaper의 경우
        if (objectNum == 5) // Wallpaper의 objectNum이 5라고 지정
        {
            UpdateRelatedWallpaperImages(imgIdx);
        }
    }

    private void UpdateRelatedWallpaperImages(int imgIdx)
    {
        var mainProducts = this.GetComponent<MainProducts>();
        for (int j = 5; j < 9; j++) // Wallpaper 관련 오브젝트들
        {
            var relatedImageList = mainProducts.goodsImages[j].imageList;
            if (imgIdx >= 0 && imgIdx < relatedImageList.Length)
            {
                mainProducts.goodsContents[j].GetComponent<Image>().sprite = relatedImageList[imgIdx];
            }
        }
    }

    public int ChangeItemIdxToObjectNum(int itemIdx)
    {
        // 아이템 인덱스 정보로 오브젝트 정보를 반환하는 함수
        Debug.Log("인덱스 " + itemIdx + "를 변경함");

        if (itemIdx >= 0 && itemIdx < 4) return 2;  // 꽃병
        if (itemIdx >= 4 && itemIdx < 8) return 3;  // 인벤토리
        if (itemIdx >= 8 && itemIdx < 13) return 4; // 실
        if (itemIdx == 13 || itemIdx == 22) return 5; // 벽지
        if (itemIdx == 14 || itemIdx == 23) return 9; // 가랜드
        if (itemIdx == 15 || itemIdx == 24) return 10; // 창틀
        if (itemIdx == 16 || itemIdx == 25) return 11; // 패드
        if (itemIdx == 17 || itemIdx == 26) return 12; // 깃펜
        if (itemIdx == 18 || itemIdx == 27) return 13; // 스타드롭
        if (itemIdx == 19 || itemIdx == 28) return 14; // 수정구슬
        if (itemIdx == 20 || itemIdx == 29) return 15; // 망원경
        if (itemIdx == 21 || itemIdx == 30) return 16; // 오르골
        return -1; // 유효하지 않은 경우
    }

    public void SelectInteriorItem(int itemIdx, int itemID)
    {
        interiorDataManager = GameManager.instance.interiorDataManager;
        var selectedItem = interiorDataManager.dataList[itemIdx];
        string category = CheckItemCategory2(itemIdx);
        ItemTheme selectedTheme = GameManager.instance.storeinfo_data.GetThemeByID(selectedItem.id);

        // ✅ 적용 해제 로직 개선
        if (currentAdjusting[itemIdx])
        {
            int defaultIdx = -1;

            // ✅ 특정 카테고리 (꽃병, 상자, 실, 벽지)의 기본 아이템 찾기
            if (category == "Vase" || category == "Box" || category == "Thread" || category == "Wallpaper")
            {
                var defaultItem = interiorDataManager.dataList.FirstOrDefault(item =>
                     CheckItemCategory2(interiorDataManager.dataList.IndexOf(item)) == category &&
                     GameManager.instance.storeinfo_data.GetThemeByID(item.id) == ItemTheme.Default);

                if (defaultItem != null)
                {
                    defaultIdx = interiorDataManager.dataList.IndexOf(defaultItem);

                    if (itemIdx == defaultIdx)
                    {
                        return; // 기본 아이템이면 해제하지 않음
                    }

                    UpdateInteriorImage(itemIdx, defaultIdx);
                }

                // ✅ 같은 카테고리의 모든 아이템을 해제
                foreach (var item in interiorDataManager.dataList)
                {
                    int index = interiorDataManager.dataList.IndexOf(item);
                    if (CheckItemCategory2(index) == category && index != defaultIdx)
                    {
                        currentAdjusting[index] = false;
                        item.isAdjusting = false;
                    }
                }
            }
            else
            {
                // ✅ 일반 아이템 해제 로직
                currentAdjusting[itemIdx] = false;
                selectedItem.isAdjusting = false;
                UpdateInteriorImage(itemIdx, 0);
            }
        }
        else
        {
            // ✅ 같은 카테고리의 기존 아이템을 해제하고 새로운 아이템 적용
            var sameCategoryItem = interiorDataManager.dataList.FirstOrDefault(item =>
                CheckItemCategory2(interiorDataManager.dataList.IndexOf(item)) == category &&
                currentAdjusting[interiorDataManager.dataList.IndexOf(item)]);

            if (sameCategoryItem != null)
            {
                int sameCategoryItemIdx = interiorDataManager.dataList.IndexOf(sameCategoryItem);
                currentAdjusting[sameCategoryItemIdx] = false;
                sameCategoryItem.isAdjusting = false;
            }

            // ✅ 새 아이템 적용
            currentAdjusting[itemIdx] = true;
            selectedItem.isAdjusting = true;

            // ✅ Wallpaper의 경우 책상과 WindowFrame도 함께 변경
            if (category == "Wallpaper")
            {
                foreach (var relatedCategory in new[] { "Wood", "WindowFrame" })
                {
                    var relatedItem = interiorDataManager.dataList.FirstOrDefault(item =>
                        CheckItemCategory2(interiorDataManager.dataList.IndexOf(item)) == relatedCategory &&
                        GameManager.instance.storeinfo_data.GetThemeByID(item.id) == selectedTheme);

                    if (relatedItem != null)
                    {
                        int relatedIdx = interiorDataManager.dataList.IndexOf(relatedItem);
                        currentAdjusting[relatedIdx] = true;
                        relatedItem.isAdjusting = true;
                    }
                }
            }
        }

        // 데이터 저장
        GameManager.instance.interiorDataManager = interiorDataManager;
        GameManager.instance.jsonManager.SaveData(Constants.InteriorDataFile, interiorDataManager);
    }
    public void SettingItemHide(int itemIdx)
    {
        var item = interiorDataManager.dataList[itemIdx];

        Transform buttonTransform = interiorItemArray[itemIdx].transform;
        GameObject blackImage = buttonTransform.GetChild(buttonTransform.childCount - 1).gameObject;

        if (item.isHaving)
        {
            blackImage.SetActive(false);
            interiorItemArray[itemIdx].GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else
        {
            blackImage.SetActive(true);
            interiorItemArray[itemIdx].GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }

    // 테스트용: 모든 아이템의 isHaving 값을 true로 설정하는 함수
    public void SetAllItemsHavingTrue()
    {
        foreach (var item in interiorDataManager.dataList)
        {
            item.isHaving = true;
        }
    }


}


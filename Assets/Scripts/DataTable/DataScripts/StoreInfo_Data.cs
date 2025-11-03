using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using UnityEditor;
using System.Linq;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/StoreInfo")]
public class StoreInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1Kfo_r8_sHcCpR3YIue9AwwLu6eY7IMGYuRwH7hwPPus";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:I";
    private static string objectName = "StoreInfo";

    public List<StoreInfo_Object> dataList = new List<StoreInfo_Object>();

    public void UpdateStoreInfoData(Action onUpdateComplete)
    {
        // StoreInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<StoreInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // 변동사항 저장
            onUpdateComplete?.Invoke(); //onUpdateComplete 콜백 호출
        });
    }

    public void InitializeStoreInfoData()
    {
        // StoreInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<StoreInfo_Data>(CreateInstance<StoreInfo_Data>(), objectName);
    }

    public List<string> GetSortedIDsByTheme(ItemTheme keyword)
    {
        // theme 키워드를 검색해서 id 리스트를 반환하는 함수

        return dataList
            .Where(storeItem => storeItem.theme == keyword && storeItem.isButtonActive == true)
            .Select(storeItem => storeItem.id)
            .ToList();
    }
    public List<string> GetSortedIDsByTheme(StoreItemCategory keyword)
    {
        // theme 키워드를 검색해서 id 리스트를 반환하는 함수

        return dataList
            .Where(storeItem => storeItem.category == keyword && storeItem.isButtonActive == true)
            .Select(storeItem => storeItem.id)
            .ToList();
    }

    public List<string> GetSortedIDsByTheme(ItemTheme keyword1, StoreItemCategory keyword2)
    {
        // theme 키워드를 검색해서 id 리스트를 반환하는 함수

        Debug.Log("keyword1 : " + keyword1 + "keyword2: " +  keyword2);

        return dataList
            .Where(storeItem => storeItem.theme == keyword1 && storeItem.category == keyword2 && storeItem.isButtonActive == true)
            .Select(storeItem => storeItem.id)
            .ToList();
    }

    public StoreItemCategory GetCategoryByItemID(string itemID)
    {
        var matchedItem = dataList.FirstOrDefault(storeItem => storeItem.id == itemID);
        Debug.Log(matchedItem.category);
        return matchedItem.category;
    }

    public int GetLevelByID(string id)
    {
        var result = dataList
            .Where(storeItem => storeItem.id == id)
            .Select(storeItem => storeItem.level)
            .FirstOrDefault();

        return result;
    }

    public string GetIDByCategoryAndLevel(string category, int level)
    {
        var matchingItems = dataList
            .Where(storeItem => storeItem.category.ToString().Trim().Equals(category.Trim(), StringComparison.OrdinalIgnoreCase)
                                && storeItem.level == level)
            .ToList();

        if (matchingItems.Count == 1)
        {
            return matchingItems[0].id;
        }
        else if (matchingItems.Count > 1)
        {
            Debug.LogError($"[ERROR] {category}에 해당하는 여러 개의 아이템이 존재합니다! (레벨 {level})");
            return matchingItems[0].id;  // 첫 번째 ID 반환 (임시)
        }
        else
        {
            Debug.LogError($"[ERROR] {category}에 해당하는 아이템을 찾을 수 없음! (레벨 {level})");
            return ""; // 에러 처리
        }
    }


    public ItemTheme GetThemeByID(string id)
    {
        var result = dataList
            .Where(storeItem => storeItem.id == id)
            .Select(storeItem => storeItem.theme)
            .FirstOrDefault();

        return result;
    }

    public string GetContentsByID(string id)
    {
        var result = dataList
            .Where(storeItem => storeItem.id == id)
            .Select(storeItem => storeItem.contents)
            .FirstOrDefault();

        return result;
    }

    public string GetEffectByID(string id)
    {
        var result = dataList
            .Where(storeItem => storeItem.id == id)
            .Select(storeItem => storeItem.effect)
            .FirstOrDefault();

        return result;
    }

    public int GetGoldByID(string id)
    {
        var result = dataList
            .Where(storeItem => storeItem.id == id)
            .Select(storeItem => storeItem.gold)
            .FirstOrDefault();

        return result;
    }
}

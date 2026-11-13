using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

/// <summary>현재 판매하는 항목을 동일한 ID 순서로 반환하여 버튼과 표시 목록을 일치시킵니다.</summary>
    public List<StoreInfo_Object> GetPurchasableItems(ItemTheme theme)
    {
        return GetItemsByTheme(theme).FindAll(item => item.isButtonActive);
    }

    /// <summary>기존 UI 연결을 구성할 때는 비활성 상품도 포함하여 뒤 상품의 아이콘이 밀리지 않게 합니다.</summary>
    public List<StoreInfo_Object> GetItemsByTheme(ItemTheme theme)
    {
        var result = new List<StoreInfo_Object>();
        foreach (StoreInfo_Object item in dataList)
            if (item != null && item.theme == theme) result.Add(item);
        result.Sort((left, right) => StringComparer.Ordinal.Compare(left.id, right.id));
        return result;
    }

    /// <summary>없는 ID와 중복 ID를 정상 상품으로 취급하지 않습니다.</summary>
    public bool TryGetItem(string id, out StoreInfo_Object item)
    {
        item = null;
        if (string.IsNullOrWhiteSpace(id)) return false;
        foreach (StoreInfo_Object candidate in dataList)
        {
            if (candidate == null || candidate.id != id) continue;
            if (item != null)
            {
                item = null;
                return false;
            }
            item = candidate;
        }
        return item != null;
    }

    public List<string> GetSortedIDsByTheme(ItemTheme keyword)
    {
        return GetPurchasableItems(keyword).Select(item => item.id).ToList();
    }

    public List<string> GetSortedIDsByTheme(StoreItemCategory keyword)
    {
        return dataList.Where(item => item != null && item.category == keyword && item.isButtonActive)
            .OrderBy(item => item.id, StringComparer.Ordinal).Select(item => item.id).ToList();
    }

    public List<string> GetSortedIDsByTheme(ItemTheme theme, StoreItemCategory category)
    {
        return GetPurchasableItems(theme).Where(item => item.category == category).Select(item => item.id).ToList();
    }

    public StoreItemCategory GetCategoryByItemID(string id)
    {
        if (TryGetItem(id, out StoreInfo_Object item)) return item.category;
        Debug.LogError("[StoreInfo_Data] 유효한 상품 ID가 아닙니다: " + id);
        return default;
    }

    public int GetLevelByID(string id) => TryGetItem(id, out StoreInfo_Object item) ? item.level : 0;

    /// <summary>보조도구의 기본 테마만 조회합니다. 다음 레벨이 없는 품절은 정상 결과입니다.</summary>
    public string GetIDByCategoryAndLevel(string category, int level)
    {
        if (string.IsNullOrWhiteSpace(category)) return "";
        StoreInfo_Object result = null;
        foreach (StoreInfo_Object item in dataList)
        {
            if (item == null || item.theme != ItemTheme.Default || item.level != level
                || !string.Equals(item.category.ToString(), category.Trim(), StringComparison.OrdinalIgnoreCase)) continue;
            if (result != null)
            {
                Debug.LogError("[StoreInfo_Data] 동일 카테고리와 레벨의 상품이 중복됩니다: " + category);
                return "";
            }
            result = item;
        }
        return result != null ? result.id : "";
    }

    public ItemTheme GetThemeByID(string id) => TryGetItem(id, out StoreInfo_Object item) ? item.theme : default;
    public string GetContentsByID(string id) => TryGetItem(id, out StoreInfo_Object item) ? item.contents : null;
    public string GetEffectByID(string id) => TryGetItem(id, out StoreInfo_Object item) ? item.effect : null;
    public int GetGoldByID(string id) => TryGetItem(id, out StoreInfo_Object item) ? item.gold : 0;
}

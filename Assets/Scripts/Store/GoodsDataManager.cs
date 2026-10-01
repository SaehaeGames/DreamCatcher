using System;
using System.Collections.Generic;

[Serializable]
public class GoodsData
{
    public string id;
    public string name;
    public string category;
    public int level;
    public string interior_id;

    public GoodsData()
    {
        id = "JS_0000";
        name = category = "";
        interior_id = "SO_0000";
    }

    public GoodsData(string _id, string _name, string _category, int _level, string _interior_id)
    {
        id = _id;
        name = _name;
        category = _category;
        level = _level;
        interior_id = _interior_id;
    }
}

/// <summary>보조도구의 레벨 저장과 조회, 구매할 다음 레벨의 검증을 담당합니다.</summary>
public class GoodsDataManager
{
    public List<GoodsData> dataList = new List<GoodsData>();
    private readonly JsonManager jsonManager = new JsonManager();

    public void ResetData()
    {
        dataList = jsonManager.LoadDefaultData<GoodsDataManager>(Constants.GoodsDataFile).dataList
            ?? new List<GoodsData>();
        Save();
    }

    public void Load()
    {
        dataList = jsonManager.LoadData<GoodsDataManager>(Constants.GoodsDataFile).dataList
            ?? new List<GoodsData>();
    }

    public void Save() => jsonManager.SaveData(Constants.GoodsDataFile, this);

    private static bool MatchesCategory(GoodsData item, string category)
    {
        return item != null && !string.IsNullOrWhiteSpace(category)
            && string.Equals(item.category?.Trim(), category.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    public List<GoodsData> GetGoodsDataList(string category)
    {
        var result = new List<GoodsData>();
        foreach (GoodsData item in dataList)
            if (MatchesCategory(item, category)) result.Add(item);
        return result;
    }

    /// <summary>기존 조회 기능은 유지하되 조회 중 레벨을 변경하지 않습니다.</summary>
    public GoodsData GetGoodsDataByIndex(int goodsNumber)
    {
        var items = GameManager.instance?.storeinfo_data?.dataList;
        if (items == null || goodsNumber < 0 || goodsNumber >= items.Count || items[goodsNumber] == null)
            return null;
        return GetGoodsDataByCategory(items[goodsNumber].category.ToString());
    }

    public GoodsData GetGoodsDataByCategory(string category)
    {
        foreach (GoodsData item in dataList)
            if (MatchesCategory(item, category)) return item;
        return null;
    }

    /// <summary>같은 카테고리의 모든 레벨을 임시 리스트 없이 비교합니다.</summary>
    public GoodsData GetValidatedGoodsData(string category)
    {
        GoodsData first = null;
        foreach (GoodsData item in dataList)
        {
            if (!MatchesCategory(item, category)) continue;
            if (item.level < 0 || (first != null && first.level != item.level)) return null;
            if (first == null) first = item;
        }
        return first;
    }

    public GoodsData GetGoodsDataByID(string id)
    {
        foreach (GoodsData item in dataList)
            if (item != null && item.id == id) return item;
        return null;
    }

    /// <summary>다음 레벨 구매만 허용하고 저장 전 원본을 변경하지 않는 복사본을 만듭니다.</summary>
    public bool TryCreateUpgradeData(StoreInfo_Object product, out GoodsDataManager upgradedData)
    {
        upgradedData = null;
        if (product == null || product.theme != ItemTheme.Default) return false;
        string category = product.category.ToString();
        GoodsData current = GetValidatedGoodsData(category);
        if (current == null || product.level != current.level + 1) return false;

        // 1. 횃대는 앞/뒤 두 데이터, 다른 보조도구는 한 데이터가 존재해야 합니다.
        int expectedCount = product.category == StoreItemCategory.Rack ? 2 : 1;
        int count = 0;
        foreach (GoodsData item in dataList)
            if (MatchesCategory(item, category)) count++;
        if (count != expectedCount) return false;

        // 2. 대상 레벨을 명시적으로 대입하므로 잘못된 반복 호출로 레벨을 더 올리지 않습니다.
        upgradedData = new GoodsDataManager();
        foreach (GoodsData item in dataList)
        {
            if (item == null) return false;
            upgradedData.dataList.Add(new GoodsData(item.id, item.name, item.category,
                MatchesCategory(item, category) ? product.level : item.level, item.interior_id));
        }
        return true;
    }

    public JsonManager.SaveEntry CreateSaveEntry()
    {
        return JsonManager.CreateSaveEntry(Constants.GoodsDataFile, this);
    }

    public void ApplyPurchaseData(GoodsDataManager purchasedData)
    {
        dataList = purchasedData.dataList;
    }
}

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
        if (dataList == null) return result;
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
        if (dataList == null) return null;
        foreach (GoodsData item in dataList)
            if (MatchesCategory(item, category)) return item;
        return null;
    }

    /// <summary>같은 카테고리의 모든 레벨을 임시 리스트 없이 비교합니다.</summary>
    public GoodsData GetValidatedGoodsData(string category)
    {
        // 화면 초기화에서도 조회하므로 누락된 데이터는 예외 대신 표시할 수 없는 상태로 전달합니다.
        if (dataList == null) return null;
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
        if (dataList == null) return null;
        foreach (GoodsData item in dataList)
            if (item != null && item.id == id) return item;
        return null;
    }

    /// <summary>
    /// 저장 데이터를 복사하거나 변경하지 않고 다음 레벨 구매가 가능한지 검사합니다.
    /// 횃대 앞/뒤 데이터 수와 레벨이 일치해야 하므로 모든 항목을 한 번에 확인합니다.
    /// </summary>
    public bool CanUpgrade(StoreInfo_Object product)
    {
        if (product == null || product.theme != ItemTheme.Default || dataList == null) return false;
        string category = product.category.ToString();
        int expectedCount = product.category == StoreItemCategory.Rack ? 2 : 1;
        GoodsData current = null;
        int count = 0;

        // 1. 복사할 원본에 누락된 항목이 없는지, 같은 카테고리의 레벨이 일치하는지 검사합니다.
        foreach (GoodsData item in dataList)
        {
            if (item == null) return false;
            if (!MatchesCategory(item, category)) continue;
            if (item.level < 0 || (current != null && current.level != item.level)) return false;
            if (current == null) current = item;
            count++;
        }

        // 2. 횃대는 두 항목, 다른 보조도구는 한 항목이어야 하며 다음 레벨만 허용합니다.
        // int 최대 레벨에서 +1 계산이 음수로 넘치는 경우도 구매 가능한 것으로 판단하지 않습니다.
        return count == expectedCount && current != null && current.level < int.MaxValue
            && product.level == current.level + 1;
    }

    /// <summary>구매 조건을 재검사한 후 저장 전 원본을 변경하지 않는 복사본을 만듭니다.</summary>
    public bool TryCreateUpgradeData(StoreInfo_Object product, out GoodsDataManager upgradedData)
    {
        upgradedData = null;
        if (!CanUpgrade(product)) return false;
        string category = product.category.ToString();

        // 검증이 끝난 원본만 복사하고 대상 레벨을 대입합니다. 저장 확정 전에는 기존 레벨을 유지합니다.
        upgradedData = new GoodsDataManager();
        foreach (GoodsData item in dataList)
        {
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

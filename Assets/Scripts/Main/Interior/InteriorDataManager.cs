using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class InteriorData
{

    public string id;           //인테리어 아이템 id
    public bool isHaving;         //인테리어 아이템 보유중 여부
    public bool isAdjusting;      //인테리어 아이템 적용중 여부
    public string storeinfo_id; //storeinfo_data의 아이디를 참조

    public InteriorData(string _id, bool _isHaving, bool _isAdjusting, string _storeinfo_id)
    {
        this.id = _id;
        this.isHaving = _isHaving;
        this.isAdjusting = _isAdjusting;
        this.storeinfo_id = _storeinfo_id;
    }
}

/// <summary>
/// 인테리어의 보유·배치 상태를 조회하고 저장합니다.
/// 변경은 복사본에서 구성한 뒤 저장에 성공한 경우에만 실행 중 데이터에 반영합니다.
/// </summary>
public class InteriorDataManager
{
    public List<InteriorData> dataList;
    private readonly JsonManager jsonManager = new JsonManager();
    private static readonly StoreItemCategory[] DevelopmentCategories =
    {
        StoreItemCategory.Vase, StoreItemCategory.Box, StoreItemCategory.Thread
    };

    public InteriorDataManager()
    {
        dataList = new List<InteriorData>();
    }

    public void ResetData()
    {
        ResetData(true);
    }

    /// <summary>기본 데이터를 구성하고, 상점 데이터와 함께 저장할 경우에는 즉시 저장을 생략합니다.</summary>
    public void ResetData(bool saveImmediately)
    {
        // 누락되거나 깨진 기본 파일의 빈 대체값으로 기존 보유 기록을 지우지 않습니다.
        InteriorDataManager defaultData = jsonManager.LoadDefaultData<InteriorDataManager>(Constants.InteriorDataFile);
        if (defaultData.dataList == null || defaultData.dataList.Count == 0)
        {
            Debug.LogError("[InteriorData] 기본 인테리어 데이터가 없어 초기화를 중단합니다.");
            return;
        }
        if (saveImmediately)
        {
            if (!TryCommitData(defaultData, out string error)) Debug.LogError("[InteriorData] " + error);
        }
        else dataList = defaultData.dataList ?? new List<InteriorData>();
    }

    /// <summary>기존 파일을 읽습니다. 테이블과의 정합성은 공통 초기화의 TryPrepareData에서 확인합니다.</summary>
    public void Load()
    {
        InteriorDataManager loadedData = jsonManager.LoadData<InteriorDataManager>(Constants.InteriorDataFile);
        dataList = loadedData.dataList ?? new List<InteriorData>();
    }

    public void Save()
    {
        jsonManager.SaveData(Constants.InteriorDataFile, this);
    }

    /// <summary>레벨에 따라 보유하는 외형 카테고리입니다. 횃대는 인테리어 저장 항목을 사용하지 않습니다.</summary>
    public static bool IsDevelopmentCategory(StoreItemCategory category)
    {
        return category == StoreItemCategory.Vase || category == StoreItemCategory.Box
            || category == StoreItemCategory.Thread;
    }

    /// <summary>
    /// 업데이트로 추가된 기본 항목을 병합하고 잘못된 보유·배치 상태를 정리합니다.
    /// 정상적인 과거 레벨 외형 선택과 테이블에서 사라진 상품의 보유 기록은 유지합니다.
    /// </summary>
    public bool TryPrepareData(StoreInfo_Data table, GoodsDataManager goods, out string error)
    {
        error = null;
        // 1. 테이블과 기본 JSON이 잘못된 경우 기존 저장 상태를 그대로 유지합니다.
        InteriorDataManager defaults = jsonManager.LoadDefaultData<InteriorDataManager>(Constants.InteriorDataFile);
        if (!TryValidatePreparation(table, goods, defaults, out error)) return false;

        // 2. 중복 상품은 보유 기록을 합치고, 없는 항목만 기본값으로 추가합니다.
        InteriorDataManager prepared = new InteriorDataManager();
        var byProduct = new Dictionary<string, InteriorData>(StringComparer.Ordinal);
        if (dataList != null)
            foreach (InteriorData item in dataList)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.storeinfo_id)) continue;
                if (byProduct.TryGetValue(item.storeinfo_id, out InteriorData existing))
                {
                    existing.isHaving |= item.isHaving;
                    existing.isAdjusting |= item.isAdjusting;
                    continue;
                }
                var copy = new InteriorData(item.id, item.isHaving, item.isAdjusting, item.storeinfo_id);
                prepared.dataList.Add(copy);
                byProduct.Add(copy.storeinfo_id, copy);
            }
        foreach (InteriorData item in defaults.dataList)
            if (!byProduct.ContainsKey(item.storeinfo_id))
            {
                var copy = new InteriorData(item.id, item.isHaving, item.isAdjusting, item.storeinfo_id);
                prepared.dataList.Add(copy);
                byProduct.Add(copy.storeinfo_id, copy);
            }

        // 3. 저장 ID 충돌을 복구합니다. UI와 상품 판정에는 storeinfo_id를 사용합니다.
        var savedIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (InteriorData item in prepared.dataList)
        {
            if (!string.IsNullOrWhiteSpace(item.id) && savedIds.Add(item.id)) continue;
            InteriorData defaultItem = defaults.GetInteriorDataByStoreInfoId(item.storeinfo_id);
            string baseId = defaultItem != null ? defaultItem.id : "Interior_" + item.storeinfo_id;
            string uniqueId = baseId;
            for (int suffix = 1; !savedIds.Add(uniqueId); suffix++) uniqueId = baseId + "_" + suffix;
            item.id = uniqueId;
        }

        // 4. 보조도구 보유는 실제 업그레이드 레벨에 맞추고 미보유 외형의 배치를 해제합니다.
        foreach (InteriorData item in prepared.dataList)
        {
            if (!item.isHaving) item.isAdjusting = false;
            if (!table.TryGetItem(item.storeinfo_id, out StoreInfo_Object product)) continue;
            if (product.theme == ItemTheme.Default && IsDevelopmentCategory(product.category))
                item.isHaving = product.level <= goods.GetValidatedGoodsData(product.category.ToString()).level;
            if (!item.isHaving) item.isAdjusting = false;
        }

        // 5. 정상적인 기존 배치를 우선 유지합니다. 복수 배치는 상품 ID로 일관되게 하나를 선택합니다.
        foreach (StoreItemCategory category in Enum.GetValues(typeof(StoreItemCategory)))
        {
            InteriorData selected = null;
            foreach (InteriorData item in prepared.dataList)
                if (item.isHaving && item.isAdjusting && table.TryGetItem(item.storeinfo_id, out StoreInfo_Object product)
                    && product.category == category
                    && (selected == null || StringComparer.Ordinal.Compare(item.storeinfo_id, selected.storeinfo_id) < 0))
                    selected = item;
            if (selected == null && TryGetDefaultProduct(category, table, out StoreInfo_Object defaultProduct))
            {
                InteriorData candidate = prepared.GetInteriorDataByStoreInfoId(defaultProduct.id);
                if (candidate != null && candidate.isHaving) selected = candidate;
            }
            foreach (InteriorData item in prepared.dataList)
                if (table.TryGetItem(item.storeinfo_id, out StoreInfo_Object product) && product.category == category)
                    item.isAdjusting = ReferenceEquals(item, selected);
        }

        // 6. 변경이 없는 데이터는 다시 저장하지 않습니다. 저장 실패 시 원본 메모리도 유지합니다.
        return TryCommitData(prepared, out error);
    }

    /// <summary>준비에 필요한 기본 상품·저장 항목·보조도구 레벨을 검사하며 파일을 변경하지 않습니다.</summary>
    private static bool TryValidatePreparation(StoreInfo_Data table, GoodsDataManager goods,
        InteriorDataManager defaults, out string error)
    {
        error = "인테리어 테이블 또는 기본 저장 데이터가 누락되었습니다.";
        if (table?.dataList == null || table.dataList.Count == 0 || defaults?.dataList == null
            || defaults.dataList.Count == 0 || goods == null) return false;

        // 1. 상품 ID와 기본 테마의 카테고리/레벨 조합은 유일해야 합니다.
        var productIds = new HashSet<string>(StringComparer.Ordinal);
        var defaultLevels = new HashSet<(StoreItemCategory category, int level)>();
        foreach (StoreInfo_Object product in table.dataList)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.id) || !productIds.Add(product.id)
                || product.level < 0 || !Enum.IsDefined(typeof(StoreItemCategory), product.category)
                || !Enum.IsDefined(typeof(ItemTheme), product.theme)
                || (product.theme == ItemTheme.Default && !defaultLevels.Add((product.category, product.level))))
            {
                error = "인테리어 상품 ID 또는 기본 상품 레벨이 누락되었거나 중복되었습니다.";
                return false;
            }
        }

        // 2. 기본 JSON은 현재 테이블의 상품을 참조하고 중복된 저장 항목이 없어야 합니다.
        var savedIds = new HashSet<string>(StringComparer.Ordinal);
        var savedProducts = new HashSet<string>(StringComparer.Ordinal);
        foreach (InteriorData item in defaults.dataList)
            if (item == null || string.IsNullOrWhiteSpace(item.id) || !savedIds.Add(item.id)
                || string.IsNullOrWhiteSpace(item.storeinfo_id) || !savedProducts.Add(item.storeinfo_id)
                || !productIds.Contains(item.storeinfo_id))
            {
                error = "기본 인테리어 JSON과 상품 테이블이 일치하지 않습니다.";
                return false;
            }
        foreach (StoreInfo_Object product in table.dataList)
        {
            bool needsSavedItem = product.theme != ItemTheme.Default || IsDevelopmentCategory(product.category);
            if (needsSavedItem && !savedProducts.Contains(product.id))
            {
                error = "기본 인테리어 저장 항목이 없습니다: " + product.id;
                return false;
            }
            if (product.category != StoreItemCategory.Rack && !TryGetDefaultProduct(product.category, table, out _))
            {
                error = "기본 레벨0 외형이 없습니다: " + product.category;
                return false;
            }
        }

        // 3. 현재 보조도구 레벨이 테이블에 존재해야 소유 여부를 안전하게 복구할 수 있습니다.
        foreach (StoreItemCategory category in DevelopmentCategories)
        {
            GoodsData current = goods.GetValidatedGoodsData(category.ToString());
            if (current == null || goods.GetGoodsDataList(category.ToString()).Count != 1
                || !table.TryGetItem(table.GetIDByCategoryAndLevel(category.ToString(), current.level), out _))
            {
                error = "보조도구 레벨과 인테리어 상품이 일치하지 않습니다: " + category;
                return false;
            }
        }
        error = null;
        return true;
    }

    public InteriorData GetInteriorDataByStoreInfoId(string _storeinfo_id)
    {
        if (dataList == null) return null;
        foreach (InteriorData item in dataList)
            if (item != null && item.storeinfo_id == _storeinfo_id) return item;
        return null;
    }

    public InteriorData GetInteriorDataById(string _id)
    {
        if (dataList == null) return null;
        foreach (InteriorData item in dataList)
            if (item != null && item.id == _id) return item;
        return null;
    }

    /// <summary>복사본을 만들지 않고 상품의 저장 항목이 존재하고 아직 미보유인지 검사합니다.</summary>
    public bool CanPurchase(StoreInfo_Object product)
    {
        InteriorData current = product != null ? GetUniqueItem(product.id) : null;
        return current != null && !current.isHaving;
    }

    /// <summary>보유 상태와 자동 배치를 복사본에 적용하여 구매 확정 전 원본을 유지합니다.</summary>
    public bool TryCreatePurchaseData(StoreInfo_Object product, StoreInfo_Data table, bool adjust,
        out InteriorDataManager purchasedData)
    {
        purchasedData = null;
        if (table?.dataList == null || product == null || !table.TryGetItem(product.id, out StoreInfo_Object actual)
            || !CanPurchase(actual)) return false;

        // 검증과 복사를 구분하여 구매 가능 여부를 묻는 동안에는 전체 저장 데이터를 복사하지 않습니다.
        purchasedData = CopyData();
        purchasedData.GetInteriorDataByStoreInfoId(actual.id).isHaving = true;
        if (adjust) purchasedData.SetAdjusting(actual, table);
        return true;
    }

    /// <summary>보유한 아이템만 배치하고 저장 실패 시 기존 배치 상태를 유지합니다.</summary>
    public bool TryAdjustItem(string id, StoreInfo_Data table)
    {
        if (!TryGetOwnedProduct(id, table, out StoreInfo_Object product, out _)) return false;
        InteriorDataManager adjusted = CopyData();
        adjusted.SetAdjusting(product, table);
        return TryCommitData(adjusted, out _);
    }

    /// <summary>선택 후 외형을 미리 조회하여 UI에서 이미지 참조를 검증할 수 있게 합니다. 데이터를 변경하지 않습니다.</summary>
    public bool TryGetSelectionTarget(string id, StoreInfo_Data table, out StoreInfo_Object target, out string error)
    {
        target = null;
        if (!TryGetOwnedProduct(id, table, out StoreInfo_Object product, out error)) return false;
        // 적용 중인 외형의 재클릭은 Default 레벨0으로 되돌립니다. 실제 업그레이드 레벨은 바꾸지 않습니다.
        if (!GetUniqueItem(id).isAdjusting) { target = product; return true; }
        if (!TryGetDefaultProduct(product.category, table, out target))
        {
            error = "복원할 기본 외형이 없습니다: " + product.category;
            return false;
        }
        if (IsDevelopmentCategory(product.category) && GetUniqueItem(target.id)?.isHaving != true)
        {
            target = null;
            error = "기본 보조도구 외형의 보유 데이터가 없습니다.";
            return false;
        }
        return true;
    }

    /// <summary>Main에서 보유 외형을 선택하거나 재클릭으로 기본 외형에 복원한 뒤 저장합니다.</summary>
    public bool TryToggleItem(string id, StoreInfo_Data table, out string error)
    {
        // 1. 소유·상품·기본 복원 대상을 먼저 검증하여 실패한 선택은 변경하지 않습니다.
        if (!TryGetSelectionTarget(id, table, out StoreInfo_Object target, out error)) return false;
        // 2. 저장 항목이 없는 장식 Default는 같은 카테고리의 모든 배치를 해제하는 상태로 표현합니다.
        InteriorDataManager adjusted = CopyData();
        adjusted.SetAdjusting(target, table);
        // 3. 파일 저장에 성공해야 메모리 배치가 바뀝니다. 이미 같은 상태이면 저장을 생략합니다.
        return TryCommitData(adjusted, out error);
    }

    /// <summary>카테고리의 보유·적용 외형을 조회하며, 적용 항목이 없으면 테이블의 Default 레벨0을 반환합니다.</summary>
    public bool TryGetDisplayedProduct(StoreItemCategory category, StoreInfo_Data table, out StoreInfo_Object product)
    {
        product = null;
        if (table?.dataList == null) return false;
        if (dataList != null)
            foreach (InteriorData item in dataList)
                if (item != null && item.isHaving && item.isAdjusting
                    && table.TryGetItem(item.storeinfo_id, out StoreInfo_Object candidate) && candidate.category == category)
                {
                    if (product != null) { product = null; return false; }
                    product = candidate;
                }
        return product != null || TryGetDefaultProduct(category, table, out product);
    }

    /// <summary>상품 ID가 중복된 저장 행은 한 상품의 정상 상태로 취급하지 않습니다.</summary>
    private InteriorData GetUniqueItem(string id)
    {
        if (dataList == null || string.IsNullOrWhiteSpace(id)) return null;
        InteriorData found = null;
        foreach (InteriorData item in dataList)
            if (item != null && item.storeinfo_id == id)
            {
                if (found != null) return null;
                found = item;
            }
        return found;
    }

    /// <summary>사용자가 선택할 수 있는 현재 테이블의 보유 상품인지 확인합니다.</summary>
    private bool TryGetOwnedProduct(string id, StoreInfo_Data table, out StoreInfo_Object product, out string error)
    {
        product = null;
        error = "보유하지 않았거나 유효하지 않은 인테리어 상품입니다.";
        if (table?.dataList == null || !table.TryGetItem(id, out product) || GetUniqueItem(id)?.isHaving != true
            || product.category == StoreItemCategory.Rack || product.level < 0
            || !Enum.IsDefined(typeof(StoreItemCategory), product.category)
            || !Enum.IsDefined(typeof(ItemTheme), product.theme)) return false;
        error = null;
        return true;
    }

    /// <summary>기본 외형을 ID 숫자나 저장 순서가 아닌 테이블의 카테고리와 레벨로 조회합니다.</summary>
    private static bool TryGetDefaultProduct(StoreItemCategory category, StoreInfo_Data table, out StoreInfo_Object product)
    {
        product = null;
        return table?.dataList != null
            && table.TryGetItem(table.GetIDByCategoryAndLevel(category.ToString(), 0), out product);
    }

    private void SetAdjusting(StoreInfo_Object product, StoreInfo_Data table)
    {
        foreach (InteriorData item in dataList)
        {
            if (table.TryGetItem(item.storeinfo_id, out StoreInfo_Object other) && other.category == product.category)
                item.isAdjusting = item.isHaving && item.storeinfo_id == product.id;
        }
    }

    private InteriorDataManager CopyData()
    {
        var copy = new InteriorDataManager();
        if (dataList != null)
            foreach (InteriorData item in dataList)
                if (item != null) copy.dataList.Add(new InteriorData(item.id, item.isHaving, item.isAdjusting, item.storeinfo_id));
        return copy;
    }

    /// <summary>변경이 있을 때만 저장하고 성공 후 복사본을 실행 중 데이터에 적용합니다.</summary>
    private bool TryCommitData(InteriorDataManager changed, out string error)
    {
        error = null;
        if (HasSameData(changed)) return true;
        try
        {
            if (!jsonManager.TrySaveData(Constants.InteriorDataFile, changed))
            {
                error = "인테리어 상태를 저장할 수 없어 이전 상태를 유지합니다.";
                return false;
            }
            dataList = changed.dataList;
            return true;
        }
        catch (Exception exception)
        {
            error = "인테리어 저장 실패: " + exception.Message;
            return false;
        }
    }

    /// <summary>필드 값까지 비교하여 재실행이나 같은 항목 선택 때문에 불필요한 저장을 하지 않게 합니다.</summary>
    private bool HasSameData(InteriorDataManager other)
    {
        if (dataList == null || other?.dataList == null || dataList.Count != other.dataList.Count) return false;
        for (int i = 0; i < dataList.Count; i++)
        {
            InteriorData current = dataList[i];
            InteriorData candidate = other.dataList[i];
            if (current == null || candidate == null || current.id != candidate.id || current.storeinfo_id != candidate.storeinfo_id
                || current.isHaving != candidate.isHaving || current.isAdjusting != candidate.isAdjusting) return false;
        }
        return true;
    }

    public JsonManager.SaveEntry CreateSaveEntry()
    {
        return JsonManager.CreateSaveEntry(Constants.InteriorDataFile, this);
    }

    public void ApplyPurchaseData(InteriorDataManager purchasedData)
    {
        dataList = purchasedData.dataList;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>상점 화면을 초기화하고 상품별 표시와 구매 결과를 갱신합니다.</summary>
public class StoreData : MonoBehaviour
{
    [Header("[Store Product]")]
    public GameObject[] goodsContents;
    public GameObject[] interiorContetns; // 기존 씬 연결 유지: Star, Sea 순
    public SpriteArray[] goodsImages;
    public StoreInfo_Data storeinfo_data;
    public GameObject[] soldOut;
    public GameObject[] StarSoldOut;
    public GameObject[] SeaSoldOut;
    public string[] developCategory = { Constants.GoodsData_Rack, Constants.GoodsData_Vase, Constants.GoodsData_Box, Constants.GoodsData_Thread };

    [Header("상품별 UI 참조 (비워두면 기존 씬에서 한 번만 연결)")]
    [SerializeField] private ProductView[] developmentViews;
    [SerializeField] private ProductView[] starViews;
    [SerializeField] private ProductView[] seaViews;
    [SerializeField] private ProductView specialFeedView;
    [SerializeField, Min(0)] private int specialFeedPrice = 1000;

    private GoodsDataManager goodsDataManager;
    private InteriorDataManager interiorDataManager;
    private BuyCheck buyCheck;
    private StorePurchaseService purchaseService;
    private bool initialized;

    public const string SpecialFeedProductId = "SpecialFeed";
    public bool IsInitialized => initialized;
    public string LastPurchaseError { get; private set; }

    /// <summary>여러 Start 호출 순서에 관계없이 참조와 상품 UI를 한 번만 준비합니다.</summary>
    public bool Initialize()
    {
        if (initialized) return true;
        GameManager manager = GameManager.instance;
        if (manager == null || !manager.IsInitialized || manager.storeinfo_data == null)
        {
            Debug.LogError("[StoreData] GameManager 데이터 초기화가 필요합니다.", this);
            return false;
        }

        // 1. 화면에서 사용할 데이터와 구매 서비스를 명시적으로 전달합니다.
        goodsDataManager = manager.goodsDataManager;
        interiorDataManager = manager.interiorDataManager;
        storeinfo_data = manager.storeinfo_data;
        buyCheck = GetComponent<BuyCheck>();
        if (buyCheck == null) return false;
        buyCheck.Initialize(this, interiorDataManager, storeinfo_data);
        purchaseService = new StorePurchaseService(manager.jsonManager, manager.playerDataManager,
            goodsDataManager, interiorDataManager, storeinfo_data, specialFeedPrice);

        // 2. 기존 씬은 유지하면서 참조를 캐싱합니다. 이후 갱신에서는 계층을 다시 탐색하지 않습니다.
        if (developmentViews == null || developmentViews.Length == 0)
        {
            int count = Mathf.Min(goodsContents?.Length ?? 0, developCategory?.Length ?? 0);
            developmentViews = new ProductView[count];
            for (int i = 0; i < count; i++)
            {
                developmentViews[i] = ProductView.FromLegacy(goodsContents[i], GetButton(buyCheck.ButtonObj, i), GetObject(soldOut, i));
                developmentViews[i].category = developCategory[i];
            }
        }
        starViews = CreateInteriorViews(starViews, ItemTheme.Star, buyCheck.StarButtonObj, StarSoldOut);
        seaViews = CreateInteriorViews(seaViews, ItemTheme.Sea, buyCheck.SeaButtonObj, SeaSoldOut);
        CacheSpecialFeedView();
        initialized = true;
        RefreshProducts();
        return true;
    }

    public void Start() => Initialize();

    private ProductView[] CreateInteriorViews(ProductView[] views, ItemTheme theme, GameObject[] buttons, GameObject[] overlays)
    {
        if (views != null && views.Length > 0) return views;
        List<StoreInfo_Object> items = storeinfo_data.GetItemsByTheme(theme);
        int count = buttons?.Length ?? 0;
        views = new ProductView[count];
        for (int i = 0; i < count; i++)
        {
            BuyButtonInfo button = GetButton(buttons, i);
            views[i] = ProductView.FromLegacy(button != null ? button.transform.parent.gameObject : null, button, GetObject(overlays, i));
            // 상품 ID를 한 번 연결하고 이후 갱신은 테이블 순서가 아닌 ID로 처리합니다.
            views[i].productId = i < items.Count ? items[i].id : null;
        }
        return views;
    }

    private void CacheSpecialFeedView()
    {
        if (specialFeedView == null || specialFeedView.button == null)
        {
            CategorySelect categories = GetComponent<CategorySelect>();
            GameObject panel = categories != null ? GetObject(categories.categoryProducts, (int)StoreType.SpecialProduct) : null;
            BuyButtonInfo button = panel != null ? panel.GetComponentInChildren<BuyButtonInfo>(true) : null;
            specialFeedView = ProductView.FromLegacy(button != null ? button.transform.parent.gameObject : null, button, null);
        }
        specialFeedView.productId = SpecialFeedProductId;
    }

    private static GameObject GetObject(GameObject[] objects, int index)
    {
        return objects != null && index >= 0 && index < objects.Length ? objects[index] : null;
    }

    private static BuyButtonInfo GetButton(GameObject[] objects, int index)
    {
        GameObject target = GetObject(objects, index);
        return target != null ? target.GetComponent<BuyButtonInfo>() : null;
    }

    /// <summary>선택한 테마의 상품을 ID로 조회하여 가격과 보유 상태를 갱신합니다.</summary>
    public void UpdateStoreInteriorData(ItemTheme theme)
    {
        if (Initialize()) RefreshInteriorViews(theme == ItemTheme.Sea ? seaViews : starViews);
    }

    public void RefreshProducts()
    {
        if (!initialized) return;
        for (int i = 0; i < developmentViews.Length; i++)
        {
            ProductView view = developmentViews[i];
            if (view == null) continue;
            GoodsData current = goodsDataManager.GetValidatedGoodsData(view.category);
            string id = current != null ? storeinfo_data.GetIDByCategoryAndLevel(view.category, current.level + 1) : null;
            bool available = storeinfo_data.TryGetItem(id, out StoreInfo_Object item);
            view.SetAvailable(buyCheck, i, id, available);
            if (!available) continue;
            if (view.price != null) view.price.text = item.gold.ToString();
            if (view.description != null) view.description.text = item.effect ?? "";
            if (view.image != null && goodsImages != null && i < goodsImages.Length
                && goodsImages[i]?.imageList != null && item.level >= 0 && item.level < goodsImages[i].imageList.Length)
                view.image.sprite = goodsImages[i].imageList[item.level];
        }
        RefreshInteriorViews(starViews);
        RefreshInteriorViews(seaViews);
        specialFeedView.SetAvailable(buyCheck, 0, SpecialFeedProductId, true);
        if (specialFeedView.price != null) specialFeedView.price.text = specialFeedPrice.ToString();
    }

    private void RefreshInteriorViews(ProductView[] views)
    {
        if (views == null) return;
        for (int i = 0; i < views.Length; i++)
        {
            ProductView view = views[i];
            if (view == null) continue;
            bool exists = storeinfo_data.TryGetItem(view.productId, out StoreInfo_Object item);
            if (view.content != null) view.content.SetActive(exists && item.isButtonActive);
            InteriorData saved = exists ? interiorDataManager.GetInteriorDataByStoreInfoId(item.id) : null;
            view.SetAvailable(buyCheck, i, view.productId, exists && item.isButtonActive && saved != null && !saved.isHaving);
            if (!exists) continue;
            if (view.price != null) view.price.text = item.gold.ToString();
            if (view.description != null) view.description.text = (item.contents ?? "").Replace("nn", "\n");
        }
    }

    public bool IsInteriorProduct(string id)
    {
        return storeinfo_data != null && storeinfo_data.TryGetItem(id, out StoreInfo_Object item) && item.theme != ItemTheme.Default;
    }

    public bool CanBuy(string id, out string error)
    {
        error = "상점 초기화에 실패했습니다.";
        return Initialize() && purchaseService.CanBuy(id, out error);
    }

    /// <summary>검증과 저장을 구매 서비스에 위임하고 확정된 구매만 화면에 반영합니다.</summary>
    public bool BuyGoods(string id)
    {
        if (!Initialize()) return false;
        bool success = purchaseService.TryPurchase(id, out string error);
        LastPurchaseError = error;
        if (success) RefreshProducts();
        else Debug.LogWarning("[StoreData] " + error, this);
        return success;
    }

    [Serializable]
    public class SpriteArray
    {
        public Sprite[] imageList;
    }

    /// <summary>같은 상품의 ID와 UI 참조를 묶어 병렬 배열의 순서 불일치를 줄입니다.</summary>
    [Serializable]
    public class ProductView
    {
        public string productId;
        public string category;
        public Image image;
        public Text price;
        public Text description;
        public BuyButtonInfo button;
        public GameObject soldOut;
        public GameObject content;

        public void SetAvailable(BuyCheck owner, int number, string id, bool available)
        {
            if (soldOut != null) soldOut.SetActive(!available);
            if (button == null) return;
            button.Initialize(owner);
            button.SetSelectGoodsNumber(number);
            button.SetSelectGoodsId(id);
            button.SetAvailable(available);
        }

        /// <summary>기존 UI 이름을 사용하여 자식 번호에 의존하지 않고 초기화 때만 참조를 가져옵니다.</summary>
        public static ProductView FromLegacy(GameObject root, BuyButtonInfo button, GameObject overlay)
        {
            var view = new ProductView { button = button, soldOut = overlay, content = root };
            if (root == null) return view;
            Transform cost = root.transform.Find("Cost");
            if (cost != null) view.price = cost.GetComponentInChildren<Text>(true);
            Transform effect = root.transform.Find("Text_EffectNumber") ?? root.transform.Find("Text_Effect");
            if (effect != null) view.description = effect.GetComponent<Text>();
            if (view.soldOut == null)
            {
                Transform sold = root.transform.Find("SoldOut");
                if (sold != null) view.soldOut = sold.gameObject;
            }
            foreach (Transform child in root.transform)
                if (child.name.StartsWith("Image_Product", StringComparison.Ordinal))
                {
                    view.image = child.GetComponent<Image>();
                    break;
                }
            return view;
        }
    }

    private void OnValidate()
    {
        ValidateViews(developmentViews);
        ValidateViews(starViews);
        ValidateViews(seaViews);
        if (goodsContents != null && goodsImages != null && goodsContents.Length != goodsImages.Length)
            Debug.LogWarning("[StoreData] 보조도구 오브젝트와 이미지 배열 길이가 다릅니다.", this);
        if (goodsContents != null && soldOut != null && goodsContents.Length != soldOut.Length)
            Debug.LogWarning("[StoreData] 보조도구 오브젝트와 품절 배열 길이가 다릅니다.", this);
        if (developmentViews != null)
        {
            var categories = new HashSet<string>();
            foreach (ProductView view in developmentViews)
                if (view != null && (string.IsNullOrWhiteSpace(view.category) || !categories.Add(view.category)))
                    Debug.LogWarning("[StoreData] 보조도구 카테고리를 중복 없이 지정해 주세요.", this);
        }
    }

    private void ValidateViews(ProductView[] views)
    {
        if (views == null) return;
        var ids = new HashSet<string>();
        foreach (ProductView view in views)
        {
            if (view == null || view.button == null || view.price == null)
                Debug.LogWarning("[StoreData] 상품 UI의 버튼 또는 가격 참조가 누락되었습니다.", this);
            if (view != null && !string.IsNullOrEmpty(view.productId) && !ids.Add(view.productId))
                Debug.LogWarning("[StoreData] 상품 ID가 중복되었습니다: " + view.productId, this);
        }
    }
}

/// <summary>UI와 분리된 구매 검증 및 여러 데이터의 저장 확정을 담당합니다.</summary>
public sealed class StorePurchaseService
{
    private readonly JsonManager jsonManager;
    private readonly PlayerDataManager player;
    private readonly GoodsDataManager goods;
    private readonly InteriorDataManager interior;
    private readonly StoreInfo_Data table;
    private readonly int specialFeedPrice;
    private bool purchasing;

    public StorePurchaseService(JsonManager jsonManager, PlayerDataManager player, GoodsDataManager goods,
        InteriorDataManager interior, StoreInfo_Data table, int specialFeedPrice)
    {
        this.jsonManager = jsonManager;
        this.player = player;
        this.goods = goods;
        this.interior = interior;
        this.table = table;
        this.specialFeedPrice = specialFeedPrice;
    }

    public bool CanBuy(string id, out string error)
    {
        return TryPrepare(id, out _, out _, out _, out error);
    }

    private bool TryPrepare(string id, out PlayerData playerData, out GoodsDataManager goodsData,
        out InteriorDataManager interiorData, out string error)
    {
        playerData = null;
        goodsData = null;
        interiorData = null;
        error = "상품 정보가 없거나 현재 구매할 수 없는 상품입니다.";
        int price;
        int reward = 0;
        if (id == StoreData.SpecialFeedProductId)
        {
            price = specialFeedPrice;
            reward = 1; // 특제먹이는 구매당 1개이며 보유 수에 따른 품절이 없습니다.
        }
        else
        {
            if (!table.TryGetItem(id, out StoreInfo_Object product)) return false;
            price = product.gold;
            if (product.theme == ItemTheme.Default)
            {
                if ((int)product.category > (int)StoreItemCategory.Thread
                    || table.GetIDByCategoryAndLevel(product.category.ToString(), product.level) != product.id
                    || !goods.TryCreateUpgradeData(product, out goodsData)) return false;
                // 횃대는 기존 데이터 구조상 인테리어 보유 항목이 없으므로 레벨만 저장합니다.
                if (product.category != StoreItemCategory.Rack
                    && !interior.TryCreatePurchaseData(product, table, true, out interiorData)) return false;
            }
            else if (!product.isButtonActive
                || !interior.TryCreatePurchaseData(product, table, false, out interiorData)) return false;
        }

        if (!player.TryCreatePurchaseData(price, reward, out playerData))
        {
            error = "골드가 부족하거나 상품 가격/보유 수량이 올바르지 않습니다.";
            return false;
        }
        error = null;
        return true;
    }

    public bool TryPurchase(string id, out string error)
    {
        error = "이미 구매를 처리하고 있습니다.";
        if (purchasing) return false;
        purchasing = true;
        try
        {
            // 1. 변경할 데이터의 복사본을 만들고 구매 조건을 최종 검사합니다.
            if (!TryPrepare(id, out PlayerData playerData, out GoodsDataManager goodsData,
                out InteriorDataManager interiorData, out error)) return false;
            var entries = new List<JsonManager.SaveEntry> { player.CreatePurchaseSaveEntry(playerData) };
            if (goodsData != null) entries.Add(goodsData.CreateSaveEntry());
            if (interiorData != null) entries.Add(interiorData.CreateSaveEntry());

            // 2. 복구 가능한 기록을 저장한 후에만 메모리 상태를 교체합니다.
            if (!jsonManager.TrySaveBatch(entries))
            {
                error = "구매 정보를 저장할 수 없습니다. 재화와 상품은 변경되지 않았습니다.";
                return false;
            }
            if (goodsData != null) goods.ApplyPurchaseData(goodsData);
            if (interiorData != null) interior.ApplyPurchaseData(interiorData);
            player.ApplyPurchaseData(playerData);
            return true;
        }
        finally { purchasing = false; }
    }
}

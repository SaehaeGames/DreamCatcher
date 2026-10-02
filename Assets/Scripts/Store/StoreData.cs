using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>상점 초기화와 상품 화면 갱신을 담당합니다. 구매 규칙과 저장은 구매 서비스에 위임합니다.</summary>
public class StoreData : MonoBehaviour
{
    [Header("[Store Product]")]
    public GameObject[] goodsContents;
    public GameObject[] interiorContetns; // 기존 씬의 저장된 연결 유지: Star, Sea 순
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
    public string LastPurchaseError { get; private set; }

    private void Start() => Initialize();

    /// <summary>참조 준비, UI 연결, 첫 화면 갱신을 순서대로 수행하며 중복 초기화는 생략합니다.</summary>
    public bool Initialize()
    {
        if (initialized) return true;
        if (!TryInitializeReferences()) return false;

        // 1. 기존 배열 또는 Inspector의 상품별 참조로 표시 대상을 구성합니다.
        InitializeProductViews();
        // 2. 각 버튼에는 담당 객체와 상품 ID를 한 번만 연결합니다.
        BindButtons(developmentViews);
        BindButtons(starViews);
        BindButtons(seaViews);
        specialFeedView.Bind(buyCheck);

        // 3. 모든 연결을 마친 뒤 데이터에 맞는 표시 상태를 적용합니다.
        initialized = true;
        RefreshProducts();
        return true;
    }

    /// <summary>구매와 화면에서 사용하는 의존성을 초기화 시 명시적으로 전달합니다.</summary>
    private bool TryInitializeReferences()
    {
        GameManager manager = GameManager.instance;
        buyCheck = GetComponent<BuyCheck>();
        if (manager == null || !manager.IsInitialized || manager.jsonManager == null
            || manager.playerDataManager == null || manager.storeinfo_data == null
            || manager.goodsDataManager == null || manager.interiorDataManager == null || buyCheck == null)
        {
            Debug.LogError("[StoreData] 데이터 매니저와 BuyCheck 초기화가 필요합니다.", this);
            return false;
        }

        goodsDataManager = manager.goodsDataManager;
        interiorDataManager = manager.interiorDataManager;
        storeinfo_data = manager.storeinfo_data;
        buyCheck.Initialize(this, interiorDataManager, storeinfo_data);
        purchaseService = new StorePurchaseService(manager.jsonManager, manager.playerDataManager,
            goodsDataManager, interiorDataManager, storeinfo_data, specialFeedPrice);
        return true;
    }

    /// <summary>기존 씬의 자동 연결은 초기화 때만 수행하고 이후에는 캐시된 참조를 사용합니다.</summary>
    private void InitializeProductViews()
    {
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

        // 이미지 배열도 상품별로 연결하여 매번 병렬 배열의 범위를 비교하지 않습니다.
        for (int i = 0; i < developmentViews.Length; i++)
        {
            ProductView view = developmentViews[i];
            if (view != null && (view.levelImages == null || view.levelImages.Length == 0)
                && goodsImages != null && i < goodsImages.Length)
                view.levelImages = goodsImages[i]?.imageList;
        }
        starViews = CreateInteriorViews(starViews, ItemTheme.Star, buyCheck.StarButtonObj, StarSoldOut);
        seaViews = CreateInteriorViews(seaViews, ItemTheme.Sea, buyCheck.SeaButtonObj, SeaSoldOut);

        if (specialFeedView == null || specialFeedView.button == null)
        {
            CategorySelect categories = GetComponent<CategorySelect>();
            GameObject panel = categories != null ? GetObject(categories.categoryProducts, (int)StoreType.SpecialProduct) : null;
            BuyButtonInfo button = panel != null ? panel.GetComponentInChildren<BuyButtonInfo>(true) : null;
            specialFeedView = ProductView.FromLegacy(GetProductRoot(button), button, null);
        }
        specialFeedView.productId = StorePurchaseService.SpecialFeedProductId;
    }

    private ProductView[] CreateInteriorViews(ProductView[] views, ItemTheme theme, GameObject[] buttons, GameObject[] overlays)
    {
        if (views != null && views.Length > 0) return views;
        List<StoreInfo_Object> items = storeinfo_data.GetItemsByTheme(theme);
        views = new ProductView[buttons?.Length ?? 0];
        for (int i = 0; i < views.Length; i++)
        {
            BuyButtonInfo button = GetButton(buttons, i);
            views[i] = ProductView.FromLegacy(GetProductRoot(button), button, GetObject(overlays, i));
            // 비활성 상품도 같은 슬롯에 남겨 뒤 상품의 이미지 연결이 밀리지 않게 합니다.
            views[i].productId = i < items.Count ? items[i].id : null;
        }
        return views;
    }

    private void BindButtons(ProductView[] views)
    {
        foreach (ProductView view in views)
            if (view != null) view.Bind(buyCheck);
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

    private static GameObject GetProductRoot(BuyButtonInfo button)
    {
        return button != null && button.transform.parent != null ? button.transform.parent.gameObject : null;
    }

    /// <summary>테마 선택은 지역적인 두 갈래 결정이므로 조건문을 유지합니다.</summary>
    public void UpdateStoreInteriorData(ItemTheme theme)
    {
        if ((theme == ItemTheme.Sea || theme == ItemTheme.Star) && Initialize())
            RefreshInteriorViews(theme == ItemTheme.Sea ? seaViews : starViews);
    }

    /// <summary>유형별 화면을 갱신합니다. 반복 가능한 특제먹이의 두 표시 동작은 이 함수에 둡니다.</summary>
    public void RefreshProducts()
    {
        if (!initialized) return;
        RefreshDevelopmentViews();
        RefreshInteriorViews(starViews);
        RefreshInteriorViews(seaViews);
        specialFeedView.UpdateAvailability(StorePurchaseService.SpecialFeedProductId, true);
        specialFeedView.ShowInformation(specialFeedPrice, null);
    }

    private void RefreshDevelopmentViews()
    {
        foreach (ProductView view in developmentViews)
        {
            if (view == null) continue;
            // 1. 다음 레벨이 없거나 데이터가 잘못되면 구매 버튼과 품절 표시만 갱신합니다.
            GoodsData current = goodsDataManager.GetValidatedGoodsData(view.category);
            string id = current != null && current.level < int.MaxValue
                ? storeinfo_data.GetIDByCategoryAndLevel(view.category, current.level + 1) : null;
            bool available = storeinfo_data.TryGetItem(id, out StoreInfo_Object item);
            view.UpdateAvailability(id, available);
            if (!available) continue;

            // 2. 표시값과 이미지는 상품별 UI 객체가 담당합니다.
            view.ShowInformation(item.gold, item.effect ?? "");
            view.ShowLevelImage(item.level);
        }
    }

    private void RefreshInteriorViews(ProductView[] views)
    {
        foreach (ProductView view in views)
        {
            if (view == null) continue;
            bool exists = storeinfo_data.TryGetItem(view.productId, out StoreInfo_Object item);
            bool visible = exists && item.isButtonActive;
            InteriorData saved = visible ? interiorDataManager.GetInteriorDataByStoreInfoId(item.id) : null;
            if (view.content != null) view.content.SetActive(visible);
            view.UpdateAvailability(view.productId, visible && saved != null && !saved.isHaving);
            if (exists) view.ShowInformation(item.gold, (item.contents ?? "").Replace("nn", "\n"));
        }
    }

    public bool IsInteriorProduct(string id)
    {
        return storeinfo_data != null && storeinfo_data.TryGetItem(id, out StoreInfo_Object item) && item.theme != ItemTheme.Default;
    }

    /// <summary>확인창을 열기 위한 검사는 저장용 데이터 복사본을 만들지 않습니다.</summary>
    public bool CanBuy(string id, out string error)
    {
        error = "상점 초기화에 실패했습니다.";
        return Initialize() && purchaseService.CanBuy(id, out error);
    }

    /// <summary>구매를 확정하고 결과에 따른 전체 상품 표시만 갱신합니다.</summary>
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

    /// <summary>상품의 표시 참조와 표시 동작을 묶습니다. 데이터 검증과 구매 처리는 포함하지 않습니다.</summary>
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
        public Sprite[] levelImages;

        public void Bind(BuyCheck owner)
        {
            if (button != null) button.BindProduct(owner, productId);
        }

        public void UpdateAvailability(string id, bool available)
        {
            if (soldOut != null) soldOut.SetActive(!available);
            if (button != null) button.UpdateProduct(id, available);
        }

        public void ShowInformation(int gold, string text)
        {
            if (price != null) price.text = gold.ToString();
            if (description != null && text != null) description.text = text;
        }

        public void ShowLevelImage(int level)
        {
            if (image == null || levelImages == null || level < 0 || level >= levelImages.Length) return;
            Sprite sprite = levelImages[level];
            if (sprite != null && image.sprite != sprite) image.sprite = sprite;
        }

        /// <summary>기존 UI 이름으로 참조를 모읍니다. 씬 구조를 변경하지 않고 명시적 연결로 옮기기 위한 초기화 경로입니다.</summary>
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
        if (developmentViews == null) return;
        var categories = new HashSet<string>();
        foreach (ProductView view in developmentViews)
            if (view != null && (string.IsNullOrWhiteSpace(view.category) || !categories.Add(view.category)))
                Debug.LogWarning("[StoreData] 보조도구 카테고리를 중복 없이 지정해 주세요.", this);
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

/// <summary>유형별 구매 규칙을 선택하고 검증, 저장 확정, 메모리 반영의 공통 순서를 담당합니다.</summary>
public sealed class StorePurchaseService
{
    public const string SpecialFeedProductId = "SpecialFeed";
    private const string InvalidProductError = "상품 정보가 없거나 현재 구매할 수 없는 상품입니다.";
    private readonly JsonManager jsonManager;
    private readonly PlayerDataManager player;
    private readonly StoreInfo_Data table;
    private readonly GoodsDataManager goods;
    private readonly InteriorDataManager interior;
    private readonly PurchaseRule developmentRule;
    private readonly PurchaseRule interiorRule;
    private readonly PurchaseRule specialFeedRule;
    private bool purchasing;

    public StorePurchaseService(JsonManager jsonManager, PlayerDataManager player, GoodsDataManager goods,
        InteriorDataManager interior, StoreInfo_Data table, int specialFeedPrice)
    {
        this.jsonManager = jsonManager;
        this.player = player;
        this.goods = goods;
        this.interior = interior;
        this.table = table;
        // 규칙 객체는 매 클릭마다 만들지 않고 서비스 초기화 때 한 번 생성합니다.
        developmentRule = new DevelopmentPurchaseRule(goods, interior, table);
        interiorRule = new InteriorPurchaseRule(interior, table);
        specialFeedRule = new SpecialFeedPurchaseRule(specialFeedPrice);
    }

    public bool CanBuy(string id, out string error) => TryGetPurchasePlan(id, out _, out error);

    /// <summary>상품 선택은 짧은 조건문으로 처리하고 실제 검증은 해당 유형의 규칙에 위임합니다.</summary>
    private bool TryGetPurchasePlan(string id, out PurchasePlan plan, out string error)
    {
        plan = default;
        error = InvalidProductError;
        StoreInfo_Object product = null;
        PurchaseRule rule;
        if (id == SpecialFeedProductId) rule = specialFeedRule;
        else
        {
            if (!table.TryGetItem(id, out product)) return false;
            rule = product.theme == ItemTheme.Default ? developmentRule : interiorRule;
        }
        if (!rule.CanBuy(product)) return false;

        int cost = rule.GetCost(product);
        int reward = rule.SpecialFeedReward;
        if (!player.CanPurchase(cost, reward))
        {
            error = "골드가 부족하거나 상품 가격/보유 수량이 올바르지 않습니다.";
            return false;
        }
        plan = new PurchasePlan(product, rule, cost, reward);
        error = null;
        return true;
    }

    /// <summary>확인창 이후 상태가 바뀔 수 있으므로 최종 검증 뒤에만 복사와 저장을 수행합니다.</summary>
    public bool TryPurchase(string id, out string error)
    {
        error = "이미 구매를 처리하고 있습니다.";
        if (purchasing) return false;
        purchasing = true;
        try
        {
            // 1. 읽기 전용 검증을 다시 수행하고 실제 구매에서만 변경 후 데이터를 만듭니다.
            if (!TryGetPurchasePlan(id, out PurchasePlan plan, out error)) return false;
            if (!player.TryCreatePurchaseData(plan.Cost, plan.SpecialFeedReward, out PlayerData playerData)
                || !plan.Rule.TryCreateChanges(plan.Product, out PurchaseChanges changes))
            {
                error = InvalidProductError;
                return false;
            }
            var entries = new List<JsonManager.SaveEntry> { player.CreatePurchaseSaveEntry(playerData) };
            if (changes.Goods != null) entries.Add(changes.Goods.CreateSaveEntry());
            if (changes.Interior != null) entries.Add(changes.Interior.CreateSaveEntry());

            // 2. 저장 순서와 확정 시점은 상품 유형과 관계없이 한 곳에서 관리합니다.
            if (!jsonManager.TrySaveBatch(entries))
            {
                error = "구매 정보를 저장할 수 없습니다. 재화와 상품은 변경되지 않았습니다.";
                return false;
            }
            if (changes.Goods != null) goods.ApplyPurchaseData(changes.Goods);
            if (changes.Interior != null) interior.ApplyPurchaseData(changes.Interior);
            player.ApplyPurchaseData(playerData);
            return true;
        }
        finally { purchasing = false; }
    }

    /// <summary>검증된 상품과 가격을 구매 처리에 전달하는 값 형식이며 데이터 복사본은 포함하지 않습니다.</summary>
    private readonly struct PurchasePlan
    {
        public readonly StoreInfo_Object Product;
        public readonly PurchaseRule Rule;
        public readonly int Cost;
        public readonly int SpecialFeedReward;

        public PurchasePlan(StoreInfo_Object product, PurchaseRule rule, int cost, int reward)
        {
            Product = product;
            Rule = rule;
            Cost = cost;
            SpecialFeedReward = reward;
        }
    }

    /// <summary>해당 유형에서 변경하는 데이터만 공통 저장 흐름에 전달합니다.</summary>
    private readonly struct PurchaseChanges
    {
        public readonly GoodsDataManager Goods;
        public readonly InteriorDataManager Interior;

        public PurchaseChanges(GoodsDataManager goods, InteriorDataManager interior)
        {
            Goods = goods;
            Interior = interior;
        }
    }

    /// <summary>유형별로 달라지는 구매 가능 조건과 변경 데이터 생성만 분리합니다.</summary>
    private abstract class PurchaseRule
    {
        public virtual int GetCost(StoreInfo_Object product) => product.gold;
        public virtual int SpecialFeedReward => 0;
        public abstract bool CanBuy(StoreInfo_Object product);
        public abstract bool TryCreateChanges(StoreInfo_Object product, out PurchaseChanges changes);
    }

    /// <summary>보조도구는 다음 레벨만 구매하며, 횃대 외의 보조도구는 구매와 배치를 함께 확정합니다.</summary>
    private sealed class DevelopmentPurchaseRule : PurchaseRule
    {
        private readonly GoodsDataManager goods;
        private readonly InteriorDataManager interior;
        private readonly StoreInfo_Data table;

        public DevelopmentPurchaseRule(GoodsDataManager goods, InteriorDataManager interior, StoreInfo_Data table)
        {
            this.goods = goods;
            this.interior = interior;
            this.table = table;
        }

        public override bool CanBuy(StoreInfo_Object product)
        {
            if (product == null) return false;
            // enum의 숫자 순서 대신 현재 보조도구 종류를 명시합니다.
            switch (product.category)
            {
                case StoreItemCategory.Rack:
                case StoreItemCategory.Vase:
                case StoreItemCategory.Box:
                case StoreItemCategory.Thread:
                    return table.GetIDByCategoryAndLevel(product.category.ToString(), product.level) == product.id
                        && goods.CanUpgrade(product)
                        && (product.category == StoreItemCategory.Rack || interior.CanPurchase(product));
                default:
                    return false;
            }
        }

        public override bool TryCreateChanges(StoreInfo_Object product, out PurchaseChanges changes)
        {
            changes = default;
            if (!goods.TryCreateUpgradeData(product, out GoodsDataManager upgraded)) return false;
            InteriorDataManager purchased = null;
            // 횃대는 기존 데이터 구조에 인테리어 보유 항목이 없어 레벨만 변경합니다.
            if (product.category != StoreItemCategory.Rack
                && !interior.TryCreatePurchaseData(product, table, true, out purchased)) return false;
            changes = new PurchaseChanges(upgraded, purchased);
            return true;
        }
    }

    /// <summary>인테리어는 미보유 상품만 구매하고, 실제 배치는 별도의 확인창에서 선택합니다.</summary>
    private sealed class InteriorPurchaseRule : PurchaseRule
    {
        private readonly InteriorDataManager interior;
        private readonly StoreInfo_Data table;

        public InteriorPurchaseRule(InteriorDataManager interior, StoreInfo_Data table)
        {
            this.interior = interior;
            this.table = table;
        }

        public override bool CanBuy(StoreInfo_Object product)
        {
            return product != null && product.isButtonActive && interior.CanPurchase(product);
        }

        public override bool TryCreateChanges(StoreInfo_Object product, out PurchaseChanges changes)
        {
            changes = default;
            if (!interior.TryCreatePurchaseData(product, table, false, out InteriorDataManager purchased)) return false;
            changes = new PurchaseChanges(null, purchased);
            return true;
        }
    }

    /// <summary>특제먹이는 제한 없이 반복 구매하며 설정된 가격으로 한 개씩 플레이어에게 지급합니다.</summary>
    private sealed class SpecialFeedPurchaseRule : PurchaseRule
    {
        private readonly int price;

        public SpecialFeedPurchaseRule(int price) => this.price = price;
        public override int GetCost(StoreInfo_Object product) => price;
        public override int SpecialFeedReward => 1;
        public override bool CanBuy(StoreInfo_Object product) => true; // 재고 제한은 없고 재화는 공통 흐름에서 검증합니다.

        public override bool TryCreateChanges(StoreInfo_Object product, out PurchaseChanges changes)
        {
            changes = default; // 특제먹이는 플레이어 재화 데이터만 변경합니다.
            return true;
        }
    }
}

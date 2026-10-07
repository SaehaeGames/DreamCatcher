using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>상점 초기화와 상품 화면 갱신을 담당합니다. 구매 규칙과 저장은 구매 서비스에 위임합니다.</summary>
public class StoreData : MonoBehaviour
{
    [Header("[Store Product]")]
    public StoreInfo_Data storeinfo_data;

    [Header("상품 ID와 UI 직접 연결")]
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
    public IReadOnlyList<ProductView> DevelopmentViews => developmentViews;
    public ProductView SpecialFeedView => specialFeedView;

    /// <summary>에디터 검증에서 실제 테마별 연결을 읽습니다. 목록을 새로 생성하거나 재정렬하지 않습니다.</summary>
    public IReadOnlyList<ProductView> GetInteriorViews(ItemTheme theme)
    {
        return theme == ItemTheme.Star ? starViews : theme == ItemTheme.Sea ? seaViews : null;
    }

    private void Start() => Initialize();

    /// <summary>참조 준비, UI 연결, 첫 화면 갱신을 순서대로 수행하며 중복 초기화는 생략합니다.</summary>
    public bool Initialize()
    {
        if (initialized) return true;
        if (!TryInitializeReferences()) return false;

        // 1. 씬에 저장된 명시적 연결을 검사합니다. 테이블 순서로 상품 ID를 추측하지 않습니다.
        if (!ValidateProductBindings()) return false;
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

    /// <summary>상품 ID와 버튼이 연결된 구성만 실행하여 잘못된 상품을 대신 판매하지 않습니다.</summary>
    private bool ValidateProductBindings()
    {
        bool valid = HasBindings(developmentViews) && HasBindings(starViews) && HasBindings(seaViews)
            && specialFeedView != null && specialFeedView.button != null
            && specialFeedView.productId == StorePurchaseService.SpecialFeedProductId;
        if (!valid)
            Debug.LogError("[StoreData] 상품 ID와 버튼을 Inspector에서 연결한 뒤 상점 상품 검증 메뉴를 실행해 주세요.", this);
        return valid;
    }

    private static bool HasBindings(ProductView[] views)
    {
        if (views == null || views.Length == 0) return false;
        foreach (ProductView view in views)
            if (view == null || view.button == null || string.IsNullOrWhiteSpace(view.productId)) return false;
        return true;
    }

    private void BindButtons(ProductView[] views)
    {
        foreach (ProductView view in views) view.Bind(buyCheck);
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
            // 1. 고정된 기본 상품 ID로 카테고리를 얻고, 저장된 레벨의 다음 상품을 찾습니다.
            // 기본 상품 ID는 변경하지 않고, 실제 구매 버튼의 ID만 다음 레벨 상품으로 갱신합니다.
            bool configured = storeinfo_data.TryGetItem(view.productId, out StoreInfo_Object baseItem)
                && baseItem.theme == ItemTheme.Default;
            string category = configured ? baseItem.category.ToString() : null;
            GoodsData current = goodsDataManager.GetValidatedGoodsData(category);
            string id = current != null && current.level < int.MaxValue
                ? storeinfo_data.GetIDByCategoryAndLevel(category, current.level + 1) : null;
            bool available = storeinfo_data.TryGetItem(id, out StoreInfo_Object item);
            view.UpdateAvailability(id, available);

            // 2. 다음 상품이 없으면 현재 레벨을 표시합니다. 재진입 후에도 최대 레벨 이미지가 유지됩니다.
            if (!available && current != null)
                storeinfo_data.TryGetItem(storeinfo_data.GetIDByCategoryAndLevel(category, current.level), out item);
            view.ShowInformation(item != null ? item.gold : 0, item?.effect ?? "", available);
            if (current != null) view.ShowLevelImage(available ? item.level : current.level);
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

    /// <summary>상품의 표시 참조와 표시 동작을 묶습니다. 데이터 검증과 구매 처리는 포함하지 않습니다.</summary>
    [Serializable]
    public class ProductView
    {
        public string productId;
        public Image image;
        public Text price;
        public Text description;
        public BuyButtonInfo button;
        public GameObject soldOut;
        public GameObject content;
        public GameObject priceRoot;
        [Tooltip("효과 값 뒤에 붙이는 고정 문구입니다. 실 색상에는 ' 실'을 설정합니다.")]
        public string descriptionSuffix;
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

        /// <summary>변경되는 효과 값만 갱신합니다. 단위는 별도 Text 또는 설정한 접미사로 유지합니다.</summary>
        public void ShowInformation(int gold, string text, bool showPrice = true)
        {
            // 최대 레벨에서는 숫자뿐 아니라 골드 아이콘을 포함한 가격 영역 전체를 숨깁니다.
            if (priceRoot != null) priceRoot.SetActive(showPrice);
            else if (price != null) price.gameObject.SetActive(showPrice);
            if (price != null) price.text = gold.ToString();
            if (description != null && text != null) description.text = text + descriptionSuffix;
        }

        public void ShowLevelImage(int level)
        {
            if (image == null || levelImages == null || level < 0 || level >= levelImages.Length) return;
            Sprite sprite = levelImages[level];
            if (sprite != null && image.sprite != sprite) image.sprite = sprite;
        }

    }

    private void OnValidate()
    {
        ValidateViews(developmentViews);
        ValidateViews(starViews);
        ValidateViews(seaViews);
        if (specialFeedView != null && specialFeedView.button == null)
            Debug.LogWarning("[StoreData] 특제먹이 상품 버튼 참조가 누락되었습니다.", this);
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

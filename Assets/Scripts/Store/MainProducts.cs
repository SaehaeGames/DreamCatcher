using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main의 보조도구와 장식 이미지를 표시합니다.
/// 인테리어 메뉴의 연결 상태와 관계없이 저장된 최종 외형을 한 번에 적용합니다.
/// </summary>
public class MainProducts : MonoBehaviour
{
    [Header("종류와 화면 이미지 직접 연결")]
    [SerializeField] private DisplayBinding[] displayBindings;

    private StoreInfo_Data storeInfo;
    private GoodsDataManager goods;
    private InteriorDataManager interior;
    private bool initialized;

    public IReadOnlyList<DisplayBinding> DisplayBindings => displayBindings;

    /// <summary>메뉴 버튼·탭의 설정 오류와 독립적으로 첫 화면의 저장 외형을 준비합니다.</summary>
    private void Start()
    {
        // 1. 메뉴에서 먼저 초기화한 경우 중복 조회와 이미지 변경을 생략합니다.
        if (initialized) return;
        GameManager manager = GameManager.instance;
        if (manager == null || !manager.IsInitialized)
        {
            ReportError("게임 데이터 초기화가 필요합니다.");
            return;
        }

        // 2. 모든 Awake에서 데이터 로드가 끝난 뒤 실행되므로 첫 렌더링 전에 최종 외형을 적용할 수 있습니다.
        Initialize(manager.storeinfo_data, manager.goodsDataManager, manager.interiorDataManager);
    }

    /// <summary>조회에 필요한 데이터를 전달받고 모든 초기 이미지를 검증한 뒤 함께 표시합니다.</summary>
    public bool Initialize(StoreInfo_Data table, GoodsDataManager goodsData, InteriorDataManager interiorData)
    {
        if (initialized) return true;
        if (table == null || goodsData == null || interiorData == null || displayBindings == null || displayBindings.Length == 0)
            return ReportError("데이터 또는 Main 이미지 연결이 없습니다.");

        // 1. 화면이 별도의 저장 파일을 읽거나 공통 매니저를 교체하지 않도록 참조만 전달받습니다.
        storeInfo = table;
        goods = goodsData;
        interior = interiorData;

        // 2. 같은 Image에 서로 다른 외형이 덮어써지지 않도록 중복 연결과 초기 표시 대상을 검사합니다.
        var images = new HashSet<Image>();
        foreach (DisplayBinding binding in displayBindings)
        {
            if (binding == null || binding.image == null || !images.Add(binding.image)
                || !Enum.IsDefined(typeof(StoreItemCategory), binding.category))
                return ReportError("Main 이미지가 누락되거나 중복 연결되었습니다.");
            if (!TryGetDisplayedProduct(binding, out StoreInfo_Object product, out string error)
                || !binding.TryGetSprite(product, out _))
                return ReportError(error ?? "초기 외형의 Sprite가 없습니다: " + binding.category);
        }

        // 3. 모든 초기 외형이 유효한 경우에만 적용합니다. Start 순서를 맞추는 지연 코루틴은 필요하지 않습니다.
        initialized = true;
        return RefreshAll(out _);
    }

    /// <summary>전체 표시 대상을 먼저 검사하여 일부 이미지만 변경되는 초기화 실패를 방지합니다.</summary>
    public bool RefreshAll(out string error)
    {
        error = null;
        if (!initialized || displayBindings == null || displayBindings.Length == 0)
        { error = "Main 이미지가 초기화되지 않았습니다."; return false; }

        // 1. 표시할 상품과 이미지가 모두 있는지 확인합니다.
        foreach (DisplayBinding binding in displayBindings)
            if (binding == null || binding.image == null || !TryGetDisplayedProduct(binding, out StoreInfo_Object product, out error)
                || !binding.TryGetSprite(product, out _))
            {
                error = error ?? "Main 이미지 연결 또는 Sprite가 없습니다.";
                return ReportError(error);
            }

        // 2. 검증된 데이터로만 화면을 갱신합니다. 벽지에 연결된 여러 Image도 같은 종류로 처리합니다.
        foreach (DisplayBinding binding in displayBindings)
        {
            TryGetDisplayedProduct(binding, out StoreInfo_Object product, out _);
            binding.TryGetSprite(product, out Sprite sprite);
            binding.Apply(sprite);
        }
        return true;
    }

    /// <summary>저장하기 전에 해당 외형을 표시할 모든 Image와 Sprite가 준비되어 있는지 확인합니다.</summary>
    public bool TryValidateProduct(StoreInfo_Object product, out string error)
    {
        error = "상품 또는 Main 이미지 초기화가 올바르지 않습니다.";
        if (!initialized || product == null || displayBindings == null) return false;
        bool found = false;
        foreach (DisplayBinding binding in displayBindings)
        {
            if (binding == null) { error = "Main 이미지 연결이 없습니다."; return false; }
            if (binding.category != product.category) continue;
            found = true;
            if (binding.image == null || !binding.TryGetSprite(product, out _))
            {
                error = "적용할 외형의 Image 또는 Sprite가 없습니다: " + product.id;
                return false;
            }
        }
        error = found ? null : "상품 종류에 연결된 Main 이미지가 없습니다: " + product.category;
        return found;
    }

    /// <summary>저장 성공 후 해당 종류의 이미지만 갱신합니다. 이미지가 같은 경우 재설정하지 않습니다.</summary>
    public bool TryApplyProduct(StoreInfo_Object product, out string error)
    {
        if (!TryValidateProduct(product, out error)) return false;
        foreach (DisplayBinding binding in displayBindings)
            if (binding.category == product.category)
            {
                binding.TryGetSprite(product, out Sprite sprite);
                binding.Apply(sprite);
            }
        return true;
    }

    /// <summary>횃대는 구매 레벨을, 나머지는 저장된 선택 외형 또는 기본 장식을 조회합니다.</summary>
    private bool TryGetDisplayedProduct(DisplayBinding binding, out StoreInfo_Object product, out string error)
    {
        product = null;
        error = null;
        if (binding.category == StoreItemCategory.Rack)
        {
            GoodsData rack = goods.GetGoodsDataByID(binding.goodsId);
            if (rack == null || rack.level < 0 || !string.Equals(rack.category?.Trim(), Constants.GoodsData_Rack, StringComparison.OrdinalIgnoreCase)
                || !storeInfo.TryGetItem(storeInfo.GetIDByCategoryAndLevel(Constants.GoodsData_Rack, rack.level), out product))
            {
                error = "횃대의 저장 ID 또는 레벨이 올바르지 않습니다: " + binding.goodsId;
                return false;
            }
            return true;
        }
        if (interior.TryGetDisplayedProduct(binding.category, storeInfo, out product)) return true;
        error = "선택 외형 또는 기본 장식이 없습니다: " + binding.category;
        return false;
    }

    private bool ReportError(string error)
    {
        Debug.LogError("[MainProducts] " + error, this);
        return false;
    }

    /// <summary>
    /// 화면 Image 하나와 해당 종류의 외형을 묶습니다.
    /// 벽지처럼 한 상품이 여러 화면을 바꾸면 같은 category의 연결을 여러 개 설정합니다.
    /// </summary>
    [Serializable]
    public class DisplayBinding
    {
        public StoreItemCategory category;
        [Tooltip("횃대 앞/뒤를 구분하는 GoodsData ID입니다. 다른 종류는 선택된 인테리어 데이터를 사용합니다.")]
        public string goodsId;
        public Image image;
        [Tooltip("보조도구는 레벨 순서, 장식은 Default / Sea / Star 순서입니다.")]
        public Sprite[] sprites;

        /// <summary>저장 목록의 위치가 아니라 상품의 레벨 또는 테마로 Sprite를 선택합니다.</summary>
        public bool TryGetSprite(StoreInfo_Object product, out Sprite sprite)
        {
            sprite = null;
            if (product == null || product.category != category || sprites == null) return false;
            int index;
            if (category == StoreItemCategory.Rack || InteriorDataManager.IsDevelopmentCategory(category))
            {
                // 현재 보조도구는 기본 테마의 레벨 이미지입니다. 새 테마를 기본 이미지로 잘못 표시하지 않습니다.
                if (product.theme != ItemTheme.Default) return false;
                index = product.level;
            }
            else
            {
                switch (product.theme)
                {
                    case ItemTheme.Default: index = 0; break;
                    case ItemTheme.Sea: index = 1; break;
                    case ItemTheme.Star: index = 2; break;
                    default: return false;
                }
            }
            if (index < 0 || index >= sprites.Length || sprites[index] == null) return false;
            sprite = sprites[index];
            return true;
        }

        /// <summary>검증한 Sprite가 실제 표시와 다를 때만 Image를 변경합니다.</summary>
        public void Apply(Sprite sprite)
        {
            if (image != null && image.sprite != sprite) image.sprite = sprite;
        }
    }
}

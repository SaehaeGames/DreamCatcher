using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인테리어 탭과 상품 표시를 관리합니다.
/// 보유·선택·저장은 데이터 매니저에, Main 외형 적용은 MainProducts에 맡깁니다.
/// </summary>
public class InteriorCategory : MonoBehaviour
{
    private const int DefaultTabIndex = 0;

    [Header("탭과 상품 ID 직접 연결")]
    [SerializeField] private InteriorTab[] tabs;
    [SerializeField] private InteriorItemView[] itemViews;
    [SerializeField] private MainProducts mainProducts;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Text errorText;

    public int currentCategoryIndex;
    private StoreInfo_Data storeInfo;
    private InteriorDataManager interior;
    private bool initialized;

    public bool IsInitialized => initialized;
    public string LastError { get; private set; }
    public IReadOnlyList<InteriorTab> Tabs => tabs;
    public IReadOnlyList<InteriorItemView> ItemViews => itemViews;
    public MainProducts MainProducts => mainProducts;
    public ScrollRect ScrollRect => scrollRect;

    private void Start() => Initialize();

    /// <summary>명시적 연결과 데이터를 준비하고 최초 외형·잠금·적용 표시를 한 번에 갱신합니다.</summary>
    public bool Initialize()
    {
        if (initialized) return true;
        GameManager manager = GameManager.instance;
        if (manager == null || !manager.IsInitialized || manager.storeinfo_data == null
            || manager.goodsDataManager == null || manager.interiorDataManager == null)
            return ReportError("게임 데이터 초기화가 필요합니다.");

        // 1. 초기화에서 준비한 공통 데이터를 사용합니다. 화면을 열 때 세이브를 다시 읽지 않습니다.
        storeInfo = manager.storeinfo_data;
        interior = manager.interiorDataManager;

        // 2. 메뉴 연결 오류가 저장된 Main 외형 적용을 막지 않도록 표시 초기화를 먼저 요청합니다.
        // MainProducts도 자체 Start에서 초기화하므로 메뉴의 MainProducts 참조가 없어도 Main 표시는 독립적으로 준비됩니다.
        if (mainProducts == null || !mainProducts.Initialize(storeInfo, manager.goodsDataManager, interior))
            return ReportError("Main 외형 연결을 확인해 주세요.");
        if (!ValidateBindings()) return false;

        // 3. 비활성 탭의 버튼도 담당 객체와 고정 ID를 전달받습니다. OnClick은 Inspector 연결만 사용합니다.
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].button.buttonNumber = i;
            tabs[i].button.Initialize(this);
        }
        foreach (InteriorItemView view in itemViews)
        {
            view.button.itemID = view.productId;
            view.button.Initialize(this);
        }

        // 4. 씬에 남은 탭 인덱스와 관계없이 최초 화면은 기본 탭과 목록 상단으로 통일합니다.
        initialized = true;
        UpdateCategoryPanel(DefaultTabIndex);
        RefreshItemViews();
        ClearError();
        return true;
    }

    /// <summary>패널 재열기 시 현재 데이터로 표시를 갱신하고 기본 탭의 상단에서 다시 시작합니다.</summary>
    public bool RefreshPanel()
    {
        if (!initialized) return Initialize();
        if (!mainProducts.RefreshAll(out string error)) return ReportError(error);
        RefreshItemViews();
        UpdateCategoryPanel(DefaultTabIndex);
        ClearError();
        return true;
    }

    /// <summary>탭 활성 상태와 스크롤 대상을 함께 변경합니다. 목록의 상품 ID를 재정렬하지 않습니다.</summary>
    public void UpdateCategoryPanel(int index)
    {
        if (!Initialize()) return;
        if (index < 0 || index >= tabs.Length) { ReportError("선택한 인테리어 탭이 없습니다."); return; }

        // 1. 열린 패널 안에서는 선택한 탭을 표시합니다. 패널 재열기는 RefreshPanel에서 기본 탭을 선택합니다.
        currentCategoryIndex = index;
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].content.gameObject.SetActive(i == index);
            tabs[i].unselectedLine.SetActive(i != index);
        }

        // 2. 화면에 보이는 목록을 스크롤 대상으로 설정하고 이전 목록의 관성이 이어지지 않게 합니다.
        scrollRect.StopMovement();
        scrollRect.content = tabs[index].content;
        RectTransform content = scrollRect.content;
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0f);
    }

    /// <summary>선택 결과를 먼저 검증하고 저장이 성공한 경우에만 외형과 적용 표시를 변경합니다.</summary>
    public void SelectInteriorItem(string productId)
    {
        if (!Initialize()) return;

        // 1. 적용할 상품 또는 기본 복귀 대상을 확인합니다. 미보유 상품과 없는 기본 외형은 여기서 차단합니다.
        if (!interior.TryGetSelectionTarget(productId, storeInfo, out StoreInfo_Object target, out string error)
            || !mainProducts.TryValidateProduct(target, out error))
        {
            ReportError(error);
            return;
        }

        // 2. 데이터 매니저가 복사본의 저장을 먼저 확정합니다. 저장 실패 시 기존 데이터와 화면을 그대로 유지합니다.
        if (!interior.TryToggleItem(productId, storeInfo, out error))
        {
            ReportError(error);
            return;
        }

        // 3. 확정한 종류의 외형만 바꾸고 보유·적용 표시를 갱신합니다.
        if (!mainProducts.TryApplyProduct(target, out error)) { ReportError(error); return; }
        RefreshItemViews();
        ClearError();
    }

    /// <summary>불완전한 씬 연결로 다른 상품을 대신 선택하지 않도록 초기화 전에 검사합니다.</summary>
    private bool ValidateBindings()
    {
        if (scrollRect == null || tabs == null || tabs.Length == 0 || itemViews == null || itemViews.Length == 0)
            return ReportError("탭, 상품 또는 스크롤 참조가 없습니다.");

        var contents = new HashSet<RectTransform>();
        var buttons = new HashSet<InteriorButton>();
        foreach (InteriorTab tab in tabs)
            if (tab == null || tab.button == null || tab.button.GetComponent<Button>() == null
                || tab.content == null || !contents.Add(tab.content) || !buttons.Add(tab.button) || tab.unselectedLine == null)
                return ReportError("탭의 버튼·목록·표시 참조가 누락되거나 중복되었습니다.");

        var productIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (InteriorItemView view in itemViews)
            if (view == null || view.button == null || view.button.GetComponent<Button>() == null
                || view.locked == null || view.applying == null || !buttons.Add(view.button)
                || !productIds.Add(view.productId ?? "") || !storeInfo.TryGetItem(view.productId, out StoreInfo_Object product)
                || product.category == StoreItemCategory.Rack)
                return ReportError("상품 ID 또는 상품 UI 참조가 누락되거나 중복되었습니다.");
        return true;
    }

    /// <summary>저장 목록의 개수·순서와 무관하게 각 상품 ID의 상태를 표시합니다.</summary>
    private void RefreshItemViews()
    {
        foreach (InteriorItemView view in itemViews)
        {
            InteriorData item = interior.GetInteriorDataByStoreInfoId(view.productId);
            bool owned = item != null && item.isHaving;
            view.locked.SetActive(!owned);
            view.applying.SetActive(owned && item.isAdjusting);
            view.button.SetInteractable(owned);
        }
    }

    /// <summary>연결된 오류 Text 또는 Console로 실패 원인을 알립니다.</summary>
    private bool ReportError(string error)
    {
        LastError = error ?? "인테리어 설정을 확인해 주세요.";
        if (errorText != null) errorText.text = LastError;
        Debug.LogWarning("[InteriorCategory] " + LastError, this);
        return false;
    }

    private void ClearError()
    {
        LastError = null;
        if (errorText != null) errorText.text = "";
    }

    /// <summary>카테고리 버튼과 해당 목록, 미선택 표시를 묶는 Inspector 설정입니다.</summary>
    [Serializable]
    public class InteriorTab
    {
        public InteriorButton button;
        public RectTransform content;
        public GameObject unselectedLine;
    }

    /// <summary>상품 ID와 잠금·적용 표시를 묶습니다. 배열 순서는 화면 데이터의 의미로 사용하지 않습니다.</summary>
    [Serializable]
    public class InteriorItemView
    {
        public string productId;
        public InteriorButton button;
        public GameObject locked;
        public GameObject applying;
    }
}

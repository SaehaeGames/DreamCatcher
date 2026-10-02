using UnityEngine;

/// <summary>상품 선택과 확인 팝업을 관리하며, 구매 및 배치는 데이터 담당 코드에 위임합니다.</summary>
public class BuyCheck : MonoBehaviour
{
    [Header("[Buy Check]")]
    [SerializeField] private string selectId;
    public GameObject[] ButtonObj;
    public GameObject[] StarButtonObj;
    public GameObject[] SeaButtonObj;
    public GameObject panel_buyCheck;
    public GameObject panel_AdjustCheck;
    [SerializeField] private UnityEngine.UI.Text purchaseErrorText;

    private StoreData storeData;
    private InteriorDataManager interiorDataManager;
    private StoreInfo_Data storeInfoData;
    private string purchasedInteriorId;

    /// <summary>상점의 초기화 진입점에서 구매 화면에 필요한 담당 객체를 전달합니다.</summary>
    public void Initialize(StoreData owner, InteriorDataManager interior, StoreInfo_Data table)
    {
        storeData = owner;
        interiorDataManager = interior;
        storeInfoData = table;
        // 선택은 이번 실행에서만 유효하며, Inspector에 남은 ID를 구매 요청으로 취급하지 않습니다.
        selectId = null;
        purchasedInteriorId = null;
    }

    /// <summary>현재 구매 가능한 상품만 확인창에 전달합니다.</summary>
    public void SelectBuyingGoods(string id)
    {
        // 1. 이미 확인 중인 상품을 다른 클릭으로 바꾸지 않고, 두 확인창이 겹치지 않게 합니다.
        if (panel_buyCheck == null || !string.IsNullOrEmpty(selectId)
            || !string.IsNullOrEmpty(purchasedInteriorId)) return;

        // 2. 최종 구매와 같은 검증을 사용하여 현재 구매할 수 없는 상품은 선택하지 않습니다.
        string error = null;
        if (storeData == null || !storeData.CanBuy(id, out error))
        {
            ShowError(storeData == null ? "상점 참조가 없습니다." : error);
            return;
        }

        // 3. 상품 ID 자체가 선택 상태를 나타내므로 별도의 번호나 선택 여부를 중복 보관하지 않습니다.
        selectId = id;
        ShowError(null);
        panel_buyCheck.SetActive(true);
    }

    /// <summary>구매 성공 시에만 선택을 소비하고 실제 구매한 인테리어의 배치 확인창을 엽니다.</summary>
    public void SelectYesButton()
    {
        // 1. 선택한 ID를 기준으로 구매를 확정하며 실패하면 확인창과 선택을 유지합니다.
        if (string.IsNullOrEmpty(selectId) || storeData == null) return;
        string purchasedId = selectId;
        if (!storeData.BuyGoods(purchasedId))
        {
            ShowError(storeData.LastPurchaseError);
            return;
        }

        // 2. 성공한 선택을 먼저 지워 같은 확인 이벤트가 반복되어도 재구매하지 않습니다.
        SetInActiveBuyCheckPanel();
        if (storeData.IsInteriorProduct(purchasedId) && panel_AdjustCheck != null)
        {
            purchasedInteriorId = purchasedId;
            SetPanel(panel_AdjustCheck, true);
        }
    }

    /// <summary>실제로 구매한 인테리어를 배치하고 저장에 성공한 경우에만 확인창을 닫습니다.</summary>
    public void SelectYesButton_Adjust()
    {
        if (string.IsNullOrEmpty(purchasedInteriorId) || interiorDataManager == null) return;
        if (!interiorDataManager.TryAdjustItem(purchasedInteriorId, storeInfoData))
        {
            ShowError("배치 상태를 저장할 수 없습니다.");
            return;
        }
        SelectNoButton_Adjust();
    }

    /// <summary>배치할 상품 선택을 지우고 확인창을 닫습니다. 구매한 보유 상태는 유지합니다.</summary>
    public void SelectNoButton_Adjust()
    {
        purchasedInteriorId = null;
        SetPanel(panel_AdjustCheck, false);
    }

    /// <summary>연결된 오류 문구를 갱신하고, 미연결 상태에서는 Console에 원인을 표시합니다.</summary>
    private void ShowError(string message)
    {
        if (purchaseErrorText != null) purchaseErrorText.text = message ?? "";
        else if (!string.IsNullOrEmpty(message)) Debug.LogWarning("[BuyCheck] " + message, this);
    }

    private static void SetPanel(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }

    /// <summary>구매를 취소하거나 완료한 뒤 선택과 확인창을 함께 정리합니다. 씬 버튼이 연결한 이름을 유지합니다.</summary>
    public void SetInActiveBuyCheckPanel()
    {
        selectId = null;
        ShowError(null);
        SetPanel(panel_buyCheck, false);
    }
}

using UnityEngine;

/// <summary>상품 선택과 확인 팝업을 관리하며, 구매 및 배치는 데이터 담당 코드에 위임합니다.</summary>
public class BuyCheck : MonoBehaviour
{
    [Header("[Buy Check]")]
    [SerializeField] private int selectGoods;
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
    private bool hasSelection;

    public void Initialize(StoreData owner, InteriorDataManager interior, StoreInfo_Data table)
    {
        storeData = owner;
        interiorDataManager = interior;
        storeInfoData = table;
    }

    private void Start()
    {
        if (storeData == null) storeData = GetComponent<StoreData>();
        if (storeData != null) storeData.Initialize();
    }

    public void SetDevelopmentButtonId(int index, string id)
    {
        if (ButtonObj == null || index < 0 || index >= ButtonObj.Length || ButtonObj[index] == null) return;
        BuyButtonInfo info = ButtonObj[index].GetComponent<BuyButtonInfo>();
        if (info != null) info.SetSelectGoodsId(id);
    }

    /// <summary>현재 구매 가능한 상품만 확인창에 전달합니다.</summary>
    public void SelectBuyingGoods(int buttonNumber, string id)
    {
        string error = null;
        if (storeData == null || !storeData.CanBuy(id, out error))
        {
            ShowError(storeData == null ? "상점 참조가 없습니다." : error);
            return;
        }
        if (panel_buyCheck == null) return;
        selectGoods = buttonNumber;
        selectId = id;
        hasSelection = true;
        ShowError(null);
        panel_buyCheck.SetActive(true);
    }

    /// <summary>구매 성공 시에만 선택을 소비하고 실제 구매한 인테리어의 배치 확인창을 엽니다.</summary>
    public void SelectYesButton()
    {
        if (!hasSelection || storeData == null) return;
        if (!storeData.BuyGoods(selectId))
        {
            ShowError(storeData.LastPurchaseError);
            return;
        }

        hasSelection = false; // 같은 확인 이벤트가 다시 호출되어도 재구매하지 않습니다.
        SetPanel(panel_buyCheck, false);
        if (storeData.IsInteriorProduct(selectId))
        {
            purchasedInteriorId = selectId;
            SetPanel(panel_AdjustCheck, true);
        }
    }

    public void SelectYesButton_Adjust()
    {
        if (string.IsNullOrEmpty(purchasedInteriorId) || interiorDataManager == null) return;
        if (!interiorDataManager.TryAdjustItem(purchasedInteriorId, storeInfoData))
        {
            ShowError("배치 상태를 저장할 수 없습니다.");
            return;
        }
        purchasedInteriorId = null;
        SetPanel(panel_AdjustCheck, false);
    }

    public void SelectNoButton_Adjust()
    {
        purchasedInteriorId = null;
        SetPanel(panel_AdjustCheck, false);
    }

    private void ShowError(string message)
    {
        if (purchaseErrorText != null) purchaseErrorText.text = message ?? "";
        else if (!string.IsNullOrEmpty(message)) Debug.LogWarning("[BuyCheck] " + message, this);
    }

    private static void SetPanel(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }

    public void SetActiveBuyCheckPanel() => SetPanel(panel_buyCheck, true);
    public void SetInActiveBuyCheckPanel()
    {
        hasSelection = false;
        SetPanel(panel_buyCheck, false);
    }
    public void SetActiveAdjustCheckPanel() => SetPanel(panel_AdjustCheck, true);
    public void SetInActiveAdjustCheckPanel() => SelectNoButton_Adjust();
}

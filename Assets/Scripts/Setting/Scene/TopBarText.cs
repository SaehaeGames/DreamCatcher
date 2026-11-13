using UnityEngine;
using UnityEngine.UI;

/// <summary>재화 변경 알림을 받아 활성화된 상단바 텍스트를 즉시 갱신합니다.</summary>
public class TopBarText : MonoBehaviour
{
    public Text DreamMarbleText;
    public Text GoldText;
    public Text SpecialFeedText;
    private PlayerDataManager playerDataManager;

    private void Start() => SubscribeAndRefresh();

    private void OnEnable()
    {
        // 첫 OnEnable 시 다른 객체의 Awake가 아직 실행되지 않았을 수 있어 Start에서도 준비합니다.
        SubscribeAndRefresh();
    }

    private void OnDisable()
    {
        if (playerDataManager != null) playerDataManager.OnCurrencyChanged -= UpdateText;
        playerDataManager = null;
    }

    private void SubscribeAndRefresh()
    {
        GameManager manager = GameManager.instance;
        if (manager == null || !manager.IsInitialized) return;
        if (playerDataManager != manager.playerDataManager)
        {
            if (playerDataManager != null) playerDataManager.OnCurrencyChanged -= UpdateText;
            playerDataManager = manager.playerDataManager;
            playerDataManager.OnCurrencyChanged += UpdateText;
        }
        UpdateText();
    }

    public void UpdateText()
    {
        if (playerDataManager == null) return;
        SetText(DreamMarbleText, playerDataManager.GetDreamMarble());
        SetText(GoldText, playerDataManager.GetGold());
        SetText(SpecialFeedText, playerDataManager.GetSpecialFeed());
    }

    private static void SetText(Text text, int value)
    {
        if (text != null && text.isActiveAndEnabled) text.text = value.ToString();
    }
}

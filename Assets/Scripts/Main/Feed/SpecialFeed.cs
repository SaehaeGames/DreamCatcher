using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialFeed : MonoBehaviour
{
    //스페셜 먹이 개수를 관리하고 사용하는 클래스

    [Header("[Special Feed]")]
    [SerializeField] private int feedCount;    //특제 먹이 개수
    [SerializeField] private int selectCount;    //선택한 먹이 수
    [SerializeField] private float decreaseTime;   //감소시키는 시간
    private int currentRackIndex;    //현재 특제 먹이를 사용할 횃대 번호

    [SerializeField] private PlayerDataManager playerDataManager;   //상품 정보
    [SerializeField] private FeedTimer feedTimer;
    [SerializeField] private FeedPanel feedPanel;
    [SerializeField] private TopBarText topBarText;
    [SerializeField] private TutorialManager tutorialManager;

    [Space]
    [Header("[Feed Text]")]
    [SerializeField] private Text countText;      //먹이 개수 텍스트

    private void Awake()
    {
        if (feedTimer == null) feedTimer = GetComponent<FeedTimer>();
        if (feedPanel == null) feedPanel = GetComponent<FeedPanel>();
        if (tutorialManager == null) tutorialManager = FindObjectOfType<TutorialManager>();

        if (topBarText == null)
        {
            GameObject topBarObject = GameObject.FindGameObjectWithTag(Constants.Tag_TopBar);
            if (topBarObject != null) topBarText = topBarObject.GetComponent<TopBarText>();
        }
    }

    void Start()
    {
        playerDataManager = GameManager.instance.playerDataManager;    //플레이어의 상단바 데이터 정보를 가져옴
        playerDataManager.OnCurrencyChanged += RefreshAvailableCount;

        RefreshAvailableCount();
        decreaseTime = 300;   //특제먹이 감소 시간
        selectCount = 0;
        UpdateCountText(selectCount);
    }

    private void OnDestroy()
    {
        if (playerDataManager != null)
        {
            playerDataManager.OnCurrencyChanged -= RefreshAvailableCount;
        }
    }

    public void SetCurrentRackIndex(int rackIndex)
    {
        //현재 특제 먹이를 사용할 횃대 번호를 설정하는 함수

        currentRackIndex = rackIndex;
        selectCount = 0;
        UpdateCountText(selectCount);
    }

    public void LeftButton()
    {
        //특제 먹이 선택 패널에서 선택 개수 감소 함수

        if (selectCount == 0)   //0개에서 감소 버튼을 누르면
        {
            //바로 최대로 사용 가능한 개수로 변경
            float leftTime = feedTimer.GetLeftTime(currentRackIndex);    //먹이 남은 시간
            int maxCount = Mathf.CeilToInt(leftTime / decreaseTime); //남은 시간을 즉시 끝내는 데 필요한 최대 개수
            selectCount = Mathf.Min(feedCount, maxCount); //보유량보다 많이 선택되지 않도록 제한
        }
        else
        {
            selectCount--;
        }

        UpdateCountText(selectCount);
    }

    public void RightButton()
    {
        //특제 먹이 선택 패널에서 선택 개수 증가 함수

        if (selectCount < feedCount)
        {
            selectCount++;
            float leftTime = feedTimer.GetLeftTime(currentRackIndex);    //먹이 남은 시간
            int maxCount = Mathf.CeilToInt(leftTime / decreaseTime); //남은 시간 내 최대 사용 가능 개수
            selectCount = Mathf.Min(selectCount, maxCount); //필요량을 넘지 않도록 조정

            UpdateCountText(selectCount);
        }
    }

    public void selectSpecialFeed()
    {
        //특제먹이 사용 함수

        if (selectCount <= 0) return;
        if (playerDataManager == null || feedTimer == null) return;

        int useCount = selectCount;
        if (playerDataManager.GetSpecialFeed() >= useCount)
        {
            float decrease = useCount * decreaseTime;     // 특제 먹이 사용으로 감소하는 시간 계산
            TutorialPipeline tutorialPipeline = null;
            bool isTutorialUse = tutorialManager != null
                && tutorialManager.TryGetActivePipeline(out tutorialPipeline);

            if (isTutorialUse && !playerDataManager.TryReserveSpecialFeed(useCount)) return;

            if (!feedTimer.DecreaseFeedingTime(currentRackIndex, decrease))
            {
                if (isTutorialUse) playerDataManager.CancelReservedSpecialFeed(useCount);
                return;
            }

            if (isTutorialUse)
            {
                tutorialPipeline.RegisterCompletionAction(
                    () => playerDataManager.CommitReservedSpecialFeed(useCount, false),
                    () => playerDataManager.CancelReservedSpecialFeed(useCount));
            }
            else if (!playerDataManager.UseSpecialFeed(useCount))
            {
                return;
            }

            selectCount = 0;
            RefreshAvailableCount();

            // UI 업데이트 및 저장
            UpdateCountText(selectCount);
            if (topBarText != null) topBarText.UpdateText();

            if (feedPanel != null) feedPanel.SetSpecialFeedPanelActive(false);  // 패널 닫기
        }
    }

    private void RefreshAvailableCount()
    {
        if (playerDataManager == null) return;
        feedCount = playerDataManager.GetSpecialFeed();
        selectCount = Mathf.Min(selectCount, feedCount);
        UpdateCountText(selectCount);
    }
    private void UpdateCountText(int count)
    {
        // 먹이 개수 텍스트 업데이트

        if (countText != null) countText.text = count + " 개";
    }
}

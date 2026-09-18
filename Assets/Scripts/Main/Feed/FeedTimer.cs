using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedTimer : MonoBehaviour
{
    // 먹이 타이머 클래스

    [Header("[Timer Objects]")]
    public GameObject[] timers;                     // 타이머 오브젝트 배열

    private List<RackData> rackData;                // 저장된 횃대 데이터
    private float[] leftTimes;                      // 먹이 남은 시간 배열 (단위: 초)
    private int rackLevel;                          // 플레이어 횃대 레벨
    private GameObject[] activeTimerObjects;        // 현재 횃대 레벨에서 사용하는 타이머 오브젝트 캐시
    private Text[] timerTexts;                      // 매 프레임 GetComponent를 호출하지 않기 위한 텍스트 캐시
    private int[] displayedSeconds;                 // 화면에 마지막으로 표시한 초
    private FeedManager feedManager;

    private void Start()
    {
        rackLevel = GameManager.instance.goodsDataManager.GetValidatedGoodsData(Constants.GoodsData_Rack).level;   // 플레이어의 횃대 레벨
        rackData = GameManager.instance.rackDataList;
        feedManager = GetComponent<FeedManager>();

        if (timers == null || rackLevel < 0 || rackLevel >= timers.Length || timers[rackLevel] == null)
        {
            Debug.LogError($"[FeedTimer] 횃대 레벨 {rackLevel}의 타이머 오브젝트가 설정되지 않았습니다.");
            enabled = false;
            return;
        }

        CacheTimerComponents();
        if (rackData != null && rackData.Count > 0)
            UpdateTimerSetting();   // 타이머 상태 업데이트
    }

    /// <summary>
    /// 현재 횃대 레벨의 타이머 오브젝트와 Text를 한 번만 찾아 저장합니다.
    /// </summary>
    private void CacheTimerComponents()
    {
        Transform timerParent = timers[rackLevel].transform;
        int timerCount = timerParent.childCount;
        activeTimerObjects = new GameObject[timerCount];
        timerTexts = new Text[timerCount];
        leftTimes = new float[timerCount];
        displayedSeconds = new int[timerCount];

        for (int i = 0; i < timerCount; i++)
        {
            activeTimerObjects[i] = timerParent.GetChild(i).gameObject;
            timerTexts[i] = activeTimerObjects[i].GetComponent<Text>();
            displayedSeconds[i] = int.MinValue;
        }
    }

    private void Update()
    {
        // 활성화 되어 있는 타이머는 실시간으로 초 계산

        if (activeTimerObjects == null) return;

        for (int i = 0; i < activeTimerObjects.Length; i++)                  // 타이머 개수만큼 반복
        {
            if (!activeTimerObjects[i].activeInHierarchy)    // 비활성화된 타이머라면 넘어가기
                continue;

            if (leftTimes[i] > 0)      // 타이머 시간이 남아 있다면
            {
                leftTimes[i] -= Time.deltaTime;
                RefreshTimerText(i);
            }
            else                       // 타이머가 끝났다면
            {
                ExpiredTimer(i);
            }
        }
    }

    private void ExpiredTimer(int index)
    {
        // 타이머를 만료시키는 함수

        leftTimes[index] = 0f;                         // 남은 시간 초기화
        activeTimerObjects[index].SetActive(false);      // 타이머 비활성화


        FeedType feed = rackData[index].feed;           // 놓인 먹이를 가져옴
        int birdNumber = rackData[index].birdNumber;    // 나타날 새의 번호를 가져옴

        if (feedManager != null)
        {
            feedManager.SetInactiveRackFeed(index, feed);   // 횃대에 놓인 먹이 비활성화
            feedManager.ArriveRackBird(index, birdNumber);  // 새 오브젝트 활성화
        }
        //GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_BirdArrived(); //새 도착 효과음

        SaveTimerData(index, false, true);   // 변경 내용 저장
    }

    public void UpdateTimerSetting()
    {
        // 타이머 상태 업데이트 함수 (타이머 활성화 / 비활성화)
        // 먹이가 놓여있고, 새가 나타남 -> 해당 경우 없음 (새가 등장하면 먹이가 없어져야 함)
        // 먹이가 놓여있고, 새가 나타나지 않음 -> 먹이 시간 남음 (== 타이머 활성화)
        // 먹이가 놓여 있지않고, 새가 나타남 -> 먹이 시간 만료 (== 타이머 비활성화)
        // 먹이가 놓여 있지않고, 새가 나타나지 않음 -> 먹이를 두지 않음 (== 타이머 비활성화)

        timers[rackLevel].gameObject.SetActive(true);               // 플레이어의 레벨에 맞는 횃대 타이머 활성화

        if (activeTimerObjects == null) return;

        for (int i = 0; i < activeTimerObjects.Length; i++)
        {
            if (i < rackData.Count && rackData[i].isFed && !rackData[i].isAppeared)       // 먹이를 두었고, 새가 나타나지 않았다면
            {
                leftTimes[i] = CalculateLeftTime(i);                                   // 타이머 남은 시간 계산
                activeTimerObjects[i].SetActive(true);    // 타이머 활성화
                RefreshTimerText(i, true);
            }
            else
            {
                activeTimerObjects[i].SetActive(false);    // 타이머 비활성화
                displayedSeconds[i] = int.MinValue;
            }
        }
    }

    public void ClearTimerObjects()
    {
        rackData = GameManager.instance.rackDataList;

        foreach (GameObject timerObject in timers)
        {
            for (int i = 0; i < timerObject.transform.childCount; i++)
            {
                timerObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public float GetLeftTime(int rackNumber)
    {
        // 먹이 남은 시간을 반환하는 함수

        if (leftTimes == null || rackNumber < 0 || rackNumber >= leftTimes.Length)
        {
            return 0f;
        }

        return leftTimes[rackNumber];
    }

    private float CalculateLeftTime(int rackNumber)
    {
        // 현재 먹이 남은 시간을 계산하는 함수

        DateTime startTime = DateTime.Parse(rackData[rackNumber].startTime); 
        TimeSpan timeDif = DateTime.Now - startTime;                // 시작 시간과 현재 시간의 차 계산 (== 먹이를 놓고 얼마나 지났는지 계산)
        float leftTime = rackData[rackNumber].feedingTime - (float)timeDif.TotalSeconds;             // 먹이 남은 시간 계산
        leftTime -= rackData[rackNumber].decreaseTime;              // 먹이 감소 시간 차감

        return leftTime;
    }

    private void RefreshTimerText(int index, bool force = false)
    {
        if (timerTexts == null || index < 0 || index >= timerTexts.Length || timerTexts[index] == null) return;

        int currentSeconds = Mathf.Max(0, (int)leftTimes[index]);
        if (!force && displayedSeconds[index] == currentSeconds) return;

        displayedSeconds[index] = currentSeconds;
        timerTexts[index].text = FormatTime(currentSeconds);
    }

    private string FormatTime(int amount)
    {
        // 입력한 숫자를 실제 시간으로 변환하는 함수 (시, 분, 초 양식)

        int hour = amount / 3600;           // 시
        int minute = (amount % 3600) / 60;  // 분
        int second = amount % 60;           // 초
        
        return $"{hour:00}:{minute:00}:{second:00}";
    }

    public bool DecreaseFeedingTime(int rackNumber, float decreaseTime)
    {
        // 먹이 시간을 줄이는 함수 (특제 먹이 사용 등)

        if (rackData == null || leftTimes == null || rackNumber < 0
            || rackNumber >= rackData.Count || rackNumber >= leftTimes.Length
            || !rackData[rackNumber].isFed || rackData[rackNumber].isAppeared
            || decreaseTime <= 0f)
        {
            Debug.LogWarning($"[FeedTimer] 특제 먹이를 적용할 수 없는 횃대입니다. rack: {rackNumber}");
            return false;
        }

        leftTimes[rackNumber] = Mathf.Max(0f, leftTimes[rackNumber] - decreaseTime); // 화면상 남은 시간은 음수가 되지 않게 유지
        RefreshTimerText(rackNumber, true);
        float curDecreaseTime = rackData[rackNumber].decreaseTime + decreaseTime;   // 누적 감소 시간 계산
        SaveTimerData(rackNumber, curDecreaseTime);         // 변동된 타이머 데이터 저장
        return true;
    }

    public void DecreaseAllActiveFeedingTimes(float decreaseTime)
    {
        // 진행 중인 모든 타이머에 먹이 시간을 줄이는 함수

        if (activeTimerObjects == null) return;

        for (int i = 0; i < activeTimerObjects.Length; i++)
        {
            if (i < rackData.Count && rackData[i].isFed && !rackData[i].isAppeared)
            {
                DecreaseFeedingTime(i, decreaseTime);
            }
        }
    }

    public void SaveTimerData(int rackNumber, float feedingTime, float decreaseTime, FeedType feed, int birdNumber)
    {
        // 타이머 데이터 저장 오버로드 함수 (초기화)

        SaveTimerData(rackNumber, data =>
        {
            data.isFed = true;
            data.startTime = DateTime.Now.ToString();
            data.feedingTime = feedingTime;
            data.decreaseTime = decreaseTime;
            data.isAppeared = false;
            data.feed = feed;
            data.birdNumber = birdNumber;
        });
    }

    public void SaveTimerData(int rackNumber, bool isFed, bool isAppear)
    {
        // 타이머 데이터 저장 오버로드 함수 (새 등장)

        SaveTimerData(rackNumber, data =>
        {
            data.isFed = isFed;
            data.isAppeared = isAppear;
        });
    }

    public void SaveTimerData(int rackNumber, float decreaseTime)
    {
        // 타이머 데이터 저장 오버로드 함수 (먹이 시간 변경)

        SaveTimerData(rackNumber, data => data.decreaseTime = decreaseTime);
    }

    private void SaveTimerData(int rackNumber, Action<RackData> updateAction)
    {
        // 타이머 데이터를 저장하는 함수

        List<RackData> savedData = GameManager.instance.rackDataList;

        while (rackNumber >= savedData.Count)
        {
            savedData.Add(new RackData());
        }
        updateAction(savedData[rackNumber]);    // rackNumber에 해당하는 데이터를 updateAction을 사용해 업데이트

        GameManager.instance.rackDataList = savedData;
        GameManager.instance.RackDataManager.SetData(savedData);

        PlayerDataManager playerDataManager = GameManager.instance.playerDataManager;
        bool isTutorialInProgress = !playerDataManager.GetIsQuestActinoPlaying()
            && TutorialManager.IsTutorialScene(playerDataManager.GetCurrentScene());
        if (!isTutorialInProgress)
        {
            GameManager.instance.RackDataManager.Save();
        }
    }
}

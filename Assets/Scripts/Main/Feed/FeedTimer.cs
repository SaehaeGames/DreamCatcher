using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedTimer : MonoBehaviour
{
    // 먹이 타이머 클래스

    [Header("[Timer Objects]")]
    public GameObject[] timers;                     // 타이머 오브젝트 배열

    [Header("[Timer Data")]
    private static string fileName = "RackData";    // 타이머 데이터 저장 파일 이름
    private List<RackData> rackData;                // 저장된 횃대 데이터
    private float[] leftTimes;                      // 먹이 남은 시간 배열 (단위: 초)
    private int rackLevel;                          // 플레이어 횃대 레벨

    private void Start()
    {
        rackData = GameManager.instance.rackDataList;  
        leftTimes = new float[timers[rackLevel].transform.childCount];
        rackLevel = 0;
        //rackLevel = GameManager.instance.goodsDataList[0].goodsLevel;   // 플레이어의 횃대 레벨
        //UpdateTimerSetting();   // 타이머 상태 업데이트
    }

    private void Update()
    {
        // 활성화 되어 있는 타이머는 실시간으로 초 계산

        for (int i = 0; i < timers[rackLevel].transform.childCount; i++)                  // 타이머 개수만큼 반복
        {
            if (!timers[rackLevel].transform.GetChild(i).gameObject.activeInHierarchy)    // 비활성화된 타이머라면 넘어가기
                continue;

            if (leftTimes[i] > 0)      // 타이머 시간이 남아 있다면
            {
                leftTimes[i] -= Time.deltaTime;             
                timers[rackLevel].transform.GetChild(i).GetComponent<Text>().text = FormatTime(leftTimes[i]);    // 남은 시간 출력
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
        timers[rackLevel].transform.GetChild(index).gameObject.SetActive(false);      // 타이머 비활성화


        FeedType feed = rackData[index].feed;           // 놓인 먹이를 가져옴
        int birdNumber = rackData[index].birdNumber;    // 나타날 새의 번호를 가져옴

        FeedManager feedManager = GetComponent<FeedManager>();
        feedManager.SetInactiveRackFeed(index, feed);   // 횃대에 놓인 먹이 비활성화
        feedManager.ArriveRackBird(index, birdNumber);  // 새 오브젝트 활성화
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

        for (int i = 0; i < timers[rackLevel].transform.childCount; i++)     
        {
            if (rackData[i].isFed && !rackData[i].isAppeared)       // 먹이를 두었고, 새가 나타나지 않았다면
            {
                leftTimes[i] = CalculateLeftTime(i);                                   // 타이머 남은 시간 계산
                timers[rackLevel].transform.GetChild(i).gameObject.SetActive(true);    // 타이머 활성화
            }
            else
            {
                timers[rackLevel].transform.GetChild(i).gameObject.SetActive(false);    // 타이머 비활성화
            }
        }
    }

    public float GetLeftTime(int rackNumber)
    {
        // 먹이 남은 시간을 반환하는 함수

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

    private string FormatTime(float amount)
    {
        // 입력한 숫자를 실제 시간으로 변환하는 함수 (시, 분, 초 양식)

        int hour = (int)amount / 3600;           // 시
        int minute = ((int)amount % 3600) / 60;  // 분  
        int second = (int)amount % 60;           // 초
        
        return $"{hour:00}:{minute:00}:{second:00}";
    }

    public void DecreaseFeedingTime(int rackNumber, float decreaseTime)
    {
        // 먹이 시간을 줄이는 함수 (특제 먹이 사용 등)

        leftTimes[rackNumber] -= decreaseTime;                                 // 남은 시간에 감소 시간 차감
        float curDecreaseTime = rackData[rackNumber].decreaseTime + decreaseTime;   // 누적 감소 시간 계산
        SaveTimerData(rackNumber, curDecreaseTime);         // 변동된 타이머 데이터 저장
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
        GameManager.instance.jsonManager.SaveDataList<RackData>(fileName, savedData);
    }
}

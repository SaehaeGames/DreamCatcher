using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedTimer : MonoBehaviour
{
    // 먹이 타이머 클래스
    // 먹이 타이머를 on/off 하고 업데이트함

    [Header("[Timer Objects]")]
    public GameObject[] timerText;      // 타이머 오브젝트 배열

    [Space]
    [Header("[Feed Timer]")]
    [SerializeField] private float[] leftTime;   // 먹이 남은 시간 배열 (단위: 초)
    BirdContainer saveBirds;    // 플레이어 새 저장 데이터
    int rackLevel; // 플레이어 횃대 레벨

    private void Start()
    {
        saveBirds = GameManager.instance.loadBirdData;    // 저장된 새 데이터를 가져옴
        rackLevel = GameManager.instance.loadGoodsData.goodsList[0].goodsLevel; // 플레이어의 횃대 레벨
        leftTime = new float[rackLevel + 1];    // 횃대 레벨만큼 타이머 저장 공간 설정
        UpdateTimerSetting();   // 타이머 상태 업데이트
    }

    private void Update()
    {
        // 활성화 되어 있는 타이머는
        // 실시간으로 초 계산

        for (int i = 0; i < timerText[rackLevel].transform.childCount; i++)   // 타이머 개수만큼 반복
        {
            if (!timerText[rackLevel].transform.GetChild(i).gameObject.activeInHierarchy)    // 비활성화된 타이머라면
            {
                continue;   // 넘어가기
            }
            else    // 활성화된 타이머라면
            {
                if (leftTime[i] > 1)    // 남은 시간이 0이 아니라면 (== 먹이 시간이 남아있다면)
                {
                    leftTime[i] -= Time.deltaTime; // 실시간으로 남은 시간 감소
                    timerText[rackLevel].transform.GetChild(i).GetComponent<Text>().text = CalculateRealTime(leftTime[i]);    // 남은 시간 보여줌
                }
                else    // 남은 시간이 0초가 되었다면 (=  먹이 시간이 모두 지났다면)
                {
                    leftTime[i] = 0f;   // 해당 타이머 남은 시간 0으로 초기화
                    timerText[rackLevel].transform.GetChild(i).gameObject.SetActive(false); // 타이머 비활성화

                    int feedNumber = saveBirds.birdList[i].feedNumber; // 놓인 먹이의 번호를 가져옴
                    this.gameObject.GetComponent<FeedManager>().SetInactiveRackFeed(i, feedNumber);    // 해당 횃대에 놓인 먹이 비활성화

                    int birdNumber = saveBirds.birdList[i].birdNumber;  // 나타날 새의 고유 번호 가져옴
                    this.gameObject.GetComponent<FeedManager>().ArriveRackBird(i, birdNumber);   // 새 도착 오브젝트 활성화
                    GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_BirdArrived(); //새 도착 효과음

                    // 검토해야 할 것
                    SaveTimerData(i, false, true);   //초기화 내용 저장
                }
            }
        }
    }

    public void UpdateTimerSetting()
    {
        // 타이머 오브젝트 활성화/비활성화 상태 업데이트 함수
        // 먹이 시간이 남아있는 타이머를 활성화함

        timerText[rackLevel].gameObject.SetActive(true);    // 플레이어의 레벨에 맞는 횃대 타이머 활성화

        BirdContainer birdData = GameManager.instance.loadBirdData;

        for (int i = 0; i < timerText[rackLevel].transform.childCount; i++)     // 해당 레벨의 타이머 개수만큼 반복
        {
            if (birdData.birdList[i].isFed && !birdData.birdList[i].isAppeared)   // 먹이를 두었고, 새가 나타나지 않았다면 ( == 먹이 남은 시간이 있다면)
            {
                leftTime[i] = CalculateLeftTime(i);    // 타이머 남은 시간 계산하여 저장
                timerText[rackLevel].transform.GetChild(i).gameObject.SetActive(true);    // 타이머 활성화
            }
            else
            {
                timerText[rackLevel].transform.GetChild(i).gameObject.SetActive(false);    // 타이머 비활성화
            }
            // 먹이가 놓여있고, 새가 나타남 -> 해당 경우 없음 (새가 등장하면 먹이가 없어져야 함)
            // 먹이가 놓여있고, 새가 나타나지 않음 -> 먹이 시간 남음 (== 타이머 활성화)
            // 먹이가 놓여 있지않고, 새가 나타남 -> 먹이 시간 만료 (== 타이머 비활성화)
            // 먹이가 놓여 있지않고, 새가 나타나지 않음 -> 먹이를 두지 않음
        }
    }

    public float GetLeftTime(int rackNumber)
    {
        // 인게임 중 먹이 남은 시간을 반환하는 함수

        return this.leftTime[rackNumber];
    }

    public float CalculateLeftTime(int rackNumber)
    {
        // 먹이 저장 데이터로 현재 남은 시간 반환하는 함수

        BirdContainer saveBirds = GameManager.instance.loadBirdData;    // 저장된 새 데이터 가져옴

        DateTime currentTime = DateTime.Now;    //현재 시간 설정
        string savedStartTime = saveBirds.birdList[rackNumber].startTime; // 먹이 시작 시간 가져옴
        DateTime startTime = DateTime.Parse(savedStartTime);    // DateTime 형식으로 변환

        TimeSpan timeDif = currentTime - startTime; // 시작 시간과 현재 시간의 차 구함 (== 먹이를 놓고 얼마나 지났는지)
        float difSeconds = (float)timeDif.TotalSeconds; // 지난 시간을 초로 변환

        float feedingTime = saveBirds.birdList[rackNumber].feedingTime;  // 랜덤 먹이 시간 가져옴
        float decreaseTime = saveBirds.birdList[rackNumber].decreaseTime;    // 특제 먹이 사용으로 감소된 시간 가져옴
        float left = feedingTime - difSeconds;   // 먹이 남은 시간 계산
        left -= decreaseTime;   // 특제 먹이 사용으로 감소된 시간 차감

        return left;    // 먹이 남은 시간 반환
    }

    public string CalculateRealTime(float amount)
    {
        // 입력한 숫자만큼 실제 시간으로 변환하는 함수
        // 시, 분, 초 계산

        int hour = (int)amount / 3600; // 시
        float left = amount % 3600;  // 시간을 나누고 남은 시간
        int minute = (int)left / 60; // 분  
        left = left % 60;   // 분을 나누고 남은 시간
        int second = (int)left; // 초
        Mathf.Floor(second).ToString();  // 초 소수점 버리기

        string realTime = string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);   // 텍스트 양식(시:분:초)로 출력
        return realTime;    // 실제 남은 시간 반환
    }

    public void DecreaseFeedingTime(int rackNumber, float decreaseTime)
    {
        // 먹이 시간을 줄이는 함수 (특제 먹이 사용 등)

        this.leftTime[rackNumber] -= decreaseTime; // 남은 시간에 감소할 시간 차감

        BirdContainer saveBirds = GameManager.instance.loadBirdData;
        float curDecreaseTime = saveBirds.birdList[rackNumber].decreaseTime + decreaseTime; // 누적 감소 시간 증가

        SaveTimerData(rackNumber, curDecreaseTime);    // 변동된 타이머 데이터 저장
    }

    public void SaveTimerData(int rackNumber, float feedingTime, float decreaseTime, int feedNumber, int birdNumber)
    {
        // 타이머 데이터 저장 오버로드 함수 (먹이를 처음 두었을 때)

        SaveTimerJSON(rackNumber, birdData =>
        {
            birdData.isFed = true;
            birdData.startTime = DateTime.Now.ToString();
            birdData.feedingTime = feedingTime;
            birdData.decreaseTime = decreaseTime;
            birdData.isAppeared = false;
            birdData.feedNumber = feedNumber;
            birdData.birdNumber = birdNumber;
        });

        Debug.Log("먹이 번호 저장 : " + feedNumber);
    }

    public void SaveTimerData(int rackNumber, bool isFed, bool isAppear)
    {
        // 타이머 데이터 저장 오버로드 함수 (먹이 시간이 지나고 새가 나타났을 때)

        SaveTimerJSON(rackNumber, birdData =>
        {
            birdData.isFed = isFed;
            birdData.isAppeared = isAppear;
        });
    }

    public void SaveTimerData(int rackNumber, float decreaseTime)
    {
        // 타이머 데이터 저장 오버로드 함수 (먹이 시간이 감소했을 때)

        SaveTimerJSON(rackNumber, birdData =>
        {
            birdData.decreaseTime = decreaseTime;
        });
    }

    private void SaveTimerJSON(int rackNumber, Action<BirdData> updateAction)
    {
        // 타이머 데이터를 저장하는 함수

        BirdContainer saveBirds = GameManager.instance.loadBirdData;

        updateAction(saveBirds.birdList[rackNumber]);

        GameManager.instance.loadBirdData = saveBirds;
        GameManager.instance.GetComponent<BirdJSON>().DataSaveText(saveBirds);
    }
}

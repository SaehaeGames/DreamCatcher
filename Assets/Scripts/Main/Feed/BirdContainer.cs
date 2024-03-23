using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BirdData
{
    public BirdData(bool _isFed, string _startTime, float _feedingTime, float _decreaseTime, bool _isAppeared, int _feedNumber, int _birdNumber)
    {
        isFed = _isFed;
        startTime = _startTime;
        feedingTime =_feedingTime;
        decreaseTime = _decreaseTime;
        isAppeared = _isAppeared;
        feedNumber = _feedNumber;
        birdNumber = _birdNumber;
    }

    public bool isFed;  //놓인 먹이가 있는지 여부
    public string startTime; //먹이 놓은 시간
    public float feedingTime; //먹이 시간(랜덤 적용 시간)
    public float decreaseTime;  //특제 먹이 사용으로 감소된 시간
    public bool isAppeared; //새가 나타났는지 여부
    public int feedNumber;  //놓은 먹이 번호
    public int birdNumber;  //새의 번호
    
}


public class BirdContainer
{
    public BirdContainer()
    {
        ResetBirdData();

    }

    public int birdListCnt; //횃대에 놓을 수 있는 총 새의 수
    public BirdData[] birdList; //플레이어의 횃대의 새 리스트

    public void ResetBirdData()
    {
        birdListCnt = 3;
        birdList = new BirdData[birdListCnt];

        for (int i = 0; i < birdListCnt; i++)
        {
            birdList[i] = new BirdData(false, "0", 0f, 0f, false, 0, 0);
        }
    }
}

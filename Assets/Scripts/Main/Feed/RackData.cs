using System;

[Serializable]
public class RackData
{
    public bool isFed;              // 횃대에 놓인 먹이가 있는지 여부
    public string startTime;        // 횃대에 먹이를 놓은 시작 시간
    public float feedingTime;       // 먹이가 사라지기까지 걸리는 시간
    public float decreaseTime;      // 특제 먹이 사용으로 감소된 시간
    public bool isAppeared;         // 횃대에 새가 나타났는지 여부
    public FeedType feed;           // 횃대에 놓은 먹이 종류
    public int birdNumber;          // 나타날 새의 번호

    public RackData()
    {
        isFed = isAppeared = false;
        startTime = string.Empty;
        feedingTime = decreaseTime = 0f;
        feed = default;
        birdNumber = 0;
    }

    public RackData(bool isFed, string startTime, float feedingTime, float decreaseTime, bool isAppeared, FeedType feed, int birdNumber)
    {
        this.isFed = isFed;
        this.startTime = startTime;
        this.feedingTime = feedingTime;
        this.decreaseTime = decreaseTime;
        this.isAppeared = isAppeared;
        this.feed = feed;
        this.birdNumber = birdNumber;
    }   
}

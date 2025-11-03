using System;

/*[Serializable]
public class MyFeatherNumber
{
    public int bird_id;
    public int feather_number;

    public MyFeatherNumber(int _birdID, int _featherNumber)
    {
        bird_id = _birdID;
        feather_number = _featherNumber;
    }

    public int GetBirdID()
    {
        return bird_id;
    }

    public int GetFeatherNumber()
    {
        return feather_number;
    }

    public void SetFeatherNumber(int num)
    {
        feather_number = num;
    }
}*/

[Serializable]
public class FeatherData
{
    public string bird_id;
    public int feather_number;
    public int appear;

    public FeatherData(string _birdID, int _featherNumber, int _appear)
    {
        bird_id = _birdID;
        feather_number = _featherNumber;
        appear = _appear;
    }
}

public class MyFeatherNumber
{
    public MyFeatherNumber()
    {
        ResetBirdData();
    }
    public int birdCnt;
    public FeatherData[] featherList;

    public void ResetBirdData()
    {
        birdCnt = 16;
        featherList = new FeatherData[birdCnt];

        for (int i = 0; i < birdCnt; i++)
        {
            featherList[i] = new FeatherData("JS_"+(1000 + i + 1), 0, 0);
        }
    }

}
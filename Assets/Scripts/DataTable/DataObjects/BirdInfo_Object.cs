using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BirdInfo_Object
{
    public int id;
    public string name;
    public string exp;
    public FeedType feed;
    public int price;
    public int startTime;
    public int endTime;
    public int maxNum;
    public int probability;

    public BirdInfo_Object()
    {
        id = price = startTime = endTime = maxNum = probability = 0;
        feed = FeedType.PigeonBeans;
        name = exp = "";
    }

    public BirdInfo_Object(int _id, string _name, string _exp, FeedType _feed, int _price, 
        int _startTime, int _endTime, int _maxNum, int _probability)
    {
        id = _id;
        name = _name;
        exp = _exp;
        feed = _feed;
        price = _price;
        startTime = _startTime;
        endTime = _endTime;
        maxNum = _maxNum;
        probability = _probability;
    }
}

public enum FeedType
{
    PigeonBeans,
    Berry,
    Earthworm,
    LeanMeat
}

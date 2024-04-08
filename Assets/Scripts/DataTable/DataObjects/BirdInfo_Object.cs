using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BirdInfo_Object
{
    public int id;
    public string name;
    public string exp;
    public FoodType food;
    public int price;
    public int startTime;
    public int endTime;
    public int maxNum;
    public int probability;

    public BirdInfo_Object()
    {
        id = price = startTime = endTime = maxNum = probability = 0;
        name = exp = "";
    }

    public BirdInfo_Object(int _id, string _name, string _exp, FoodType foodType, int _price, 
        int _startTime, int _endTime, int _maxNum, int _probability)
    {
        id = _id;
        name = _name;
        exp = _exp;
        food = foodType;
        price = _price;
        startTime = _startTime;
        endTime = _endTime;
        maxNum = _maxNum;
        probability = _probability;
    }

    public enum FoodType
    {
        PigeonBeans,
        Berry,
        Earthworm,
        LeanMeat
    }
}

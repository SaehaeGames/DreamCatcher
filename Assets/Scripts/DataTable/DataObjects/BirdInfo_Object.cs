using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BirdInfo_Object
{
    public int id;
    public string name;
    public string exp;
    public string food;
    public int price;
    public int starttime;
    public int endtime;
    public int maxnum;
    public int probability;
    public int appear;
    public int number;

    public BirdInfo_Object()
    {
        id = price = starttime = endtime = maxnum = probability = appear = number = 0;
        name = exp = food = "";
    }

    public BirdInfo_Object(int _id, string _name, string _exp, string _food, int _price, 
        int _starttime, int _endtime, int _maxnum, int _probability, int _appear, int _number)
    {
        id = _id;
        name = _name;
        exp = _exp;
        food = _food;
        price = _price;
        starttime = _starttime;
        endtime = _endtime;
        maxnum = _maxnum;
        probability = _probability;
        appear = _appear;
        number = _number;
    }
}

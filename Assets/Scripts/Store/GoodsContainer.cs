using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GoodsData
{
    public GoodsData(string _name, int _level)
    {
        goodsName = _name;
        goodsLevel = _level;
    }

    public string goodsName;       //상품 이름
    public int goodsLevel;         //상품 레벨
}

public class GoodsContainer
{
    public GoodsContainer()
    {
        ResetGoodsData();
    }

    public int goodsCount;  //상품(보조도구) 종류 개수
    public GoodsData[] goodsList;  //상품(보조도구 정보 리스트)

    public void ResetGoodsData()
    {
        goodsCount = 4; //상품의 개수 *소프트코딩으로 수정이 가능할지 고민..

        goodsList = new GoodsData[goodsCount];

        goodsList[0] = new GoodsData("Rack", 0);
        goodsList[1] = new GoodsData("Vase", 0);
        goodsList[2] = new GoodsData("Box", 0);
        goodsList[3] = new GoodsData("Thread", 0);
    }
}

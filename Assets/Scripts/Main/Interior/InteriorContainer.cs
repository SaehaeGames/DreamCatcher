using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InteriorData
{
    public InteriorData(int _itemId, int _itemPossessing, int _itemAdjusting)
    {
        itemId = _itemId;
        itemPossessing = _itemPossessing;
        itemAdjusting = _itemAdjusting;
    }

    public int itemId;  //인테리어 아이템 id
    public int itemPossessing;   //인테리어 아이템 보유중 여부
    public int itemAdjusting;   //인테리어 아이템 적용중 여부
}

public class InteriorContainer
{
    public InteriorContainer()
    {
        ResetInteriorData();
    }

    public int itemCount;   //인테리어 아이템 개수
    public InteriorData[] itemList; //인테리어 아이템 리스트

    public void ResetInteriorData()
    {
        itemCount = 40;
        itemList = new InteriorData[itemCount];

        int countNum = 0;
        for (int i = 0; i < itemCount; i++)
        {
            if (i < 13)
            {
                itemList[i] = new InteriorData(5004 + countNum++, 0, 0);

                if (countNum >= 13)
                {
                    countNum = 0;
                }
            }
            else
            {
                itemList[i] = new InteriorData(4001 + countNum++, 0, 0);
            }
        }

        //기본값 세팅
        itemList[0].itemAdjusting = 1;
        itemList[4].itemAdjusting = 1;
        itemList[8].itemAdjusting = 1;
    }
}

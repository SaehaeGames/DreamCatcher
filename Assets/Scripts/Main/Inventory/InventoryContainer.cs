using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryItemData
{
    public InventoryItemData(string _id, string _name, string _contents, int _appearNumber, int _appear, string _appearTime)
    {
        id = _id;
        name = _name;
        contents = _contents;
        appearNumber = _appearNumber;
        appear = _appear;
        appearTime = _appearTime;
    }

    public string id;   //인벤토리 아이템 아이디
    public string name;     //인벤토리 아이템 이름
    public string contents; //인벤토리 아이템 설명
    public int appearNumber;   //인벤토리 아이템 개수
    public int appear; //인벤토리 아이템 등장 여부
    public string appearTime;   //인벤토리 아이템 등장 시간
}

public class InventoryContainer
{
    public InventoryContainer()
    {
        ResetInventoryData();
    }

    public int inventoryItemCnt;    //인벤토리 아이템 개수
    public int featherCnt;  //깃털 개수
    public int dreamCatcherCnt; //드림캐쳐 개수
    public InventoryItemData[] itemList;    //인벤토리 아이템 리스트

    public void ResetInventoryData()
    {
        List<Dictionary<string, object>> data_Inventory = CSVParser.ReadFromFile("Inventory");  //인벤토리 데이터를 가져옴  

        inventoryItemCnt = data_Inventory.Count;    //인벤토리 아이템 수
        featherCnt = 16;
        dreamCatcherCnt = 24;

        itemList = new InventoryItemData[inventoryItemCnt]; //아이템 리스트 생성

        for (int i = 0; i < inventoryItemCnt; i++)
        {
            string itemId = data_Inventory[i]["id"].ToString(); //아이템 아이디
            string itemName = data_Inventory[i]["name"].ToString(); //아이템 이름
            string itemContents = data_Inventory[i]["contents"].ToString(); //아이템 내용
            int itemAppearNum = int.Parse(data_Inventory[i]["number"].ToString());   //아이템 개수
            int itemAppear = int.Parse(data_Inventory[i]["appear"].ToString());   //아이템 등장여부
            string itemAppearTime = data_Inventory[i]["appeartime"].ToString();   //아이템 등장 시간

            itemList[i] = new InventoryItemData(itemId, itemName, itemContents, itemAppearNum, itemAppear, itemAppearTime); //인벤토리 아이템 추가
        }
    }

    public void Initialization()
    {
        //데이터를 아예 초기화하는 함수

        List<Dictionary<string, object>> data_Inventory = CSVParser.ReadFromFile("Inventory");  //인벤토리 데이터를 가져옴  

        inventoryItemCnt = data_Inventory.Count;    //인벤토리 아이템 수
        itemList = new InventoryItemData[inventoryItemCnt]; //아이템 리스트 생성

        for (int i = 0; i < inventoryItemCnt; i++)
        {
            string itemId = data_Inventory[i]["id"].ToString(); //아이템 아이디
            string itemName = data_Inventory[i]["name"].ToString(); //아이템 이름
            string itemContents = data_Inventory[i]["contents"].ToString(); //아이템 내용
            int itemAppearNum = 0;   //아이템 개수
            int itemAppear = 0;   //아이템 등장여부
            string itemAppearTime = "";   //아이템 등장 시간

            itemList[i] = new InventoryItemData(itemId, itemName, itemContents, itemAppearNum, itemAppear, itemAppearTime); //인벤토리 아이템 추가
        }
    }
}

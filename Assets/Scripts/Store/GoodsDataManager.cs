using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;


[Serializable]
public class GoodsData
{
    public int id;
    public string name;      //상품 이름
    public string category;
    public int level;   //상품 레벨

    public GoodsData()
    {
        name = category = "";
        id = level = 0;
    }

    public GoodsData(int _id, string _name, string _category, int _level)
    {
        id = _id;
        name = _name;
        category = _category;
        level = _level;
    }
}
public class GoodsDataManager
{
    public List<GoodsData> dataList;

    public GoodsDataManager()
    {
        dataList = new List<GoodsData>();
        ResetData();
    }

    public void ResetData()
    {
        if (dataList != null)
            dataList.Clear();

        List<InteriorInfo_Object> infoDataList = GameManager.instance.interiorinfo_data.dataList;
        for (int i = 0; i < 5; i++)
            dataList.Add(new GoodsData(infoDataList[i].id, infoDataList[i].name, infoDataList[i].category, 0));
    }

    public List<GoodsData> GetGoodsDataList(string category)
    {
        string cleanedCategory = category.Trim(); // ✅ `\r` 제거
        UnityEngine.Debug.Log($"[DEBUG] GetGoodsDataList - 찾는 Category: {cleanedCategory}");

        var itemList = dataList.Where(x => x.category.Trim().Equals(cleanedCategory, StringComparison.OrdinalIgnoreCase)).ToList();

        if (itemList.Count == 0)
        {
            UnityEngine.Debug.LogError($"[ERROR] GetGoodsDataList - {cleanedCategory}에 해당하는 데이터가 없음.");
        }

        return itemList;
    }

    public GoodsData GetGoodsDataByIndex(int goodsNumber)
    {
        var storeItem = GameManager.instance.storeinfo_data.dataList[goodsNumber];
        if (storeItem == null)
        {
            UnityEngine.Debug.LogError($"[ERROR] StoreInfo_Data에 goodsNumber {goodsNumber}에 해당하는 데이터가 없음");
            return null;
        }

        // ✅ Trim()을 추가하여 `\r` 제거 후 비교 정확도 향상
        string category = storeItem.category.ToString().Trim();
        UnityEngine.Debug.Log($"[DEBUG] GetGoodsDataByIndex - 찾는 Category: {category}");

        // ✅ `RackFront`, `RackBack`을 개별적으로 처리
        if (category == "Rack")
        {
            var racks = dataList.Where(x => x.category.Trim() == category).ToList();
            if (racks.Count == 2)
            {
                // ✅ 두 개의 Rack이 존재하면 둘 다 레벨업
                UnityEngine.Debug.Log($"[SUCCESS] RackFront & RackBack 레벨업!");
                racks[0].level++;
                racks[1].level++;
                return racks[0]; // 둘 중 하나 반환
            }
        }

        // ✅ 정확한 데이터만 가져오도록 category 비교 방식 개선
        var item = dataList.FirstOrDefault(x => x.category.Trim().Equals(category, StringComparison.OrdinalIgnoreCase));

        if (item == null)
        {
            UnityEngine.Debug.LogError($"[ERROR] GetGoodsDataByIndex - {category}에 해당하는 데이터를 찾을 수 없음.");
        }

        return item;
    }

    public GoodsData GetGoodsDataByCategory(string category)
    {
        category = category.Trim();  // ✅ 불필요한 공백 제거
        return dataList.FirstOrDefault(x => x.category.Trim().Equals(category, StringComparison.OrdinalIgnoreCase));
    }

    public GoodsData GetValidatedGoodsData(string category)
    {
        var matchingItems = dataList.Where(x => x.category.Trim().ToLower() == category.Trim().ToLower()).ToList();

        if (matchingItems.Count == 0)
        {
            return null;
        }
        else if (matchingItems.Count == 1)
        {
            return matchingItems[0]; // ✅ 하나만 있으면 그대로 반환
        }
        else // ✅ 두 개 이상이면 체크 후 반환 or 오류 출력
        {
            if (matchingItems[0].level == matchingItems[1].level)
            {
                return matchingItems[0]; // ✅ 레벨이 같으면 첫 번째 반환
            }
            else
            {
                return null;
            }
        }
    }
}
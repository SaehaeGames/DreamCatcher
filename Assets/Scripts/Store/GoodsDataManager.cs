using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;

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
        for (int i = 0; i < infoDataList.Count; i++)
            dataList.Add(new GoodsData(infoDataList[i].id, infoDataList[i].name, infoDataList[i].category, 0));
    }

    public GoodsData GetGoodsData(string dataName)
    {
        GoodsData getData = dataList.FirstOrDefault(x => x.category == dataName);
        if (getData != null)
            return getData;
        return null;
    }

    // 레벨업 하면 해당 카테고리 데이터들 레벨 한 번에 다 오르게 하는 함수도 필요함
    // 예를들어 횃대 레벨업 하면 rackfront와 rackback에 해당하는 level이 전부 1씩 오르게 하는..
}
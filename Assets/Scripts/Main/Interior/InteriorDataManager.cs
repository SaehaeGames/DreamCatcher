using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class InteriorData
{

    public int id;           //인테리어 아이템 id
    public int level;       // 인테리어 아이템 레벨
    public bool isHaving;         //인테리어 아이템 보유중 여부
    public bool isAdjusting;      //인테리어 아이템 적용중 여부

    public InteriorData(int id, int level, bool isHaving, bool isAdjusting)
    {
        this.id = id;
        this.level = level;
        this.isHaving = isHaving;
        this.isAdjusting = isAdjusting;
    }
}

public class InteriorDataManager
{
    public List<InteriorData> dataList;  

    public InteriorDataManager()
    {
        dataList = new List<InteriorData>();
        ResetData();
    }

    public void ResetData()
    {
        if (dataList != null)
            dataList.Clear();

        StoreInfo_Data storeInfo_data = GameManager.instance.storeinfo_data;

        List<int> defaultItemIDList = storeInfo_data.GetSortedIDsByTheme(ItemTheme.Default);
        List<int> SeaItemIDList = storeInfo_data.GetSortedIDsByTheme(ItemTheme.Sea);
        List<int> StarItemIDList = storeInfo_data.GetSortedIDsByTheme(ItemTheme.Star);

        List<int> combinedItemIDList = new List<int>();
        combinedItemIDList.AddRange(defaultItemIDList);
        combinedItemIDList.AddRange(SeaItemIDList);
        combinedItemIDList.AddRange(StarItemIDList);

        int cnt = 0;
        for (int j = 0; j < defaultItemIDList.Count; j++)
        {
            int level = GameManager.instance.storeinfo_data.GetLevelByID(defaultItemIDList[j]);
            dataList.Add(new InteriorData(combinedItemIDList[cnt++], level, false, false));
        }
        for (int j = 0; j < SeaItemIDList.Count; j++)
        {
            dataList.Add(new InteriorData(combinedItemIDList[cnt++], 1, false, false));
        }
        for (int j = 0; j < StarItemIDList.Count; j++)
        {
            dataList.Add(new InteriorData(combinedItemIDList[cnt++], 2, false, false));
        }


        // 기초 꽃병, 주머니, 꽃병 아이템 1단계만 활성화
        int[] indicesToSet = { 0, 4, 8 };       // 설정할 인덱스 목록
        foreach (int index in indicesToSet)     // 각 인덱스에 대해 isHaving과 isAdjusting 값을 설정
        {
            dataList[index].isHaving = true;
            dataList[index].isAdjusting = true;
        }

        /*        List<InteriorInfo_Object> infoDataList = GameManager.instance.interiorinfo_data.dataList;       // 인테리어 도감 데이터
                for (int i = 0; i < infoDataList.Count; i++)
                    dataList.Add(new InteriorData(infoDataList[i].id, false, false));*/
    }

    public InteriorData GetInteriorData(int id)
    {
        InteriorData getData = dataList.FirstOrDefault(x => x.id == id);
        if (getData != null)
            return getData;
        else
            return null;
    }
}

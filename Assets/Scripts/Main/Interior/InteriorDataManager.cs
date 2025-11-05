using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Security.Cryptography;

[Serializable]
public class InteriorData
{

    public string id;           //인테리어 아이템 id
    public bool isHaving;         //인테리어 아이템 보유중 여부
    public bool isAdjusting;      //인테리어 아이템 적용중 여부
    public string storeinfo_id; //storeinfo_data의 아이디를 참조

    public InteriorData(string _id, bool _isHaving, bool _isAdjusting, string _storeinfo_id)
    {
        this.id = _id;
        this.isHaving = _isHaving;
        this.isAdjusting = _isAdjusting;
        this.storeinfo_id = _storeinfo_id;
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

        List<string> defaultItemIDList = storeInfo_data.GetSortedIDsByTheme(ItemTheme.Default);
        List<string> SeaItemIDList = storeInfo_data.GetSortedIDsByTheme(ItemTheme.Sea);
        List<string> StarItemIDList = storeInfo_data.GetSortedIDsByTheme(ItemTheme.Star);

        List<string> combinedItemIDList = new List<string>();
        combinedItemIDList.AddRange(defaultItemIDList);
        combinedItemIDList.AddRange(SeaItemIDList);
        combinedItemIDList.AddRange(StarItemIDList);

        int startId = 4000;
        int cnt = 0;
        for (int j = 0; j < defaultItemIDList.Count; j++)
        {
            int level = GameManager.instance.storeinfo_data.GetLevelByID(defaultItemIDList[j]);
            dataList.Add(new InteriorData("JS_" + startId, false, false, combinedItemIDList[cnt++]));
            startId++;
        }
        for (int j = 0; j < SeaItemIDList.Count; j++)
        {
            dataList.Add(new InteriorData("JS_" + startId, false, false, combinedItemIDList[cnt++]));
            startId++;
        }
        for (int j = 0; j < StarItemIDList.Count; j++)
        {
            dataList.Add(new InteriorData("JS_" + startId, false, false, combinedItemIDList[cnt++]));
            startId++;
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

    public InteriorData GetInteriorDataByStoreInfoId(string _storeinfo_id)
    {
        InteriorData getData = dataList.FirstOrDefault(x => x.storeinfo_id == _storeinfo_id);
        if (getData != null)
            return getData;
        else
            return null;
    }

    public InteriorData GetInteriorDataById(string _id)
    {
        InteriorData getData = dataList.FirstOrDefault(x => x.id == _id);
        if (getData != null)
            return getData;
        else
            return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyDreamCatcher
{
    public int dreamCatcherCnt; // 드림캐쳐 아이디 상태
    public int nDreamCatcher; // 드림캐쳐 갯수
    public List<DreamCatcher> dreamCatcherList;

    public MyDreamCatcher()
    {
        dreamCatcherCnt = 1000;
        dreamCatcherList = new List<DreamCatcher>();
        nDreamCatcher = dreamCatcherList.Count;
        //ResetDreamCatcherData();
    }

    // Index로 드림캐쳐 찾기
    public DreamCatcher GetDreamCatcherDataByIndex(int index)
    {
        if (index < 0 || index >= dreamCatcherList.Count)
        {
            Debug.LogError($"[MyDreamCatcher] Index {index} is out of range.");
            return null;
        }

        return dreamCatcherList[index];
    }

    // ID로 드림캐쳐 찾기
    public DreamCatcher GetDreamCatcherById(string id)
    {
        DreamCatcher result = dreamCatcherList.FirstOrDefault(dc => dc.DCid == id);

        if(result == null)
        {
            Debug.LogWarning($"[MyDreamCatcher] DreamCatcher with ID {id} not found!");
        }

        return result;
    }

    // 드림캐쳐 데이터 리셋
    public void ResetDreamCatcherData()
    {
        dreamCatcherCnt = 1000;
        dreamCatcherList.Clear();
        nDreamCatcher = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
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

    public DreamCatcher GetDreamCatcherData(int index)
    {
        Debug.Log("arrange test: "+dreamCatcherList[index]);
        return dreamCatcherList[index];
    }


    public void ResetDreamCatcherData()
    {
        dreamCatcherCnt = 1000;
        dreamCatcherList.Clear();
        nDreamCatcher = 0;
    }
}

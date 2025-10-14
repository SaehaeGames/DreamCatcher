using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestDeliveryChecker : MonoBehaviour
{
    List<QuestDreamCatcherInfo_Object> questDreamCatcehrInfoDatalist;
    List<DreamCatcher> dreamCatcherListData;
    // Start is called before the first frame update
    void Start()
    {
        // 데이터 불러오기
        questDreamCatcehrInfoDatalist = GameManager.instance.questDreamCatcherInfo_data.dataList;
        Debug.Log("questDreamCatcherInfoDatalist : " + questDreamCatcehrInfoDatalist[0].lines[4]);

        dreamCatcherListData = GameManager.instance.dreamCatcherDataManager.dreamCatcherList;
        TryDeliveryDreamCatcher();
    }

    public void TryDeliveryDreamCatcher()
    {
        DreamCatcher questDreamCatcher = DreamCatcherFactory.ConvertToDreamCatcher(questDreamCatcehrInfoDatalist[0]);
        bool testResult=IsSameDreamCatcher(questDreamCatcher, dreamCatcherListData[0]);
        if(testResult)
        {
            Debug.Log("dreamCatcher is Same");
        }
        else
        {
            Debug.Log("dreamCatcher is not Same");
        }
    }

    private bool IsSameDreamCatcher(DreamCatcher dreamCatcher1, DreamCatcher dreamCatcher2)
    {
        if(dreamCatcher1 == null || dreamCatcher2 == null)
        {
            Debug.LogError("dreamCatcher1 or dreamCatcher2 is null 비교 불가");
            return false;
        }

        Debug.Log(dreamCatcher1.DCid + " vs " + dreamCatcher2.DCid);

        // 드림캐쳐 색 비교
        if (dreamCatcher1.DCcolor != dreamCatcher2.DCcolor)
        {
            Debug.Log("dreamCatcher is not Same - color");
            return false;
        }

        // 드림캐쳐 깃털 비교
        if(dreamCatcher1.DCfeather1 != dreamCatcher2.DCfeather1)
        {
            Debug.Log("dreamCatcher is not Same - feather1");
            return false;
        }
        if(dreamCatcher1.DCfeather2 != dreamCatcher2.DCfeather2)
        {
            Debug.Log("dreamCatcher is not Same - feather2");
            return false;
        }
        if(dreamCatcher1.DCfeather3 != dreamCatcher2.DCfeather3)
        {
            Debug.Log("dreamCatcher is not Same - feather3");
            return false;
        }

        // 드림캐쳐 모양 비교
        if (!dreamCatcher1.DCline.SequenceEqual(dreamCatcher2.DCline))
        {
            Debug.Log("dreamCatcher is not Same - lines");
            return false;
        }

        // 드림캐쳐 보석 위치 비교
        if (!dreamCatcher1.DCbead.SequenceEqual(dreamCatcher2.DCbead))
        {
            Debug.Log("dreamCatcher is not Same - beads");
            return false;
        }

        return true;
    }
}

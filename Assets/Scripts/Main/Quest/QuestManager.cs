using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    //퀘스트 내용을 데이터에서 가져와서 설정하는 스크립트
    //json은 왜 한거지..? 엑셀에서는 편지 내용만 가져오고 받은 날짜, 읽은 날짜, 읽은 여부는 json으로 관리할까..

    [Header("[Quest View]")]
    public GameObject contentTexts;       //텍스트 오브젝트
    public GameObject deliveryView;     //납품 오브젝트
    public int curQuestNumber;

    private PlayerDataManager playerDataManager;
    private QuestDataManager questDataManager;
    private QuestInfo_Data questInfo_Data;

    private void Start()
    {
        playerDataManager = GameManager.instance.playerDataManager;
        questDataManager = GameManager.instance.questDataManager;
        questInfo_Data = GameManager.instance.questinfo_data;
    }

    public QuestData GetCurrentQuestData()
    {
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();

        return questDataManager.GetQuestData(currentMainQuestIndex);
    }

    public string GetCurrentMainQuestTitle()
    {
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();
        bool isClear = questDataManager.IsQuestCleared(currentMainQuestIndex);

        string title;
        if (isClear)
        {
            title = questInfo_Data.dataList[currentMainQuestIndex * 2 + 1].title.ToString();
        }
        else
        {
            title = questInfo_Data.dataList[currentMainQuestIndex * 2].title.ToString();
        }

        return title;
    }

    public string GetCurrentMainQuestContents()
    {
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();

        bool isClear = questDataManager.IsQuestCleared(currentMainQuestIndex);

        string contents;

        if (isClear) // 클리어 시
        {
            contents = questInfo_Data.dataList[currentMainQuestIndex * 2 + 1].contents.ToString();
        }
        else // 미클리어 시
        {
            contents = questInfo_Data.dataList[currentMainQuestIndex * 2].contents.ToString();
        }

        contents = contents.Replace("nn", "\n"); //퀘스트 내용 변경
        return contents;
    }

    public string GetCurrentMainQuestFrom()
    {
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();
        bool isClear = questDataManager.IsQuestCleared(currentMainQuestIndex);

        string from;
        if (isClear)
        {
            from = questInfo_Data.dataList[currentMainQuestIndex * 2 + 1].from.ToString();
        }
        else
        {
            from = questInfo_Data.dataList[currentMainQuestIndex * 2].from.ToString();
        }

        return from;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum QuestFlowState
{
    None,
    BeforeStart,
    Progress,
    DeliveryCompleted,
    QuestClear
}

public enum QuestType
{
    None = -1,
    MainQuest = 0,
    RepeatQuest = 1,
    Charon = 2
}

public class QuestManager : MonoBehaviour
{
    //퀘스트 내용을 데이터에서 가져와서 설정하는 스크립트
    //json은 왜 한거지..? 엑셀에서는 편지 내용만 가져오고 받은 날짜, 읽은 날짜, 읽은 여부는 json으로 관리할까..

    [Header("[Quest View]")]
    public GameObject contentTexts;       //텍스트 오브젝트
    public GameObject deliveryView;     //납품 오브젝트
    public int curQuestNumber;
    public QuestNotice questNotice;

    [Space]
    [Header("[Managers]")]
    public GameObject managers;

    private PlayerDataManager playerDataManager;
    private QuestDataManager questDataManager;
    private QuestInfo_Data questInfo_Data;
    private CharonInfo_Data charonInfo_Data;

    private void Start()
    {
        playerDataManager = GameManager.instance.playerDataManager;
        questDataManager = GameManager.instance.questDataManager;
        questInfo_Data = GameManager.instance.questinfo_data;
        charonInfo_Data = GameManager.instance.charoninfo_data;

        // 강제 종료시 연출 복원
        if (playerDataManager.GetIsQuestActinoPlaying())
        {
            int currentQuestIndex = GetCurrentMainQuestIndex();

            if (GetCurrentQuestFlowState() == QuestFlowState.Progress)
            {
                this.GetComponent<QuestActionController>().PlayQuestAction(QuestActionType.Accept, currentQuestIndex);
            }
            else if (GetCurrentQuestFlowState() == QuestFlowState.DeliveryCompleted)
            {
                this.GetComponent<QuestActionController>().PlayQuestAction(QuestActionType.Complete, currentQuestIndex);
                ClearMainQuest(currentQuestIndex);
            }
        }
    }
    
    public void AcceptMainQuest()
    {
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();
        bool isChecked = questDataManager.IsQuestChecked(currentMainQuestIndex);
        if (!isChecked)
        {
            questDataManager.CheckStartQuest(currentMainQuestIndex);
            questDataManager.Save();
        }
    }

    public void ClearMainQuest(int questIndex)
    {
        questDataManager.ClearQuest(questIndex);
    }

    public void ReceiveQuest(QuestType questType)
    {
        switch(questType)
        {
            case QuestType.MainQuest:
                // 현재 퀘스트 데이터 업데이트
                int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();
                playerDataManager.SetCurrentMainQuestIndex(currentMainQuestIndex + 1);
                break;
            case QuestType.RepeatQuest:
                break;
            case QuestType.Charon:
                // 다음 퀘스트 인덱스로 업데이트
                int currentCharonLetterIndex = playerDataManager.GetCurrentCharonLetterIndex();
                playerDataManager.SetCurrentCharonLetterIndex(currentCharonLetterIndex + 1);
                break;
        }

        questNotice.Notice(questType);
    }

    public QuestFlowState GetCurrentQuestFlowState()
    {
        int currentQuestIndex = playerDataManager.GetCurrentMainQuestIndex();
        bool isEndChecked = questDataManager.IsQuestEndChecked(currentQuestIndex);
        bool isStartChecked = questDataManager.IsQuestChecked(currentQuestIndex);
        bool isClear = questDataManager.IsQuestCleared(currentQuestIndex);

        if (isStartChecked && isEndChecked && isClear)
        {
            return QuestFlowState.QuestClear;
        }
        else if(isStartChecked && isEndChecked && !isClear)
        {
            return QuestFlowState.DeliveryCompleted;
        }
        else if (isStartChecked && !isEndChecked && !isClear)
        {
            return QuestFlowState.Progress;
        }
        else if(!isStartChecked && !isEndChecked && !isClear)
        {
            return QuestFlowState.BeforeStart;
        }

        return QuestFlowState.None;
    }

    public int GetCurrentMainQuestIndex()
    {
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();
        return currentMainQuestIndex;
    }

    public int GetCurrentCharonLetterIndex()
    {
        int currentCharonLetterIndex = playerDataManager.GetCurrentCharonLetterIndex();
        return currentCharonLetterIndex;
    }

    public QuestData GetCurrentQuestData()
    {
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();

        return questDataManager.GetQuestData(currentMainQuestIndex);
    }

    public string GetCurrentMainQuestTitle()
    {
        QuestFlowState questFlowState = GetCurrentQuestFlowState();
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();

        string title;
        if (questFlowState == QuestFlowState.DeliveryCompleted)
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
        QuestFlowState questFlowState = GetCurrentQuestFlowState();
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();

        string contents;

        if (questFlowState == QuestFlowState.DeliveryCompleted) // 클리어 시
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
        QuestFlowState questFlowState = GetCurrentQuestFlowState();
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();

        string from;
        if (questFlowState == QuestFlowState.DeliveryCompleted)
        {
            from = questInfo_Data.dataList[currentMainQuestIndex * 2 + 1].from.ToString();
        }
        else
        {
            from = questInfo_Data.dataList[currentMainQuestIndex * 2].from.ToString();
        }

        return from;
    }

    public string GetCurrentCharonLetterContents()
    {
        int currentCharonLetterIndex = playerDataManager.GetCurrentCharonLetterIndex();
        string contents = charonInfo_Data.dataList[currentCharonLetterIndex].contents;
        
        contents = contents.Replace("nn", "\n"); //퀘스트 내용 변경
        return contents;
    }

    public void PlayCurrentQuestAction(QuestType questType)
    {
        if (questType == QuestType.MainQuest)
        {
            PlayMainQuestAction();
        }
        else if (questType == QuestType.Charon)
        {
            PlayCharonQuestAction();
        }
    }

    private void PlayMainQuestAction()
    {
        QuestFlowState state = GetCurrentQuestFlowState();
        int questIndex = GetCurrentMainQuestIndex();

        switch (state)
        {
            case QuestFlowState.BeforeStart:
                this.GetComponent<QuestActionController>().PlayQuestAction(
                    QuestActionType.Accept,
                    questIndex,
                    () =>
                    {
                        playerDataManager.SetIsQuestActionPlaying(true);
                        playerDataManager.Save();
                    },    
                    () =>
                    {
                        playerDataManager.SetIsQuestActionPlaying(false);
                        AcceptMainQuest();
                        playerDataManager.Save();
                        questDataManager.Save();
                    });
                break;

            case QuestFlowState.DeliveryCompleted:
                this.GetComponent<QuestActionController>().PlayQuestAction(
                    QuestActionType.Complete,
                    questIndex,
                    () =>
                    {
                        playerDataManager.SetIsQuestActionPlaying(true);
                        playerDataManager.Save();
                    },
                    () =>
                    {
                        playerDataManager.SetIsQuestActionPlaying(false);
                        ClearMainQuest(questIndex);
                        ReceiveQuest(QuestType.MainQuest);
                        playerDataManager.Save();
                        questDataManager.Save();
                    });
                break;
        }
    }

    private void PlayCharonQuestAction()
    {
        int questIndex = GetCurrentCharonLetterIndex();

        this.GetComponent<QuestActionController>().PlayQuestAction(
            QuestActionType.Accept,
            questIndex,
            () =>
            {
                playerDataManager.SetIsCharonQuestActionPlaying(true);
                playerDataManager.Save();
            },
            () =>
            {
                playerDataManager.SetIsCharonQuestActionPlaying(false);
                playerDataManager.Save();
            });
    }
}

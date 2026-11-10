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
    private RepeatQuestDataManager repeatQuestDataManager;
    private QuestInfo_Data questInfo_Data;
    private CharonInfo_Data charonInfo_Data;
    private RepeatQuestInfo_Data repeatQuestInfo_Data;

    private readonly Dictionary<int, int[]> availableFeathersByLevel = new()
    {
        { 0, new int[] { 0, 1, 2 } },
        { 1, new int[] { 0, 1, 2, 4, 5, 6 } },
        { 2, new int[] { 0, 1, 2, 4, 5, 6, 8, 9, 10 } },
        { 3, new int[] { 0, 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14 } }
    };

    private void Start()
    {
        playerDataManager = GameManager.instance.playerDataManager;
        questDataManager = GameManager.instance.questDataManager;
        repeatQuestDataManager = GameManager.instance.repeatQuestDataManager;
        questInfo_Data = GameManager.instance.questinfo_data;
        charonInfo_Data = GameManager.instance.charoninfo_data;
        repeatQuestInfo_Data = GameManager.instance.repeatQuestInfo_data;

        // 강제 종료시 연출 복원
        if (playerDataManager.GetIsQuestActinoPlaying())
        {
            int currentQuestIndex = GetCurrentMainQuestIndex();
            Debug.Log($"강제 종료 복원 시작 / QuestIndex : {currentQuestIndex}");

            PlayMainQuestAction();
        }
        else if (playerDataManager.GetIsCharonQuestActionPlaying())
        {
            int currentCharonIndex = GetCurrentCharonLetterIndex();
            Debug.Log($"강제 종료 복원 시작 / 카론");
            PlayCharonQuestAction();
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
                playerDataManager.SetHasReadCharonLetter(false);
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

            // 번역퀘만 따로 처리
            if (questInfo_Data.HasTranslation(contents))
            {
                contents = questInfo_Data.GetOriginalContents(contents);
            }
        }
        else // 미클리어 시
        {
            contents = questInfo_Data.dataList[currentMainQuestIndex * 2].contents.ToString();

            // 번역퀘만 따로 처리
            if (questInfo_Data.HasTranslation(contents))
            {
                contents = questInfo_Data.GetOriginalContents(contents);
            }
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

    public string GetCurrentMainQuestMagnifierContents()
    {
        QuestFlowState questFlowState = GetCurrentQuestFlowState();
        int currentMainQuestIndex = playerDataManager.GetCurrentMainQuestIndex();

        string contents;

        if (questFlowState == QuestFlowState.DeliveryCompleted) // 클리어 시
        {
            contents = questInfo_Data.dataList[currentMainQuestIndex * 2 + 1].contents.ToString();
            contents = questInfo_Data.GetTranslatedContents(contents);
        }
        else // 미클리어 시
        {
            contents = questInfo_Data.dataList[currentMainQuestIndex * 2].contents.ToString();
            contents = questInfo_Data.GetTranslatedContents(contents);
        }

        contents = contents.Replace("nn", "\n"); //퀘스트 내용 변경
        return contents;
    }

    public string GetCurrentCharonLetterContents()
    {
        int currentCharonLetterIndex = playerDataManager.GetCurrentCharonLetterIndex();
        string contents = charonInfo_Data.dataList[currentCharonLetterIndex].contents;
        
        contents = contents.Replace("nn", "\n"); //퀘스트 내용 변경
        return contents;
    }

    // 반복퀘스트 콘텐츠
    private List<int> GetAvailableFeatherIds(int foodUnlockLevel)
    {
        if (availableFeathersByLevel.TryGetValue(
            foodUnlockLevel, out int[] featherIds))
        {
            return new List<int>(featherIds);
        }

        Debug.LogError($"지원하지 않는 먹이 해금 레벨: {foodUnlockLevel}");
        return new List<int>();
    }

    private void SetRandomFeathers(RepeatQuestDreamCatcher quest, List<int> featherIds)
    {
        quest.DCcolor = 0;
        quest.DCfeather1 = featherIds[UnityEngine.Random.Range(0, featherIds.Count)];
        quest.DCfeather2 = featherIds[UnityEngine.Random.Range(0, featherIds.Count)];
        quest.DCfeather3 = featherIds[UnityEngine.Random.Range(0, featherIds.Count)];
    }

    public void GenerateRepeatQuestDreamCatchers()
    {
        int foodUnlockLevel = playerDataManager.GetFoodUnlockLevel();

        List<int> featherIds = GetAvailableFeatherIds(foodUnlockLevel);

        if (featherIds.Count == 0)
            return;

        List<RepeatQuestDreamCatcher> quests = new List<RepeatQuestDreamCatcher>();

        for (int i = 0; i < 3; i++)
        {
            RepeatQuestDreamCatcher quest = new RepeatQuestDreamCatcher();

            quest.DCid = "JS_" + (8000 + i);

            SetRandomFeathers(quest, featherIds);

            quests.Add(quest);
        }

        repeatQuestDataManager.SetRepeatQuestDreamCatcherList(quests);
    }
    public RepeatQuestDreamCatcher GetRepeatQuestDreamCatcher(int index)
    {
        if (!repeatQuestDataManager.GetHasReadFirstLetter())
        {
            return null;
        }
        RepeatQuestDreamCatcher dreamCatcher = repeatQuestDataManager.GetRepeatQuestDreamCatcherList(index);
        return dreamCatcher;
    }

    public string GetRepeatQuestLetterTitle()
    {
        if (repeatQuestDataManager.GetHasReadFirstLetter())
        {
            return null;
        }

        string contents = repeatQuestInfo_Data.dataList[0].title;
        return contents;
    }

    public string GetRepeatQuestLetterFrom()
    {
        if (repeatQuestDataManager.GetHasReadFirstLetter())
        {
            return null;
        }

        string contents = repeatQuestInfo_Data.dataList[0].from;
        return contents;
    }

    public string GetRepeatQuestLetterContents()
    {
        if(repeatQuestDataManager.GetHasReadFirstLetter())
        {
            return null;
        }

        string contents = repeatQuestInfo_Data.dataList[0].contents;
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
            // 이미 읽었다면 퀘스트 액션 재생하지 않음
            if (playerDataManager.GetHasReadCharonLetter())
            {
                return;
            }

            PlayCharonQuestAction();
        }
        else
        {
            return;
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
            QuestActionType.Charon,
            questIndex,
            () =>
            {
                playerDataManager.SetIsCharonQuestActionPlaying(true);
                playerDataManager.Save();
            },
            () =>
            {
                playerDataManager.SetIsCharonQuestActionPlaying(false);
                playerDataManager.SetHasReadCharonLetter(true);
                playerDataManager.Save();
            });
    }
}

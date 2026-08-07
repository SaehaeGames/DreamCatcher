using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LetterType
{
    None,
    RepeatQuest,
    Charon
}

public class InteractiveSequenceUnlockLetter : InteractiveSequenceBase
{
    [Header("해금 설정")]
    public LetterType letterType;

    [Header("UI 연결")]
    public GameObject unlockPopUp; // TutorialOverlay의 UnlockPopUp 연결
    public Button okBtn;           // UnlockPopUp 안의 OKBtn 연결

    private PlayerDataManager playerDataManager;
    private bool isOKBtnClicked = false;

    public override void Enter()
    {
        isOKBtnClicked = false;

        playerDataManager = GameManager.instance.playerDataManager;

        // 1. 데이터 해금
        if(letterType == LetterType.RepeatQuest )
        {
            playerDataManager.UnlockRepeatQuest();
        }
        else if(letterType == LetterType.Charon )
        {
            playerDataManager.UnlockCharonLetter();
        }

        // 2. 팝업 띄우기
        if (unlockPopUp != null)
        {
            unlockPopUp.SetActive(true);
        }

        // 3. OK 버튼 클릭 이벤트 등록
        if (okBtn != null)
        {
            okBtn.onClick.RemoveAllListeners();
            okBtn.onClick.AddListener(OnOkButtonClicked);
        }
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        // 버튼을 눌렀다면 다음 튜토리얼 스텝으로 넘어감
        if (isOKBtnClicked)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        // 버튼을 눌렀다면 다음 퀘스트 액션으로 넘어감
        if (isOKBtnClicked)
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    public override void Exit()
    {
        // 다음 스텝으로 넘어갈 때 팝업 닫기 및 이벤트 정리
        if (unlockPopUp != null)
        {
            unlockPopUp.SetActive(false);
        }

        if (okBtn != null)
        {
            okBtn.onClick.RemoveAllListeners();
        }
    }

    private void OnOkButtonClicked()
    {
        isOKBtnClicked = true;
    }

}

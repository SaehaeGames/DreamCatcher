using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceDialog : InteractiveSequenceBase
{
    private ScriptBox scriptBox;
    [Header("대사 시작 아이디-입력")]
    public int startId;
    [Header("대사 끝 아이디-입력")]
    public int endId;

    public override void Enter()
    {
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
        if (scriptBox == null)
        {
            Debug.LogError($"[InteractiveSequenceDialog] {gameObject.name}에서 ScriptBox를 찾을 수 없습니다.");
            return;
        }

        scriptBox.ScriptBoxOnOff(true);
        scriptBox.SetScriptBox(startId, endId);
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (scriptBox == null) return;

        bool isCompleted = scriptBox.ReturnNextScript();

        if (isCompleted)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
            isCompleted = false;
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (scriptBox == null) return;

        bool isCompleted = scriptBox.ReturnNextScript();

        if (isCompleted)
        {
            questActionPipeline.SetNextQuestAction();
            isCompleted = false;
        }
    }

    public override void Exit()
    {
        if (scriptBox != null)
        {
            scriptBox.ScriptBoxOnOff(false);
        }
    }
}

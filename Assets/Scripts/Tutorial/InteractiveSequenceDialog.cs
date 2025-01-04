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
        scriptBox.ScriptBoxOnOff(true);
        scriptBox.SetScriptBox(startId, endId);
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        bool isCompleted = scriptBox.ReturnNextScript();

        if (isCompleted)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
            isCompleted = false;
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        bool isCompleted = scriptBox.ReturnNextScript();

        if (isCompleted)
        {
            questActionPipeline.SetNextQuestAction();
            isCompleted = false;
        }
    }

    public override void Exit()
    {
        scriptBox.ScriptBoxOnOff(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceReceiveLetter : InteractiveSequenceBase
{
    public LetterType letterType;
    private QuestManager questManager;

    public override void Enter()
    {
        questManager = FindFirstObjectByType<QuestManager>();

        switch (letterType)
        {
            case LetterType.Charon:
                questManager.ReceiveQuest(QuestType.Charon);
                break;
            case LetterType.RepeatQuest:
                break;
        }

        //tutorialPipeline.SetNextTutorial(SceneState.None);
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {

    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {

    }

    public override void Exit()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceReceiveLetter : InteractiveSequenceBase
{
    public LetterType letterType;
    private QuestManager questManager;
    private bool goNext = false;

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

        // юс╫ц
        goNext = true;
        //pipeline.SetNextTutorial(SceneState.None);
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (goNext)
        {
            goNext = false;
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (goNext)
        {
            goNext = false;
            questActionPipeline.SetNextQuestAction();
        }
    }

    public override void Exit()
    {

    }
}

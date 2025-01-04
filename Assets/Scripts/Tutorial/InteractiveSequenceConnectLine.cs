using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceConnectLine : InteractiveSequenceBase
{
    [SerializeField] private GameObject dragPoints;
    private int numberOfTimesCorrect;

    public override void Enter()
    {
        numberOfTimesCorrect = 0;
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (numberOfTimesCorrect == 4)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (numberOfTimesCorrect == 4)
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    public override void Exit()
    {

    }

    public void PlusNumberOfTimesCorrect()
    {
        numberOfTimesCorrect++;
    }
}

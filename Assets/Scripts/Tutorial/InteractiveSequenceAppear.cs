using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceAppear : InteractiveSequenceBase
{
    [SerializeField] private GameObject appearObject;
    [SerializeField] private bool doesItMakeObjectAppear;
    private bool appear = false;

    public override void Enter()
    {
        appearObject.SetActive(doesItMakeObjectAppear);
        appear = true;
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if(appear)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (appear)
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    public override void Exit()
    {
        
    }
}

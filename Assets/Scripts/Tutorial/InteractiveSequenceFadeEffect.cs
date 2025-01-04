using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceFadeEffect : InteractiveSequenceBase
{
    [SerializeField]
    private Fade fadeEffect;
    [SerializeField]
    //private bool isFadeIn = false;
    private bool isCompleted = false;

    public override void Enter()
    {
        fadeEffect.gameObject.SetActive(true);
        fadeEffect.FadeIn(OnAfterFadeEffect);
    }

    private void OnAfterFadeEffect()
    {
        isCompleted = true;
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (isCompleted == true)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
            isCompleted = false;
        }
    }

    

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (isCompleted == true)
        {
            questActionPipeline.SetNextQuestAction();
            isCompleted = false;
        }
    }

    public override void Exit() { }    
    
}

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
        isCompleted = false;
        if (fadeEffect == null)
        {
            Debug.LogError($"[InteractiveSequenceFadeEffect] {gameObject.name}의 fadeEffect가 설정되지 않았습니다.");
            return;
        }

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

    public override void Exit()
    {
        if (fadeEffect != null)
        {
            fadeEffect.StopFade();
        }
    }
    
}

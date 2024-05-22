using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFadeEffect : TutorialBase
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

    public override void Execute(TutorialController controller)
    {
        if (isCompleted == true)
        {
            controller.SetNextTutorial();
            isCompleted = false;
        }
    }

    public override void Exit()
    {
    }
}

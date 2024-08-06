using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAppear : TutorialBase
{
    [SerializeField] private GameObject appearObject;
    [SerializeField] private bool doesItMakeObjectAppear;
    private bool appear = false;

    public override void Enter()
    {
        appearObject.SetActive(doesItMakeObjectAppear);
        appear = true;
    }

    public override void Execute(TutorialController controller)
    {
        if(appear)
        {
            controller.SetNextTutorial(SceneState.None);
        }
    }

    public override void Exit()
    {
        
    }
}

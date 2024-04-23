using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAppear : TutorialBase
{
    [SerializeField] private GameObject appearObject;
    private bool appear = false;

    public override void Enter()
    {
        appearObject.SetActive(true);
        appear = true;
    }

    public override void Execute(TutorialController controller)
    {
        if(appear)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        
    }
}

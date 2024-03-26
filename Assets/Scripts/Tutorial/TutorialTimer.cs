using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTimer : TutorialBase
{
    [SerializeField]
    private int waitingTime;

    private float timer;

    public override void Enter()
    {
        timer = 0.0f;
    }

    public override void Execute(TutorialController controller)
    {
        timer += Time.deltaTime;
        if (timer > waitingTime)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {

    }
}

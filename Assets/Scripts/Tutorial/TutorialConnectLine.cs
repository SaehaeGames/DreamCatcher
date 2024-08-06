using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialConnectLine : TutorialBase
{
    [SerializeField] private GameObject dragPoints;
    private int numberOfTimesCorrect;

    public override void Enter()
    {
        numberOfTimesCorrect = 0;
    }

    public override void Execute(TutorialController controller)
    {
        if (numberOfTimesCorrect == 4)
        {
            controller.SetNextTutorial(SceneState.None);
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

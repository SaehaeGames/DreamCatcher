using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialConnectLine : TutorialBase
{
    [SerializeField] private GameObject dragPoints;
    [SerializeField] private GameObject guidImg;
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
        if (guidImg != null)
        {
            guidImg.SetActive(false);
        }
    }

    public void PlusNumberOfTimesCorrect()
    {
        numberOfTimesCorrect++;
    }
}

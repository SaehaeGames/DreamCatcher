using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTimer : TutorialBase
{
    [Header("��� �ð�(��)-�Է�")]
    [SerializeField] private int waitingTime; // ��� �ð�(������ �Է�)
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
            controller.SetNextTutorial(SceneState.None);
        }
    }

    public override void Exit()
    {

    }
}

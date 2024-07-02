using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTimer : TutorialBase
{
    [Header("대기 시간(초)-입력")]
    [SerializeField] private int waitingTime; // 대기 시간(개발자 입력)
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

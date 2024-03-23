using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : TutorialBase
{
    [SerializeField]
    private GameObject arrow;
    [SerializeField]
    private GameObject clickBtn;
    [SerializeField]
    private Vector2 arrowPos;

    public override void Enter()
    {
        Instantiate(arrow, arrowPos, Quaternion.identity);
        arrow.GetComponent<Animator>().enabled = true;
        arrow.GetComponent<Animator>().Play("blinkArrow");
    }

    public override void Execute(TutorialController controller)
    {
        
    }

    public override void Exit()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialog : TutorialBase
{
    private ScriptBox scriptBox;
    [Header("대사 시작 아이디-입력")]
    public int startId;
    [Header("대사 끝 아이디-입력")]
    public int endId;

    public override void Enter()
    {
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
        scriptBox.ScriptBoxOnOff(true);
        scriptBox.SetScriptBox(startId, endId);
    }

    public override void Execute(TutorialController controller)
    {
        bool isCompleted = scriptBox.ReturnNextScript(); // 수정 필요
        //Debug.Log("Dialog Execute : " + isCompleted);

        if (isCompleted)
        {
            controller.SetNextTutorial();
            isCompleted = false;
        }
    }

    public override void Exit()
    {
        scriptBox.ScriptBoxOnOff(false);
    }
}
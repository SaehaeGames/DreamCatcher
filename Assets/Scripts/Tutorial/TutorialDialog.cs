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
        Debug.Log("Dialog");
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
        scriptBox.gameObject.SetActive(true);
        scriptBox.StartScript(startId, endId);
        Debug.Log(scriptBox.startId + " " + scriptBox.endId);

    }

    public override void Execute(TutorialController controller)
    {
        bool isCompleted = scriptBox.NextScript(); // 수정 필요
        Debug.Log("Dialog Execute : " + isCompleted);

        if (isCompleted)
        {
            Debug.Log("대사 끝");
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        //scriptBox.gameObject.SetActive(false);
    }
}
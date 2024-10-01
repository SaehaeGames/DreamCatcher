using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDummy : TutorialBase
{
    private ScriptBox scriptBox;

    public override void Enter()
    {
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
        scriptBox.ScriptBoxOnOff(false);
    }

    public override void Execute(TutorialController controller)
    {
        
    }

    public override void Exit()
    {
        
    }
}

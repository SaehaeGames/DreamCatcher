using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceDummy : InteractiveSequenceBase
{
    private ScriptBox scriptBox;

    public override void Enter()
    {
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
        scriptBox.ScriptBoxOnOff(false);
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {

    }

    public override void Exit()
    {
        
    }
}

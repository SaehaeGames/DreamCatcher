using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceReceiveLetter : InteractiveSequenceBase
{
    public LetterType letterType;
    private QuestManager questManager;
    private bool isCompleted = false;

    public override void Enter()
    {
        isCompleted = false;
        questManager = FindFirstObjectByType<QuestManager>();

        switch (letterType)
        {
            case LetterType.Charon:
                questManager.ReceiveQuest(QuestType.Charon);
                break;
            case LetterType.RepeatQuest:
                break;
        }

        isCompleted = true;
    }

    public override void Execute()
    {
        if(isCompleted)
        {
            isCompleted = false;
            pipeline.SetSceneState(SceneState.None);
            pipeline.MoveToNextSequence();
        }
    }

    public override void Exit()
    {

    }
}

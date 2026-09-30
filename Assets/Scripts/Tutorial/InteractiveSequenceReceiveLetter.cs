using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceReceiveLetter : InteractiveSequenceBase
{
    public LetterType letterType;
    private QuestManager questManager;
    private bool goNext = false;

    public override void Enter()
    {
        questManager = FindFirstObjectByType<QuestManager>();

        switch (letterType)
        {
            case LetterType.Charon:
                questManager.ReceiveQuest(QuestType.Charon);
                break;
            case LetterType.RepeatQuest:
                break;
        }

        pipeline.SetSceneState(SceneState.None);
        pipeline.MoveToNextSequence();
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LetterType
{
    None,
    RepeatQuest,
    Charon
}

public class InteractiveSequenceUnlockLetter : InteractiveSequenceBase
{
    public LetterType letterType;

    private PlayerDataManager playerDataManager;

    public override void Enter()
    {
        playerDataManager = GameManager.instance.playerDataManager;
        if(letterType == LetterType.RepeatQuest )
        {
            playerDataManager.UnlockRepeatQuest();
        }
        else if(letterType == LetterType.Charon )
        {
            playerDataManager.UnlockCharonLetter();
        }
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

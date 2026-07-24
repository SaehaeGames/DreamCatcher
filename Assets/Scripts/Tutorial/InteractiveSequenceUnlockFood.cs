using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoodType
{
    None,
    PigeonBean,
    Berry,
    Earthworm,
    LeanMeat

}

public class InteractiveSequenceUnlockFood : InteractiveSequenceBase
{
    public FoodType foodType;

    private PlayerDataManager playerDataManager;

    public override void Enter()
    {
        playerDataManager = GameManager.instance.playerDataManager;

        if (foodType == FoodType.PigeonBean)
        {
            playerDataManager.UnlockFoodLevel(0);
        }
        else if (foodType == FoodType.Berry)
        {
            playerDataManager.UnlockFoodLevel(1);
        }
        else if (foodType == FoodType.Earthworm)
        {
            playerDataManager.UnlockFoodLevel(2);
        }
        else if (foodType == FoodType.LeanMeat)
        {
            playerDataManager.UnlockFoodLevel(3);
        }
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        throw new System.NotImplementedException();
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}

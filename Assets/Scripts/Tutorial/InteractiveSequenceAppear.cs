using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceAppear : InteractiveSequenceBase
{
    [SerializeField] private GameObject appearObject;
    [SerializeField] private bool doesItMakeObjectAppear;
    private bool isCompleted = false;

    public override void Enter()
    {
        if (appearObject == null)
        {
            Debug.LogError($"[InteractiveSequenceAppear] {gameObject.name}의 appearObject가 설정되지 않았습니다.");
            isCompleted = false;
            return;
        }

        appearObject.SetActive(doesItMakeObjectAppear);
        isCompleted = true;
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if(isCompleted)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (isCompleted)
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    public override void Exit()
    {
        isCompleted = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceTimer : InteractiveSequenceBase
{
    [Header("대기 시간(초)-입력")]
    [SerializeField] private float waitingTime; // 대기 시간(개발자 입력)
    private float timer;
    private ScriptBox scriptBox;

    public override void Enter()
    {
        // 스크립트 박스 관리
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            int index = transform.GetSiblingIndex();
            if (index == 0)
            {
                scriptBox.gameObject.GetComponent<ScriptBox>().ScriptBoxOnOff(false);
            }
        }
        timer = 0.0f;
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        timer += Time.deltaTime;
        if (timer > waitingTime)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        timer += Time.deltaTime;
        if (timer > waitingTime)
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    public override void Exit()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSequenceTimer : InteractiveSequenceBase
{
    [Header("��� �ð�(��)-�Է�")]
    [SerializeField] private float waitingTime; // ��� �ð�(������ �Է�)
    private float timer;
    private ScriptBox scriptBox;

    public override void Enter()
    {
        // ��ũ��Ʈ �ڽ� ����
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

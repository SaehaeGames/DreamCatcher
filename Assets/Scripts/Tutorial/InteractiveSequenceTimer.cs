using UnityEngine;

public enum InteractiveSequenceTimerMode
{
    Time,
    ObjectActivation
}

public class InteractiveSequenceTimer : InteractiveSequenceBase
{
    [Header("대기 설정")]
    [SerializeField] private float waitingTime;
    [SerializeField] private InteractiveSequenceTimerMode waitMode;
    [SerializeField] private GameObject waitObject;

    private float timer;
    private ScriptBox scriptBox;

    public override void Enter()
    {
        scriptBox = FindObjectOfType<ScriptBox>();
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            int index = transform.GetSiblingIndex();
            if (index == 0 && scriptBox != null)
            {
                scriptBox.ScriptBoxOnOff(false);
            }
        }

        timer = 0.0f;

        if (waitMode == InteractiveSequenceTimerMode.ObjectActivation && waitObject == null)
        {
            Debug.LogError($"[InteractiveSequenceTimer] {gameObject.name}: waitObject가 설정되지 않았습니다.");
        }
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (IsWaitCompleted())
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (IsWaitCompleted())
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    public override void Exit()
    {
    }

    private bool IsWaitCompleted()
    {
        if (waitMode == InteractiveSequenceTimerMode.ObjectActivation)
        {
            return waitObject != null && waitObject.activeInHierarchy;
        }

        timer += Time.deltaTime;
        return timer > waitingTime;
    }
}

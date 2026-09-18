using UnityEngine;

/// <summary>
/// 튜토리얼 대기의 완료 조건을 구분함.
/// Time은 경과 시간, ObjectActivation은 지정 오브젝트의 활성 상태.
/// </summary>
public enum InteractiveSequenceTimerMode
{
    Time,
    ObjectActivation
}

/// <summary>
/// 범용 대기 시퀀스. 일정 시간이 지나거나 지정 오브젝트가 활성화될 때까지 공용 파이프라인의 진행을 기다림.
/// </summary>
public class InteractiveSequenceTimer : InteractiveSequenceBase
{
    [Header("대기 설정")]
    [SerializeField] private float waitingTime;
    [SerializeField] private InteractiveSequenceTimerMode waitMode;
    [SerializeField] private GameObject waitObject;

    private float elapsedTime;

    /// <summary>
    /// 대기 시간을 초기화하고 이전 대사창을 닫음.
    /// </summary>
    public override void Enter()
    {
        // 1. 남아 있을 수 있는 이전 대사창을 닫음.
        TutorialContext context = TutorialContext.Get(this);
        ScriptBox scriptBox = context != null ? context.ScriptBox : FindObjectOfType<ScriptBox>();
        if (transform.GetSiblingIndex() == 0 && scriptBox != null)
        {
            scriptBox.ScriptBoxOnOff(false);
        }

        // 2. 시간 대기 상태를 처음부터 다시 시작함.
        elapsedTime = 0f;

        // 3. 오브젝트 활성화 모드의 필수 참조를 미리 검사함.
        if (waitMode == InteractiveSequenceTimerMode.ObjectActivation && waitObject == null)
        {
            Debug.LogError($"[InteractiveSequenceTimer] {gameObject.name}: waitObject가 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 선택된 대기 조건이 충족되면 다음 튜토리얼 시퀀스로 진행함.
    /// </summary>
    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (IsWaitCompleted())
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    /// <summary>
    /// 선택된 대기 조건이 충족되면 다음 퀘스트 액션으로 진행함.
    /// </summary>
    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (IsWaitCompleted())
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    /// <summary>
    /// 리스너나 코루틴을 만들지 않으므로 종료 시 경과 시간만 초기화함.
    /// </summary>
    public override void Exit()
    {
        elapsedTime = 0f;
    }

    /// <summary>
    /// 오브젝트 모드는 활성 상태를, 시간 모드는 누적된 deltaTime을 기준으로 완료 여부를 반환함.
    /// </summary>
    private bool IsWaitCompleted()
    {
        // 1. 활성화 모드는 부모까지 실제로 켜진 상태인지 확인함.
        if (waitMode == InteractiveSequenceTimerMode.ObjectActivation)
        {
            return waitObject != null && waitObject.activeInHierarchy;
        }

        // 2. 시간 모드는 별도 코루틴 없이 Pipeline의 매 프레임 Execute 호출에서 시간을 누적함.
        elapsedTime += Time.deltaTime;
        return elapsedTime >= Mathf.Max(0f, waitingTime);
    }
}
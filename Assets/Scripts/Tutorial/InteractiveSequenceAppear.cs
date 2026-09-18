using UnityEngine;

/// <summary>
/// 지정한 오브젝트를 Inspector 설정에 따라 활성화하거나 비활성화한 뒤 다음 단계로 진행함.
/// </summary>
public class InteractiveSequenceAppear : InteractiveSequenceBase
{
    [Header("활성 상태 변경")]
    [SerializeField] private GameObject appearObject;
    [SerializeField] private bool doesItMakeObjectAppear;

    private bool isCompleted;

    /// <summary>
    /// 대상 오브젝트의 활성 상태를 적용하고 정상 적용 여부를 완료 상태로 기록함.
    /// </summary>
    public override void Enter()
    {
        // 1. 필수 대상이 없으면 상태를 변경하지 않고 현재 단계에서 보류.
        isCompleted = false;
        if (appearObject == null)
        {
            Debug.LogError($"[InteractiveSequenceAppear] {gameObject.name}의 appearObject가 설정되지 않았습니다.");
            return;
        }

        // 2. 체크 여부를 그대로 SetActive에 전달함.
        appearObject.SetActive(doesItMakeObjectAppear);
        isCompleted = true;
    }

    /// <summary>
    /// 활성 상태 변경이 끝나면 다음 튜토리얼 시퀀스로 진행함.
    /// </summary>
    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (isCompleted)
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    /// <summary>
    /// 활성 상태 변경이 끝나면 다음 퀘스트 액션으로 진행함.
    /// </summary>
    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (isCompleted)
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    /// <summary>
    /// 재활성화 시 이전 완료 상태로 즉시 넘어가지 않도록 상태를 초기화함.
    /// </summary>
    public override void Exit()
    {
        isCompleted = false;
    }
}
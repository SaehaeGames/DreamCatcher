using UnityEngine;

/// <summary>
/// Fade 컴포넌트의 비동기 연출이 실제로 끝날 때까지 기다린 뒤 다음 단계로 진행함.
/// </summary>
public class InteractiveSequenceFadeEffect : InteractiveSequenceBase
{
    [Header("페이드 대상")]
    [SerializeField] private Fade fadeEffect;

    private bool isCompleted;

    /// <summary>
    /// 완료 상태를 초기화하고 페이드 오브젝트를 켠 뒤 페이드 인을 시작함.
    /// </summary>
    public override void Enter()
    {
        // 1. 재실행 시 이전 콜백의 완료 상태가 남지 않도록 초기화함.
        isCompleted = false;

        // 2. 필수 Fade 참조가 없으면 로그를 띄움.
        if (fadeEffect == null)
        {
            Debug.LogError($"[InteractiveSequenceFadeEffect] {gameObject.name}의 fadeEffect가 설정되지 않았습니다.");
            return;
        }

        // 3. Fade 오브젝트를 활성화하고 실제 연출 종료 콜백을 등록함.
        fadeEffect.gameObject.SetActive(true);
        fadeEffect.FadeIn(OnAfterFadeEffect);
    }

    /// <summary>
    /// Fade 연출이 끝난 뒤 호출되어 시퀀스 완료 상태를 기록함.
    /// </summary>
    private void OnAfterFadeEffect()
    {
        isCompleted = true;
    }

    /// <summary>
    /// Fade가 끝나면 다음 튜토리얼 시퀀스로 진행함.
    /// </summary>
    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (TryConsumeCompletion())
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    /// <summary>
    /// Fade가 끝나면 다음 퀘스트 액션으로 진행함.
    /// </summary>
    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (TryConsumeCompletion())
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    /// <summary>
    /// 단계가 중단되거나 바뀔 때 실행 중인 페이드와 완료 상태를 정리함.
    /// </summary>
    public override void Exit()
    {
        if (fadeEffect != null)
        {
            fadeEffect.StopFade();
        }

        isCompleted = false;
    }

    /// <summary>
    /// 완료 상태를 한 번만 소비하여 동일 프레임의 중복 진행 요청을 막음.
    /// </summary>
    private bool TryConsumeCompletion()
    {
        if (!isCompleted)
        {
            return false;
        }

        isCompleted = false;
        return true;
    }
}
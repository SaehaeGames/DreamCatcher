using UnityEngine;

/// <summary>
/// Inspector에 지정한 대사 ID 범위를 ScriptBox로 재생하고 마지막 대사가 끝날 때 다음 단계로 진행함.
/// </summary>
public class InteractiveSequenceDialog : InteractiveSequenceBase
{
    [Header("대사 범위")]
    [SerializeField] private int startId;
    [SerializeField] private int endId;

    private ScriptBox scriptBox;

    /// <summary>
    /// 시작 시 공용 ScriptBox를 준비하고 지정된 대사 범위 재생을 시작함.
    /// </summary>
    public override void Enter()
    {
        // 1. TutorialContext의 캐시를 우선 사용하고 이전 씬 구성은 검색하여 보완함.
        TutorialContext context = TutorialContext.Get(this);
        scriptBox = context != null ? context.ScriptBox : FindObjectOfType<ScriptBox>();

        // 2. 필수 대사창이 없으면 로그로 알림.
        if (scriptBox == null)
        {
            Debug.LogError($"[InteractiveSequenceDialog] {gameObject.name}에서 ScriptBox를 찾을 수 없습니다.");
            return;
        }

        // 3. 대사창을 열고 Inspector에 지정한 처음·마지막 ID를 전달함.
        scriptBox.ScriptBoxOnOff(true);
        scriptBox.SetScriptBox(startId, endId);
    }

    /// <summary>
    /// 전체 대사가 끝나면 다음 튜토리얼 시퀀스로 진행함.
    /// </summary>
    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (HasDialogCompleted())
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    /// <summary>
    /// 전체 대사가 끝나면 다음 퀘스트 액션으로 진행함.
    /// </summary>
    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (HasDialogCompleted())
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    /// <summary>
    /// 종료 시 남은 타이핑 코루틴과 대사 UI를 함께 정리할 수 있도록 대사창을 닫음.
    /// </summary>
    public override void Exit()
    {
        if (scriptBox != null)
        {
            scriptBox.ScriptBoxOnOff(false);
        }
    }

    /// <summary>
    /// ScriptBox가 준비되어 있고 지정 범위의 마지막 대사까지 끝났는지 반환함.
    /// </summary>
    private bool HasDialogCompleted()
    {
        return scriptBox != null && scriptBox.ReturnNextScript();
    }
}
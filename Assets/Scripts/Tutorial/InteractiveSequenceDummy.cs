using UnityEngine;

/// <summary>
/// 파이프라인을 의도적으로 현재 위치에 멈춰 두는 종료·대기용 시퀀스.
/// </summary>
public class InteractiveSequenceDummy : InteractiveSequenceBase
{
    /// <summary>
    /// 시작 시 열려 있는 대사창이 있다면 닫음.
    /// </summary>
    public override void Enter()
    {
        // 1. 공용 Context에서 대사창을 가져오고 이전 씬 구성은 검색으로 지원함.
        TutorialContext context = TutorialContext.Get(this);
        ScriptBox scriptBox = context != null ? context.ScriptBox : FindObjectOfType<ScriptBox>();

        // 2. 대사창이 있으면 현재 타이핑과 UI를 함께 닫음.
        if (scriptBox != null)
        {
            scriptBox.ScriptBoxOnOff(false);
        }
    }

    /// <summary>
    /// 의도적으로 완료 요청을 보내지 않아 튜토리얼 파이프라인을 이 단계에 유지함.
    /// </summary>
    public override void Execute(TutorialPipeline tutorialPipeline)
    {
    }

    /// <summary>
    /// 의도적으로 완료 요청을 보내지 않아 퀘스트 액션 파이프라인을 이 단계에 유지함.
    /// </summary>
    public override void Execute(QuestActionPipeline questActionPipeline)
    {
    }

    /// <summary>
    /// 실행 중 생성하는 임시 상태가 없어 종료 시 별도 정리를 하지 않음.
    /// </summary>
    public override void Exit()
    {
    }
}
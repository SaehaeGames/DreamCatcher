using UnityEngine;

/// <summary>
/// 모든 튜토리얼 동작이 구현해야 하는 공통 실행 규약.
/// Enter → Execute 반복 → Exit 생명주기를 통일하여 튜토리얼과 기존 퀘스트 액션 파이프라인이 같은 연출을 실행할 수 있게 함.
/// </summary>
public abstract class InteractiveSequenceBase : MonoBehaviour
{
    /// <summary>
    /// 시퀀스가 현재 단계로 선택될 때 한 번 호출되어 UI·상태·리스너를 준비함.
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// 튜토리얼 진행 중 매 프레임 호출되어 완료 조건을 확인하고 다음 시퀀스를 요청함.
    /// </summary>
    public abstract void Execute(TutorialPipeline tutorialPipeline);

    /// <summary>
    /// 기존 퀘스트 액션에서 공용 시퀀스를 사용할 때 매 프레임 완료 조건을 확인함.
    /// </summary>
    public abstract void Execute(QuestActionPipeline questActionPipeline);

    /// <summary>
    /// 해당 단계가 완료되거나 중단될 때 호출되어 임시 UI·부모 관계·리스너·실행 상태를 복구함.
    /// </summary>
    public abstract void Exit();
}
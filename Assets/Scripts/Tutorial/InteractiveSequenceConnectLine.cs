using UnityEngine;

/// <summary>
/// 제작 튜토리얼에서 올바른 선 연결 횟수를 모아 다음 단계 진행 여부를 판단함.
/// </summary>
public class InteractiveSequenceConnectLine : InteractiveSequenceBase
{
    private const int RequiredConnectionCount = 4;

    private int numberOfTimesCorrect;

    /// <summary>
    /// 단계가 다시 시작될 때 이전 연결 성공 횟수를 초기화함.
    /// </summary>
    public override void Enter()
    {
        numberOfTimesCorrect = 0;
    }

    /// <summary>
    /// 필요한 연결이 모두 끝나면 다음 튜토리얼 시퀀스로 진행함.
    /// </summary>
    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (HasCompletedAllConnections())
        {
            tutorialPipeline.SetNextTutorial(SceneState.None);
        }
    }

    /// <summary>
    /// 필요한 연결이 모두 끝나면 다음 퀘스트 액션으로 진행함.
    /// </summary>
    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (HasCompletedAllConnections())
        {
            questActionPipeline.SetNextQuestAction();
        }
    }

    /// <summary>
    /// 별도 UI나 리스너를 만들지 않으므로 종료 시 추가 정리가 필요하지 않음.
    /// </summary>
    public override void Exit()
    {
    }

    /// <summary>
    /// InteractiveLimitDragPoint가 유효한 연결을 확인할 때 성공 횟수를 증가시킴.
    /// </summary>
    public void PlusNumberOfTimesCorrect()
    {
        numberOfTimesCorrect++;
    }

    /// <summary>
    /// 필요한 연결 수를 채웠는지 반환함.
    /// </summary>
    private bool HasCompletedAllConnections()
    {
        return numberOfTimesCorrect >= RequiredConnectionCount;
    }
}
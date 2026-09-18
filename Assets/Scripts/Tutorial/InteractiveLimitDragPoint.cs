using System.Collections;
using UnityEngine;

/// <summary>
/// 제작 튜토리얼의 시작점마다 허용된 도착점을 제한하고 올바른 연결 횟수를 시퀀스에 전달함.
/// </summary>
public class InteractiveLimitDragPoint : MonoBehaviour
{
    [Header("허용 도착점")]
    [SerializeField] private int targetEndPoint1;
    [SerializeField] private int targetEndPoint2;

    private InteractiveSequenceConnectLine tutorialConnectLine;

    /// <summary>
    /// 현재 활성화된 튜토리얼 연결선?을 찾고 해당 단계가 아닐 때는 판정 컴포넌트를 종료함.
    /// </summary>
    private IEnumerator Start()
    {
        // 1. TutorialManager.Start가 저장된 Scene 자식을 활성화할 때까지 한 프레임 기다림.
        yield return null;

        // 2. 활성 상태인 튜토리얼 연결선 시퀀스를 찾아 현재 제작 단계가 튜토리얼 대상인지 확인함.
        tutorialConnectLine = FindFirstObjectByType<InteractiveSequenceConnectLine>();

        // 3. 튜토리얼 연결선이 없는 일반 제작에서는 제한 판정을 실행하지 않음.
        if (tutorialConnectLine == null)
        {
            enabled = false;
        }
    }

    /// <summary>
    /// 도착점 번호가 허용 범위인지 검사하고 성공한 연결을 현재 시퀀스에 한 번 전달함.
    /// </summary>
    public bool ReceiveEndPointNum(int endPointNum)
    {
        // 1. 전달받은 도착점이 이 시작점에 허용된 두 값 중 하나인지 확인함.
        if (endPointNum != targetEndPoint1 && endPointNum != targetEndPoint2)
        {
            Debug.Log($"[InteractiveLimitDragPoint] {gameObject.name}: 허용되지 않은 도착점 {endPointNum}");
            return false;
        }

        // 2. 씬 구성 순서로 참조를 놓쳤다면 성공 처리 전에 한 번 더 찾음.
        if (tutorialConnectLine == null)
        {
            tutorialConnectLine = FindFirstObjectByType<InteractiveSequenceConnectLine>();
        }

        if (tutorialConnectLine == null)
        {
            Debug.LogError("[InteractiveLimitDragPoint] 연결선 튜토리얼 시퀀스를 찾을 수 없습니다.");
            return false;
        }

        // 3. 유효한 연결 횟수를 시퀀스에 전달함.
        tutorialConnectLine.PlusNumberOfTimesCorrect();
        return true;
    }
}

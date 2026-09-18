using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 튜토리얼 전용 Unity 씬으로 바로 이동하기 위한 개발·테스트용 버튼 연결 스크립트입니다.
/// </summary>
public class GoTutorialTestScript : MonoBehaviour
{
    private const string TutorialSceneName = "Tutorial";

    /// <summary>
    /// 테스트 버튼 클릭 시 튜토리얼 Unity 씬을 직접 로드합니다.
    /// </summary>
    public void GoTutorial()
    {
        SceneManager.LoadScene(TutorialSceneName);
    }
}
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 일반 Button의 클릭을 튜토리얼 파이프라인이 확인할 수 있는 완료 상태로 변환함.
/// 기존 Button 리스너는 유지하고 튜토리얼용 클릭 여부만 별도로 기록함.
/// </summary>
[RequireComponent(typeof(Button))]
public class InteractiveButton : MonoBehaviour
{
    private Button targetButton;
    private bool buttonClicked;

    /// <summary>
    /// Button을 캐시하고 튜토리얼 클릭 리스너를 한 번 등록함.
    /// </summary>
    private void Awake()
    {
        // 1. 같은 오브젝트의 Button을 보관함.
        targetButton = GetComponent<Button>();

        // 2. 이전 클릭 상태와 중복 리스너를 지운 뒤 현재 콜백을 등록함.
        buttonClicked = false;
        targetButton.onClick.RemoveListener(TutorialButtonClicked);
        targetButton.onClick.AddListener(TutorialButtonClicked);
    }

    /// <summary>
    /// 오브젝트 파괴 시 직접 등록한 리스너만 제거함.
    /// </summary>
    private void OnDestroy()
    {
        if (targetButton != null)
        {
            targetButton.onClick.RemoveListener(TutorialButtonClicked);
        }
    }

    /// <summary>
    /// 현재 튜토리얼 클릭이 완료되었음을 기록함.
    /// </summary>
    public void TutorialButtonClicked()
    {
        buttonClicked = true;
    }

    /// <summary>
    /// 현재 단계에서 버튼이 클릭되었는지 반환함.
    /// </summary>
    public bool GetButtonClicked()
    {
        return buttonClicked;
    }

    /// <summary>
    /// 시퀀스 시작·종료 시 클릭 완료 상태를 설정함.
    /// </summary>
    public void SetButtonClicked(bool isClicked)
    {
        buttonClicked = isClicked;
    }
}
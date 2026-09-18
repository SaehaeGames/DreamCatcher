using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 검은 UI 이미지의 알파 값을 바꾸어 튜토리얼 화면의 페이드 인·아웃을 담당함.
/// 튜토리얼 단계는 페이드 종료 콜백을 기준으로 다음 단계로 넘어가므로 연출 완료 시점도 전달함.
/// </summary>
public class Fade : MonoBehaviour
{
    [Header("페이드 설정")]
    [SerializeField, Range(0.01f, 10f)] private float fadeTime;
    [SerializeField] private AnimationCurve fadeCurve;

    private Image fadeImage;
    private Coroutine fadeCoroutine;

    /// <summary>
    /// 페이드 대상 Image를 한 번만 찾아 이후 연출에서 재사용함.
    /// </summary>
    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        if (fadeImage == null)
        {
            Debug.LogError($"[Fade] {gameObject.name}에 Image가 없습니다.");
        }
    }

    /// <summary>
    /// 화면을 가린 상태에서 투명한 상태로 전환하고 완료 후 후속 동작을 실행함.
    /// </summary>
    public void FadeIn(UnityAction action)
    {
        StartFade(action, 1f, 0f);
    }

    /// <summary>
    /// 투명한 상태에서 화면을 가리는 상태로 전환하고 완료 후 후속 동작을 실행함.
    /// </summary>
    public void FadeOut(UnityAction action)
    {
        StartFade(action, 0f, 1f);
    }

    /// <summary>
    /// 진행 중인 페이드 코루틴을 중단하여 중복 연출과 뒤늦은 완료 콜백을 막음.
    /// </summary>
    public void StopFade()
    {
        if (fadeCoroutine == null)
        {
            return;
        }

        StopCoroutine(fadeCoroutine);
        fadeCoroutine = null;
    }

    /// <summary>
    /// 오브젝트가 꺼질 때 남아 있는 코루틴을 정리함.
    /// </summary>
    private void OnDisable()
    {
        StopFade();
    }

    /// <summary>
    /// 기존 연출을 정리하고 지정한 시작·종료 알파 값으로 새 페이드를 시작함.
    /// </summary>
    private void StartFade(UnityAction action, float startAlpha, float endAlpha)
    {
        // 1. 같은 Image를 변경 중인 이전 연출과 완료 콜백을 중단함.
        StopFade();

        // 2. 필수 참조가 있을 때만 새 페이드 코루틴을 시작함.
        if (fadeImage == null)
        {
            return;
        }

        fadeCoroutine = StartCoroutine(FadeRoutine(action, startAlpha, endAlpha));
    }

    /// <summary>
    /// AnimationCurve로 알파 값을 매 프레임 적용하고 최종값과 완료 콜백을 확정함.
    /// </summary>
    private IEnumerator FadeRoutine(UnityAction action, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        float duration = Mathf.Max(fadeTime, 0.01f);

        // 1. 경과 시간 비율을 곡선에 적용하여 매 프레임 알파 값을 갱신함.
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            float curvedProgress = fadeCurve != null ? fadeCurve.Evaluate(progress) : progress;
            SetAlpha(Mathf.Lerp(startAlpha, endAlpha, curvedProgress));
            yield return null;
        }

        // 2. 프레임 간격으로 생길 수 있는 오차를 없애기 위해 마지막 알파 값을 확정함.
        SetAlpha(endAlpha);
        fadeCoroutine = null;

        // 3. 시각 연출이 끝난 뒤에만 다음 튜토리얼 동작을 호출함.
        action?.Invoke();
    }

    /// <summary>
    /// 현재 Image 색상에서 알파 값만 교체함.
    /// </summary>
    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}

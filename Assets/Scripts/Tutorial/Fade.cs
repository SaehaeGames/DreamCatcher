using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 10f)]
    private float fadeTime;     // 페이드 되는 시간
    [SerializeField]
    private AnimationCurve fadeCurve;       // 페이드 효과가 적용되는 알파 값을 곡선의 값으로 설정
    private Image fadeImage;        // 페이드 효과에 사용되는 검은 바탕 이미지
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public void FadeIn(UnityAction action)
    {
        StartFade(action, 1, 0);
    }

    public void FadeOut(UnityAction action)
    {
        StartFade(action, 0, 1);
    }

    public void StopFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private void OnDisable()
    {
        StopFade();
    }

    private void StartFade(UnityAction action, float start, float end)
    {
        StopFade();
        fadeCoroutine = StartCoroutine(FadeFun(action, start, end));
    }

    private IEnumerator FadeFun(UnityAction action, float start, float end)
    {
        float current = 0.0f;
        float percent = 0.0f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / Mathf.Max(fadeTime, 0.01f);

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(start, end, fadeCurve.Evaluate(percent));
            fadeImage.color = color;

            //Debug.Log(fadeImage.color);
            yield return null;
        }

        Color completedColor = fadeImage.color;
        completedColor.a = end;
        fadeImage.color = completedColor;

        fadeCoroutine = null;
        action?.Invoke();
    }
}

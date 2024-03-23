using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    //페이드 효과를 수행하는 클래스

    public Image fadeImage; //페이드 효과에 사용할 검은색 이미지

    public void PlayFadeIn()
    {
        //페이드인 효과 함수(어둡다가 밝아짐)

        StartCoroutine(FadeInCoroutine());
    }

    public void PlayFadeOut()
    {
        //페이드아웃 효과 함수(밝다가 어두워짐)

        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeInCoroutine()
    {
        //페이드인 코루틴

        float fadeCount = 1.0f;    //처음 알파값
        fadeImage.color = new Color(0, 0, 0, fadeCount);    //알파값 초기화

        Color fadeColor = fadeImage.color;  //페이드 이미지 컬러값

        while (fadeCount > 0f) //알파값이 0이 될 때까지 반복(어두워질 때까지)
        {
            fadeCount -= 0.025f;
            fadeColor.a = fadeCount;
            fadeImage.color = fadeColor;    //페이드 이미지에 바뀐 알파값 적용
            yield return new WaitForSeconds(0.01f); //적용 딜레이
            /*            fadeImage.color = new Color(0, 0, 0, fadeCount);    //해당 알파값으로 설정*/
        }
    }

    IEnumerator FadeOutCoroutine()
    {
        //페이드아웃 코루틴
        float fadeCount = 0;    //처음 알파값
        fadeImage.color = new Color(0, 0, 0, fadeCount);    //알파값 초기화

        Color fadeColor = fadeImage.color;  //페이드 이미지 컬러값

        while (fadeCount < 1.0f) //알파값이 1.0이 될 때까지 반복(어두워질 때까지)
        {
            fadeCount += 0.01f;
            fadeColor.a = fadeCount;
            fadeImage.color = fadeColor;    //페이드 이미지에 바뀐 알파값 적용
            yield return new WaitForSeconds(0.01f); //적용 딜레이
/*            fadeImage.color = new Color(0, 0, 0, fadeCount);    //해당 알파값으로 설정*/
        }
    }
}

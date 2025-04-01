using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioHandler : MonoBehaviour
{
    public RectTransform backgroundImage;  // 남는 공간을 채울 배경 이미지
    public Text testText;
    private float targetRatio = 2560f / 1440f;

    void Start()
    {
        StartCoroutine(WaitForScreenSetup());  // 화면 설정이 완료될 때까지 대기
    }

    IEnumerator WaitForScreenSetup()
    {
        yield return new WaitForSeconds(0.1f); // 잠시 대기 후 실행

        AdjustBackground();
    }

    void AdjustBackground()
    {
        float screenRatio = (float)Screen.width / (Screen.height);

        if (screenRatio < targetRatio)
        {
            // 세로로 긴 화면: 가로를 맞추고 위아래를 배경으로 채움
            testText.text += "세로로 긴 화면\n 넓이 : "+ Screen.width+", 높이 : "+ (Screen.height);
            float scaleFactor = targetRatio / screenRatio;
            backgroundImage.localScale = new Vector3(1, scaleFactor, 1);
            this.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
            testText.text += this.GetComponent<CanvasScaler>().matchWidthOrHeight+'\n';
        }
        else
        {
            testText.text += "가로로 긴 화면\n 넓이 : " + Screen.width + ", 높이 : " + (Screen.height);
            // 가로로 긴 화면: 세로를 맞추고 좌우를 배경으로 채움
            float scaleFactor = screenRatio / targetRatio;
            backgroundImage.localScale = new Vector3(scaleFactor, 1, 1);
            this.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f; // TODO : 나중에 1.0f로 변경
            testText.text += this.GetComponent<CanvasScaler>().matchWidthOrHeight + '\n';
        }
        this.GetComponent<CanvasScaler>().enabled = false;
        this.GetComponent<CanvasScaler>().enabled = true;
        // **즉시 반영**
        Canvas.ForceUpdateCanvases();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioHandler : MonoBehaviour
{
    public RectTransform backgroundImage;  // ���� ������ ä�� ��� �̹���
    public Text testText;
    private float targetRatio = 2560f / 1440f;

    void Start()
    {
        StartCoroutine(WaitForScreenSetup());  // ȭ�� ������ �Ϸ�� ������ ���
    }

    IEnumerator WaitForScreenSetup()
    {
        yield return new WaitForSeconds(0.1f); // ��� ��� �� ����

        AdjustBackground();
    }

    void AdjustBackground()
    {
        float screenRatio = (float)Screen.width / (Screen.height);

        if (screenRatio < targetRatio)
        {
            // ���η� �� ȭ��: ���θ� ���߰� ���Ʒ��� ������� ä��
            testText.text += "���η� �� ȭ��\n ���� : "+ Screen.width+", ���� : "+ (Screen.height);
            float scaleFactor = targetRatio / screenRatio;
            backgroundImage.localScale = new Vector3(1, scaleFactor, 1);
            this.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
            testText.text += this.GetComponent<CanvasScaler>().matchWidthOrHeight+'\n';
        }
        else
        {
            testText.text += "���η� �� ȭ��\n ���� : " + Screen.width + ", ���� : " + (Screen.height);
            // ���η� �� ȭ��: ���θ� ���߰� �¿츦 ������� ä��
            float scaleFactor = screenRatio / targetRatio;
            backgroundImage.localScale = new Vector3(scaleFactor, 1, 1);
            this.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f; // TODO : ���߿� 1.0f�� ����
            testText.text += this.GetComponent<CanvasScaler>().matchWidthOrHeight + '\n';
        }
        this.GetComponent<CanvasScaler>().enabled = false;
        this.GetComponent<CanvasScaler>().enabled = true;
        // **��� �ݿ�**
        Canvas.ForceUpdateCanvases();
    }
}

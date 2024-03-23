using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollUpdate : MonoBehaviour
{
    // 스크롤뷰를 닫았다 열면 맨 위로 이동하는 클래스

    public RectTransform ScrollContent;

    public void OnEnable()
    {
        SetRectPosition();
    }

    public void SetRectPosition()
    {
        float x = ScrollContent.anchoredPosition.x;
        ScrollContent.anchoredPosition = new Vector3(x, 0, 0);
    }
}


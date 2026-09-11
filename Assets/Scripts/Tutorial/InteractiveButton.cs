using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveButton : MonoBehaviour
{
    public bool buttonClicked;

    private void Awake()
    {
        buttonClicked = false;
        Button button = GetComponent<Button>();
        button.onClick.RemoveListener(TutorialButtonClicked);
        button.onClick.AddListener(TutorialButtonClicked);
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveListener(TutorialButtonClicked);
        }
    }

    // 튜토리얼 버튼이 눌렸을 때
    public void TutorialButtonClicked()
    {
        buttonClicked = true;
    }

    public bool GetButtonClicked()
    {
        return buttonClicked;
    }

    public void SetButtonClicked(bool _buttonClicked)
    {
        buttonClicked = _buttonClicked;
    }
}

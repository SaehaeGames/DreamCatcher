using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveButton : MonoBehaviour
{
    public bool buttonClicked;

    // Start is called before the first frame update
    void Start()
    {
        buttonClicked = false;
        this.GetComponent<Button>().onClick.AddListener(TutorialButtonClicked);
    }

    // Ʃ�丮�� ��ư�� ������ ��
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

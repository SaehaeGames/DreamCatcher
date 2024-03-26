using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    public bool buttonClicked;

    // Start is called before the first frame update
    void Start()
    {
        buttonClicked = false;
        this.GetComponent<Button>().onClick.AddListener(TutorialButtonClicked);
    }

    // 튜토리얼 버튼이 눌렸을 때
    public void TutorialButtonClicked()
    {
        buttonClicked = true;
    }
}

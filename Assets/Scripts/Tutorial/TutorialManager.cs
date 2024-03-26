using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject blockBG;
    public GameObject arrow;


    // 튜토리얼 클릭 위치 배열

    // Start is called before the first frame update
    void Start()
    {
        blockBG.GetComponent<Animator>().enabled = false;
        arrow.GetComponent<Animator>().enabled = false;
        //this.transform.GetChild(1).gameObject.GetComponent<Animation>().Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 화면 강조 & 화살표 깜박임
    public void HighlightUI()
    {
        arrow.GetComponent<Animator>().enabled = true;
        arrow.GetComponent<Animator>().Play("blinkArrow");
    }

    // 화살표 이동
    public void MoveArrow()
    {
        arrow.GetComponent<Animator>().enabled = true;
        arrow.GetComponent<Animator>().Play("arrowMove");
    }

    // 페이드 인
    public void FadeIn()
    {
        blockBG.GetComponent<Animator>().enabled = true;
        blockBG.GetComponent<Animator>().Play("FadeIn");
    }

    // 페이드 아웃
    public void FadeOut()
    {
        blockBG.GetComponent<Animator>().enabled = true;
        blockBG.GetComponent<Animator>().Play("FadeOut");
    }

    // 대기(30초)
    // 
}

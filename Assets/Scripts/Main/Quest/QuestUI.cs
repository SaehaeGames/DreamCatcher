using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    //퀘스트 배경 이미지를 바꾸는 스크립트

    [Header("[Quest UI]")]
    public GameObject wallPaper;    //배경 이미지 오브젝트
    public GameObject contentTexts;       //텍스트 오브젝트
    public GameObject questPanel;   //퀘스트 패널

    [Space]
    [Header("[Quest Resource")]
    public Sprite[] wallPapers;  //배경 이미지들(0: 흰식, 1: 노란색, 2: 검정색)

    public void OpenWhiteWallPaper()
    {
        //퀘스트 창을 흰색 편지지, 검정 글씨로 바꾸고 퀘스트 패널을 여는 함수

        wallPaper.GetComponent<Image>().sprite = wallPapers[0]; //배경 이미지를 바꾼다

        int textCnt = contentTexts.transform.childCount;
        for (int i = 0; i < textCnt; i++)
        {
            contentTexts.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(0, 0, 0);    //글씨 색을 바꾼다
        }

        questPanel.SetActive(true); //퀘스트창 열기
        this.GetComponent<QuestNotice>().CloseMainQuestNotice();    //메인퀘스트 알림 끄기

    }

    public void OpenYellowWallPaper()
    {
        //퀘스트 창을 노란색 편지지, 검정 글씨로 바꾸고 퀘스트 패널을 여는 함수

        wallPaper.GetComponent<Image>().sprite = wallPapers[1]; //배경 이미지를 바꾼다

        int textCnt = contentTexts.transform.childCount;
        for (int i = 0; i < textCnt; i++)
        {
            contentTexts.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(0, 0, 0);    //글씨 색을 바꾼다
        }

        questPanel.SetActive(true); //퀘스트창 열기
        this.GetComponent<QuestNotice>().CloseRepeatQuestNotice();    //반복퀘스트 알림 끄기
    }

    public void OpenBlackWallPaper()
    {
        //퀘스트 창을 검정색 편지지, 흰색 글씨로 바꾸고 퀘스트 패널을 여는 함수

        wallPaper.GetComponent<Image>().sprite = wallPapers[2]; //배경 이미지를 바꾼다

        int textCnt = contentTexts.transform.childCount;
        for (int i = 0; i < textCnt; i++)
        {
            contentTexts.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(255, 255, 255);    //글씨 색을 바꾼다
        }

        questPanel.SetActive(true); //퀘스트창 열기
    }
}

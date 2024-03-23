using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNotice : MonoBehaviour
{
    //퀘스트 알림 표시를 띄우고 끄는 스크립트
    [Header("[Quest Notice]")]
    public GameObject[] noticeObjects;    //알림 오브젝트들
    public bool[] checkedNotice;  //알림 확인 변수(true: 확인함, false: 확인 안 함)

    private void Start()
    {
        checkedNotice = new bool[3] { false, false, false };    //퀘스트 알림 변수 초기화(나중에는 저장해야할듯,,? 읽은지 안읽었는지. 퀘스트 id, 퀘스트 읽음 확인 세트로.)
       
        //나중에 퀘스트 별로 데이터 저장해야하나?? 퀘스트 받은 날, 읽었는지 여부, 퀘스트 내용(무슨 실, 아이템인지) 데이터 저장해서 불러와야할듯)

        //테스트용. 게임이 시작하면 반복퀘스트 알림 활성화
        bool isReadRepeatNotification = System.Convert.ToBoolean(PlayerPrefs.GetInt("RepearFirstLetter", 0)); //저장된 퀘스트 확인 여부
        if (!isReadRepeatNotification)  //확인 안 했으면
        {
            NoticeRepeatQuest();
        }
    }

    public void NoticeMainQuest()
    {
        //메인 퀘스트 알림을 활성화하는 함수

        noticeObjects[0].SetActive(true);   //알림 오브젝트 활성화
        checkedNotice[0] = false;  //퀘스트 알림 확인 변수를 확인 안 함으로 설정
    }

    public void NoticeRepeatQuest()
    {
        //반복 퀘스트 알림을 활성화하는 함수

        noticeObjects[1].SetActive(true);   //알림 오브젝트 활성화
        checkedNotice[1] = false;  //퀘스트 알림 확인 변수를 확인 안 함으로 설정

        PlayerPrefs.SetInt("RepeatNotification", System.Convert.ToInt16(checkedNotice[1])); //반복 퀘스트 알림 확인 여부 저장
    }

    public void CloseMainQuestNotice()
    {
        //메인 퀘스트 알림을 비활성화하는 함수

        noticeObjects[0].SetActive(false);   //알림 오브젝트 비활성화
        checkedNotice[0] = true;  //퀘스트 알림 확인 변수를 확인함으로 설정
    }

    public void CloseRepeatQuestNotice()
    {
        //메인 퀘스트 알림을 비활성화하는 함수

        noticeObjects[1].SetActive(false);   //알림 오브젝트 비활성화
        checkedNotice[1] = true;  //퀘스트 알림 확인 변수를 확인함으로 설정

        PlayerPrefs.SetInt("RepeatNotification", System.Convert.ToInt16(checkedNotice[1])); //반복 퀘스트 알림 확인 여부 저장
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIController : MonoBehaviour
{
    //퀘스트 배경 이미지를 바꾸는 스크립트
    [Header("[Letter UI]")]
    public GameObject letters;

    [Header("[Quest UI]")]
    public GameObject wallPaper;    //배경 이미지 오브젝트
    public GameObject contentTexts;       //텍스트 오브젝트
    public GameObject questPanel;   //퀘스트 패널
    public GameObject deliveryView;     //납품 오브젝트
    public GameObject deliveryButton;
    public GameObject charonItemButton;

    [Space]
    [Header("[Quest Resource")]
    public Sprite[] wallPapers;  //배경 이미지들(0: 흰식, 1: 노란색, 2: 검정색)
    public Sprite potion;
    public Sprite paperweight;
    private Color deactiveItemColor = new Color(0, 0, 0, 157);
    private Color activeItemColor = new Color(225, 225, 225, 225);

    [Space]
    [Header("[Managers]")]
    public GameObject managers;
    private PlayerDataManager playerDataManager;

    private QuestType questType;

    public void OpenWhiteWallPaper()
    {
        questType = QuestType.MainQuest;

        //퀘스트 창을 흰색 편지지, 검정 글씨로 바꾸고 퀘스트 패널을 여는 함수
        contentTexts.SetActive(true);   //메시지 오브젝트 활성화
        deliveryView.SetActive(false);  //납품 오브젝트 비활성화

        wallPaper.GetComponent<Image>().sprite = wallPapers[0]; //배경 이미지를 바꾼다

        int textCnt = contentTexts.transform.childCount;
        for (int i = 0; i < textCnt; i++)
        {
            contentTexts.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(0, 0, 0);    //글씨 색을 바꾼다
        }

        questPanel.SetActive(true); //퀘스트창 열기
        letters.GetComponent<QuestNotice>().CloseNotice(QuestType.MainQuest);    //카론퀘스트 알림 끄기
        UpdateQuestContents();

        QuestFlowState questFlowState = managers.GetComponent<QuestManager>().GetCurrentQuestFlowState();

        if (questFlowState == QuestFlowState.DeliveryCompleted
            || questFlowState == QuestFlowState.BeforeStart)
        {
            deliveryButton.SetActive(false);
        }
        else
        {
            deliveryButton.SetActive(true);
        }

        playerDataManager = GameManager.instance.playerDataManager;
        if (playerDataManager.GetCurrentMainQuestIndex() == 7)
        {
            charonItemButton.SetActive(true);
            charonItemButton.GetComponent<Image>().sprite = potion;
            if (playerDataManager.HasReceiveCharonItem(CharonItem.Potion))
            {
                charonItemButton.GetComponent<Image>().color = activeItemColor;
            }
            else
            {
                charonItemButton.GetComponent<Image>().color = deactiveItemColor;
            }
        }
        else if(playerDataManager.GetCurrentMainQuestIndex() == 15)
        {
            charonItemButton.SetActive(true);
            charonItemButton.GetComponent<Image>().sprite = paperweight;
            if (playerDataManager.HasReceiveCharonItem(CharonItem.Paperweight))
            {
                charonItemButton.GetComponent<Image>().color = activeItemColor;
            }
            else
            {
                charonItemButton.GetComponent<Image>().color = deactiveItemColor;
            }
        }
        else
        {
            charonItemButton.SetActive(false);
        }   
    }

    public void OpenYellowWallPaper()
    {
        questType = QuestType.RepeatQuest;
        //퀘스트 창을 노란색 편지지, 검정 글씨로 바꾸고 퀘스트 패널을 여는 함수

        contentTexts.SetActive(false);   //메시지 오브젝트 비활성화
        deliveryView.SetActive(true);  //납품 오브젝트 활성화
        deliveryButton.SetActive(true);
        charonItemButton.SetActive(false);

        wallPaper.GetComponent<Image>().sprite = wallPapers[1]; //배경 이미지를 바꾼다

        int textCnt = contentTexts.transform.childCount;
        for (int i = 0; i < textCnt; i++)
        {
            contentTexts.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(0, 0, 0);    //글씨 색을 바꾼다
        }

        questPanel.SetActive(true); //퀘스트창 열기
        letters.GetComponent<QuestNotice>().CloseNotice(QuestType.RepeatQuest);    //카론퀘스트 알림 끄기
    }

    public void OpenBlackWallPaper()
    {
        //퀘스트 창을 검정색 편지지, 흰색 글씨로 바꾸고 퀘스트 패널을 여는 함수
        questType = QuestType.Charon;

        contentTexts.SetActive(true);   //메시지 오브젝트 활성화
        deliveryView.SetActive(false);  //납품 오브젝트 비활성화
        charonItemButton.SetActive(false);

        wallPaper.GetComponent<Image>().sprite = wallPapers[2]; //배경 이미지를 바꾼다

        int textCnt = contentTexts.transform.childCount;
        for (int i = 0; i < textCnt; i++)
        {
            contentTexts.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(255, 255, 255);    //글씨 색을 바꾼다
        }

        questPanel.SetActive(true); //퀘스트창 열기

        UpdateQuestContents();

        deliveryButton.SetActive(false); //납품 버튼 비활성화
        letters.GetComponent<QuestNotice>().CloseNotice(QuestType.Charon);    //카론퀘스트 알림 끄기
    }

    public void UpdateQuestContents()
    {
        if (questType == QuestType.MainQuest)
        {
            // 제목
            contentTexts.transform.GetChild(0).gameObject.GetComponent<Text>().text
                = managers.GetComponent<QuestManager>().GetCurrentMainQuestTitle();

            // 내용
            contentTexts.transform.GetChild(1).gameObject.GetComponent<Text>().text
                = managers.GetComponent<QuestManager>().GetCurrentMainQuestContents();

            // 보낸 사람
            contentTexts.transform.GetChild(2).gameObject.GetComponent<Text>().text
                = managers.GetComponent<QuestManager>().GetCurrentMainQuestFrom();
        }
        else if (questType == QuestType.RepeatQuest)
        {
            
        }
        else if (questType == QuestType.Charon)
        {
            // 제목
            contentTexts.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";

            // 내용
            contentTexts.transform.GetChild(1).gameObject.GetComponent<Text>().text
                = managers.GetComponent<QuestManager>().GetCurrentCharonLetterContents();

            // 보낸 사람
            contentTexts.transform.GetChild(2).gameObject.GetComponent<Text>().text = "";
        }
        
    }

    public void CloseQuestPaper()
    {
        managers.GetComponent<QuestManager>().PlayCurrentQuestAction(questType);
    }
}

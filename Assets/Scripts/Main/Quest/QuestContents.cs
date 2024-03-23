using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestContents : MonoBehaviour
{
    //퀘스트 내용을 데이터에서 가져와서 설정하는 스크립트
    //json은 왜 한거지..? 엑셀에서는 편지 내용만 가져오고 받은 날짜, 읽은 날짜, 읽은 여부는 json으로 관리할까..

    [Header("[Quest View]")]
    public GameObject contentTexts;       //텍스트 오브젝트
    public GameObject deliveryView;     //납품 오브젝트
    public int curQuestNumber = 0;

    void OnEnable()
    {
        contentTexts.SetActive(true);   //메시지 오브젝트 활성화
        deliveryView.SetActive(false);  //납품 오브젝트 비활성화

        UpdateQuest(curQuestNumber); //테스트로 첫번째 퀘스트 데이터 가져옴

                        //bool isReadFirstLetter = System.Convert.ToBoolean(PlayerPrefs.GetInt("RepearFirstLetter", 0)); //첫번째 반복 퀘스트 확인 여부(상단주의 편지)**이부분 꼭 수정하기
        /*        if (!isReadFirstLetter)     //상단주의 편지를 받지 않았다면
                {
                    contentTexts.SetActive(true);   //메시지 오브젝트 활성화
                    deliveryView.SetActive(false);  //납품 오브젝트 비활성화

                    UpdateQuest(0); //테스트로 첫번째 퀘스트 데이터 가져옴
                    isReadFirstLetter = true;
                    PlayerPrefs.SetInt("RepearFirstLetter", System.Convert.ToInt16(isReadFirstLetter)); //상단주의 편지 읽음 처리
                }
                else //상단주의 편지를 이미 받았다면
                {
                    //반복 퀘스트 받기

                    contentTexts.SetActive(false);   //메시지 오브젝트 비활성화
                    deliveryView.SetActive(true);  //납품 오브젝트 활성화

                    //납품목록 저장(현재 퀘스트 아이템들 배열로 저장하기 또는 퀘스트 아이템 클래스 만들어서 실, 깃털 정보 저장하기)
                }*/
    }


    public void UpdateQuest(int questNumber)
    {
        //호출하는 퀘스트 데이터를 가져오는 함수

        //List<Dictionary<string, object>> data_Quest = CSVParser.ReadFromFile("Quest");  //퀘스트 데이터를 가져옴  
        QuestInfo_Data questInfo_data = GameManager.instance.questinfo_data;    //** 스크립터블 오브젝트 말고 저장한 json에서 가져와도 될 듯..
        QuestContainer questData = GameManager.instance.loadQuestData;

        int isCheck = questData.questList[questNumber].questCheck;
        if (isCheck != 1)   //아직 확인 안 한 퀘스트라면
        {
            contentTexts.transform.GetChild(0).gameObject.GetComponent<Text>().text = questInfo_data.datalist[questNumber].title.ToString();    //퀘스트 이름 변경
            string contents = questInfo_data.datalist[questNumber].contents.ToString(); //퀘스트 내용을 가져옴
            contentTexts.transform.GetChild(1).gameObject.GetComponent<Text>().text = contents.Replace("nn", "\n"); //퀘스트 내용 변경
            contentTexts.transform.GetChild(2).gameObject.GetComponent<Text>().text = questInfo_data.datalist[questNumber].from.ToString();    //퀘스트 발신인 변경

            questData.questList[questNumber].questCheck = 1;
            GameManager.instance.loadQuestData = questData;
        }
        else
        {
            contentTexts.SetActive(false);   //메시지 오브젝트 비활성화
            deliveryView.SetActive(true);  //납품 오브젝트 활성화
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private PlayerDataContainer curPlayerData;   //플레이어 데이터 정보
    private int curScene;
    private int curTutorial;
    [SerializeField] private GameObject tutorialFadePanal;

    private void Awake()
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Start()
    {
        // json 확인 진행된 씬 확인
        //텍스트들을 업데이트 하는 함수

        curPlayerData = GameManager.instance.loadPlayerData;    //플레이어의 상단바 데이터 정보를 가져옴

        curScene = (int)curPlayerData.dataList[7].dataNumber;
        this.transform.GetChild(curScene).gameObject.SetActive(true);
        
        if(curScene==0)
        {
            tutorialFadePanal.SetActive(true);
        }
        else
        {
            tutorialFadePanal.SetActive(false);
        }

        Debug.Log(curScene);

    }

    // 씬 넘버 변경 및 씬 오브젝트 업데이트 함수
    // : 씬 넘버가 변경될 때 실행된다.
    public void ChangeScene()
    {
        // 이전 씬 오브젝트 비활성화
        this.transform.GetChild(curScene).gameObject.SetActive(false);

        // 씬 오브젝트 활성화
        curScene++;
        this.transform.GetChild(curScene).gameObject.SetActive(true);

        // 씬 데이터 업데이트
        curPlayerData.dataList[7].dataNumber = curScene;
        GameManager.instance.GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);

        
    }

    public void ChangeTutorial()
    {

    }
}

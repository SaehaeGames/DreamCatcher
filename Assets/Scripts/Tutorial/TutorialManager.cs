using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private PlayerDataContainer curPlayerData;   //플레이어 데이터 정보
    private int curScene;
    private int curTutorial;
    [SerializeField] private GameObject tutorialFadePanal;

    private ScriptBox scriptBox;

    private void Awake()
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
    }

    void Start()
    {
        // 플레이어 데이터(PlayerDataFile) 로드
        curPlayerData = GameManager.instance.loadPlayerData;

        // 현재 튜토리얼 씬 설정
        curScene = (int)curPlayerData.dataList[7].dataNumber; // 현재 튜토리얼 씬 불러오기

        if(curScene > 11) // 튜토리얼이 아닌 씬부터는 활성화하지 않음
        {
            scriptBox.ScriptBoxOnOff(false);
            if (tutorialFadePanal != null)  tutorialFadePanal.SetActive(false); // 페이더 패널(검은 패널) 비활성화
            return;
        }

        this.transform.GetChild(curScene).gameObject.SetActive(true); // 튜토리얼 씬 활성화
        // 첫 튜토리얼 씬일 경우 페이드 패널 설정
        if (tutorialFadePanal!=null)
        {
            if ((curScene == 0)) // 첫 튜토리얼 씬이 시작일 경우
            {
                tutorialFadePanal.SetActive(true); // 페이더 패널(검은 패널) 활성화
            }
            else // 그 외의 튜토리얼 씬으로 시작하는 경우
            {
                tutorialFadePanal.SetActive(false); // 페이더 패널(검은 패널) 비활성화
            }
        }
    }

    // 씬 넘버 변경 및 씬 오브젝트 업데이트 함수
    // : 씬 넘버가 변경될 때 실행된다.
    public void ChangeScene()
    {
        Debug.Log(curScene);       

        // 마지막 자식인지 확인
        if (this.transform.GetChild(curScene) == this.transform.GetChild(this.transform.childCount - 1))
        {
            // 이전 씬 오브젝트 비활성화
            this.transform.GetChild(curScene).gameObject.SetActive(false);

            // 현재 씬 데이터 증가
            curScene++;
        }
        else
        {

            // 이전 씬 오브젝트 비활성화
            this.transform.GetChild(curScene).gameObject.SetActive(false);

            // 다음 씬 오브젝트 활성화
            curScene++;
            if (this.transform.GetChild(curScene).gameObject != null)
            {
                this.transform.GetChild(curScene).gameObject.SetActive(true);
            }
        }
        
        
        // 씬 데이터 업데이트
        curPlayerData.dataList[7].dataNumber = curScene;
        GameManager.instance.GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);
    }
}

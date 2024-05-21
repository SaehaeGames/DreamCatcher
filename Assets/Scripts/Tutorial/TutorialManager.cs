using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private PlayerDataContainer curPlayerData;   //플레이어 데이터 정보
    private int curScene;

    private void Awake()
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // json 확인 진행된 씬 확인
        //텍스트들을 업데이트 하는 함수

        curPlayerData = GameManager.instance.loadPlayerData;    //플레이어의 상단바 데이터 정보를 가져옴

        curScene = (int)curPlayerData.dataList[7].dataNumber;
        this.transform.GetChild(curScene).gameObject.SetActive(true);
        
        Debug.Log(curScene);

    }

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
}

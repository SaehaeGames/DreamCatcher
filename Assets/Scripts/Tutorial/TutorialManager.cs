using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    private PlayerDataManager playerDataManager;   //플레이어 데이터 정보
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
        playerDataManager = GameManager.instance.playerDataManager;

        // 현재 튜토리얼 씬 설정
        curScene = playerDataManager.GetCurrentScene(); // 현재 튜토리얼 씬 불러오기

        if(curScene > 11) // 튜토리얼이 아닌 씬부터는 활성화하지 않음
        {
            scriptBox.ScriptBoxOnOff(false);
            if (tutorialFadePanal != null)  tutorialFadePanal.SetActive(false); // 페이더 패널(검은 패널) 비활성화
            return;
        }

        //this.transform.GetChild(curScene).gameObject.SetActive(true); // 튜토리얼 씬 활성화
        // 튜토리얼 씬 활성화
        Transform currentObj = transform.Find("Scene " + curScene);
        if (currentObj != null)
        {
            currentObj.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"[TutorialManager] 현재 씬에는 'Scene {curScene}' 오브젝트가 없습니다.");
        }

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
        // 1. 현재 켜져있던 튜토리얼 끄기
        Transform currentObj = transform.Find("Scene " + curScene);
        if (currentObj != null)
        {
            currentObj.gameObject.SetActive(false);
        }

        // 2. 데이터 증가
        curScene++;
        playerDataManager.SetCurrentScene(curScene);

        // 3. 증가된 번호의 다음 튜토리얼이 현재 씬에 있다면 켜기
        Transform nextObj = transform.Find("Scene " + curScene);
        if (nextObj != null)
        {
            nextObj.gameObject.SetActive(true);
        }
        else
        {
            // 다음 번호 오브젝트가 없다는 건, 이 씬에서의 튜토리얼이 끝났거나 다음 씬(도감 등)에서 이어서 해야 한다는 뜻
            Debug.Log($"'Scene {curScene}'이 현재 씬에 없습니다. (현재 씬 튜토리얼 종료)");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class BottomBar : MonoBehaviour
{
    public GameObject[] Selects;
    public GameObject[] UnSelects;

    public enum SceneName
    {
        Main,
        Making,
        CollectionBook,
        Store
    }

    public void Start()
    {
        // 각 카테고리 버튼 이벤트 (씬 이동)
        for (int i = 0; i < UnSelects.Length; i++)
        {
            int index = i;  // 버튼 인덱스 저장
            Button button = UnSelects[i].GetComponent<Button>();    // 버튼 컴포넌트 가져오기

            if (button != null)
            {
                button.onClick.AddListener(() => OnButtonClick((SceneName)index));
            }
        }
    }

    void OnButtonClick(SceneName sceneName)
    {
        // SceneChange 스크립트의 ChangeScene 함수 호출하여 씬 전환

        SceneChange sceneChanger = GetComponent<SceneChange>();
        if (sceneChanger != null)
        {
            sceneChanger.ChangeScene(sceneName.ToString());
        }
    }

    public void ResetCategory()
    {
        for (int i = 0; i < Selects.Length; i++)
        {
            UnSelects[i].gameObject.SetActive(true);
            Selects[i].gameObject.SetActive(false);
        }
    }

    public void SetActiveCategory()
    {
        ResetCategory();    // 모든 카테고리 초기화

        // 현재 씬에 따라 활성 카테고리 설정
        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Main":
                Selects[(int)SceneName.Main].SetActive(true);
                UnSelects[(int)SceneName.Main].SetActive(false);
                break;
            case "Making":
                Selects[(int)SceneName.Making].SetActive(true);
                UnSelects[(int)SceneName.Making].SetActive(false);
                break;
            case "Store":
                Selects[(int)SceneName.Store].SetActive(true);
                UnSelects[(int)SceneName.Store].SetActive(false);
                break;
            default:
                Selects[(int)SceneName.CollectionBook].SetActive(true);
                UnSelects[(int)SceneName.CollectionBook].SetActive(false);
                break;
        }
    }
}

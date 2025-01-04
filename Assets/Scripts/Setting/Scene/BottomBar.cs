using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
<<<<<<< HEAD
using System;
=======
using UnityEngine.UI;
>>>>>>> origin/chaemm

public class BottomBar : MonoBehaviour
{
    public GameObject[] Selects;
    public GameObject[] UnSelects;
    private GameSceneManager _gameSceneManager;
    private void Start()
    {
        _gameSceneManager = GameSceneManager.Instance;
        OnClickSetting();
    }

    public enum SceneName
    {
        Main,
        Making,
        CollectionBook,
        Store
    }

    public void Start()
    {
        // �� ī�װ� ��ư �̺�Ʈ (�� �̵�)
        for (int i = 0; i < UnSelects.Length; i++)
        {
            int index = i;  // ��ư �ε��� ����
            Button button = UnSelects[i].GetComponent<Button>();    // ��ư ������Ʈ ��������

            if (button != null)
            {
                button.onClick.AddListener(() => OnButtonClick((SceneName)index));
            }
        }
    }

    void OnButtonClick(SceneName sceneName)
    {
        // SceneChange ��ũ��Ʈ�� ChangeScene �Լ� ȣ���Ͽ� �� ��ȯ

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
        ResetCategory();    // ��� ī�װ� �ʱ�ȭ

        // ���� ���� ���� Ȱ�� ī�װ� ����
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

    public void OnClickSetting()
    {
        UnSelects[0].GetComponent<Button>().onClick.AddListener(() => _gameSceneManager.ChangeSceneState(SceneState.Main));
        UnSelects[1].GetComponent<Button>().onClick.AddListener(() => _gameSceneManager.ChangeSceneState(SceneState.Making));
        UnSelects[2].GetComponent<Button>().onClick.AddListener(() => _gameSceneManager.ChangeSceneState(SceneState.CollectionDream));
        UnSelects[3].GetComponent<Button>().onClick.AddListener(() => _gameSceneManager.ChangeSceneState(SceneState.Store));
    }

    public void onClickRemove(int menu)
    {
        switch(menu)
        {
            case 0:
                UnSelects[0].GetComponent<Button>().onClick.RemoveListener(() => _gameSceneManager.ChangeSceneState(SceneState.Main));
                Debug.Log("onClickRemove: " + 0);
                break;
            case 1:
                UnSelects[1].GetComponent<Button>().onClick.RemoveListener(() => _gameSceneManager.ChangeSceneState(SceneState.Making));
                Debug.Log("onClickRemove: " + 1);
                break;
            case 2:
                UnSelects[2].GetComponent<Button>().onClick.RemoveListener(() => _gameSceneManager.ChangeSceneState(SceneState.CollectionDream));
                Debug.Log("onClickRemove: " + 2);
                break;
            case 3:
                UnSelects[3].GetComponent<Button>().onClick.RemoveListener(() => _gameSceneManager.ChangeSceneState(SceneState.Store));
                Debug.Log("onClickRemove: " + 3);
                break;
        }
    }

    public void OnClickAdd(int menu)
    {
        switch (menu)
        {
            case 0:
                UnSelects[0].GetComponent<Button>().onClick.AddListener(() => _gameSceneManager.ChangeSceneState(SceneState.Main));
                break;
            case 1:
                UnSelects[1].GetComponent<Button>().onClick.AddListener(() => _gameSceneManager.ChangeSceneState(SceneState.Making));
                break;
            case 2:
                UnSelects[2].GetComponent<Button>().onClick.AddListener(() => _gameSceneManager.ChangeSceneState(SceneState.CollectionDream));
                break;
            case 3:
                UnSelects[3].GetComponent<Button>().onClick.AddListener(() => _gameSceneManager.ChangeSceneState(SceneState.Store));
                break;
        }
    }
}

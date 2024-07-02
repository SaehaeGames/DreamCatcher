using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public void ResetCategory()
    {
        for (int i = 0; i < 4; i++)
        {
            UnSelects[i].gameObject.SetActive(true);
            Selects[i].gameObject.SetActive(false);
        }
    }

    public void SetActiveCategory()
    {
        ResetCategory();
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Main") 
        {
            Selects[0].gameObject.SetActive(true);
            UnSelects[0].gameObject.SetActive(false);
        }
        else if (scene.name == "Making")
        {
            Selects[1].gameObject.SetActive(true);
            UnSelects[1].gameObject.SetActive(false);
        }
        else if (scene.name == "Store")
        {
            Selects[3].gameObject.SetActive(true);
            UnSelects[3].gameObject.SetActive(false);
        }
        else
        {
            Selects[2].gameObject.SetActive(true);
            UnSelects[2].gameObject.SetActive(false);
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

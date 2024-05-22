using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BottomBar : MonoBehaviour
{
    public GameObject[] Selects;
    public GameObject[] UnSelects;

    private void Start()
    {
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
        UnSelects[0].GetComponent<Button>().onClick.AddListener(() => GameSceneManager.Instance.ChangeSceneState(SceneState.Main));
        UnSelects[1].GetComponent<Button>().onClick.AddListener(() => GameSceneManager.Instance.ChangeSceneState(SceneState.Making));
        UnSelects[2].GetComponent<Button>().onClick.AddListener(() => GameSceneManager.Instance.ChangeSceneState(SceneState.CollectionDream));
        UnSelects[3].GetComponent<Button>().onClick.AddListener(() => GameSceneManager.Instance.ChangeSceneState(SceneState.Store));
    }

    public void onClickRemove(int menu)
    {
        UnSelects[menu].GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void OnClickAdd(int menu)
    {

    }
}

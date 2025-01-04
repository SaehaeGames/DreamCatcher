using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum SceneState
{
    None=0,
    Start=1,
    Main=2,
    Making=3,
    CollectionDream=4,
    CollectionBird=5,
    Store=6
}

public partial class GameSceneManager : MonoBehaviour
{
    #region Singleton

    static private GameSceneManager instance;
    public delegate void OnSceneChange(SceneState InState);
    public OnSceneChange onSceneChangedCallback;

    public static GameSceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameSceneManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<GameSceneManager>();
                    singletonObject.name = "GameSceneManager";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }
    #endregion

    public UnityAction<SceneState> SceneChangeWarn;

    private SceneState currentSceneState = SceneState.None;
    private SceneState prevSceneState = SceneState.None;

    public void UpdateSceneState(SceneState nextScene, SceneState nowScene)
    {
        prevSceneState = nowScene;
        currentSceneState = nextScene;
        Debug.Log("pre : " + prevSceneState + " | next : " + currentSceneState);
    }

    public void ChangeSceneState(SceneState inState)
    {
        if (currentSceneState == SceneState.Making)
        {
            SceneChangeWarn.Invoke(inState);
            return;
        }
        
        UpdateSceneState(inState, currentSceneState);
        //prevSceneState = currentSceneState;
        //currentSceneState = inState;
        Debug.Log("SceneChange State : " + currentSceneState.ToString());

        switch (currentSceneState)
        {
            case SceneState.Start:
                SceneManager.LoadScene(SceneState.Start.ToString());
                break;
            case SceneState.Main:
                SceneManager.LoadScene(SceneState.Main.ToString());
                break;
            case SceneState.Making:
                SceneManager.LoadScene(SceneState.Making.ToString());
                break;
            case SceneState.CollectionBird:
                SceneManager.LoadScene(SceneState.CollectionBird.ToString());
                break;
            case SceneState.CollectionDream:
                SceneManager.LoadScene(SceneState.CollectionDream.ToString());
                break;
            case SceneState.Store:
                SceneManager.LoadScene(SceneState.Store.ToString());
                break;
        }
    }

    // 씬 상태를 초기화하는 함수
    public void InitSceneState()
    {
        currentSceneState = SceneState.None;
    }

    private void Start()
    {
        InitSceneState();
    }

}

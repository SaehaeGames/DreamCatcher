using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneState
{
    None,
    Start,
    Main,
    Making,
    CollectionDream,
    CollectionBird,
    Store
}

public class GameSceneManager : MonoBehaviour
{
    static private GameSceneManager instance;

    private void Awake()
    {
        
    }

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

    private SceneState currentSceneState = SceneState.None;

    public void ChangeSceneState(SceneState inState)
    {
        currentSceneState = inState;
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


}

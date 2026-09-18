using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum SceneState
{
    None = 0,
    Start = 1,
    Main = 2,
    Making = 3,
    CollectionDream = 4,
    CollectionBird = 5,
    Store = 6
}

public partial class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager instance;
    public delegate void OnSceneChange(SceneState inState);
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
                    GameObject singletonObject = new GameObject("GameSceneManager");
                    instance = singletonObject.AddComponent<GameSceneManager>();
                }
            }
            return instance;
        }
    }

    public UnityAction<SceneState> SceneChangeWarn;

    private SceneState currentSceneState = SceneState.None;
    private SceneState prevSceneState = SceneState.None;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SyncSceneState(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        if (instance != this) return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        instance = null;
    }

    public void UpdateSceneState(SceneState nextScene, SceneState nowScene)
    {
        prevSceneState = nowScene;
        currentSceneState = nextScene;
        Debug.Log("pre : " + prevSceneState + " | next : " + currentSceneState);
    }

    public void ChangeSceneState(SceneState inState)
    {
        if (inState == SceneState.None) return;

        if (currentSceneState == SceneState.Making && SceneChangeWarn != null)
        {
            SceneChangeWarn.Invoke(inState);
            return;
        }

        UpdateSceneState(inState, currentSceneState);
        string sceneName = GetSceneName(inState);
        if (!string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName);
    }

    public void InitSceneState()
    {
        SyncSceneState(SceneManager.GetActiveScene());
    }

    public static bool TryGetSceneState(string sceneName, out SceneState sceneState)
    {
        switch (sceneName)
        {
            case "Start": sceneState = SceneState.Start; return true;
            case "Main": sceneState = SceneState.Main; return true;
            case "Making": sceneState = SceneState.Making; return true;
            case "CollectionDream": sceneState = SceneState.CollectionDream; return true;
            case "CollectionBook":
            case "CollectionBird": sceneState = SceneState.CollectionBird; return true;
            case "Store": sceneState = SceneState.Store; return true;
            default: sceneState = SceneState.None; return false;
        }
    }

    private static string GetSceneName(SceneState sceneState)
    {
        return sceneState == SceneState.CollectionBird ? "CollectionBook" : sceneState.ToString();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SyncSceneState(scene);
        onSceneChangedCallback?.Invoke(currentSceneState);
    }

    private void SyncSceneState(Scene scene)
    {
        if (!TryGetSceneState(scene.name, out SceneState loadedState)) return;

        if (loadedState != currentSceneState)
        {
            prevSceneState = currentSceneState;
        }
        currentSceneState = loadedState;
    }
}

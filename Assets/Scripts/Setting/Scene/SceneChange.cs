using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    //public UnityAction<string> SceneChangeWarn;

    [SerializeField] private BottomBar bottomBar;
    [SerializeField] private EffectChange effectChange;

    private void Awake()
    {
        if (bottomBar == null) bottomBar = GetComponent<BottomBar>();
        if (effectChange == null)
        {
            GameObject audioObject = GameObject.FindGameObjectWithTag("AudioManager");
            if (audioObject != null) effectChange = audioObject.GetComponent<EffectChange>();
        }
    }

    private void OnEnable()
    {
        // 델리게이트 체인 추가
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 전환 효과

        if (bottomBar != null)
            bottomBar.SetActiveCategory();

        if (effectChange != null)
            effectChange.PlayEffect_OpenScene();    // 씬 전환 효과음
                                                    //this.gameObject.GetComponent<FadeEffect>().PlayFadeIn();   //페이드 효과
    }

    private void OnDisable()
    {
        // 델리게이트 체인 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ChangeScene(string sceneName)
    {
        if (GameSceneManager.TryGetSceneState(sceneName, out SceneState sceneState))
        {
            GameSceneManager.Instance.ChangeSceneState(sceneState);
        }
        else
        {
            Debug.LogError($"[SceneChange] 등록되지 않은 씬 이름입니다: {sceneName}");
        }
    }

    public void ChangeMakingScene()
    {
        GameSceneManager.Instance.ChangeSceneState(SceneState.Making);
    }
   
    public void ChangeGuideScene()
    {
        GameSceneManager.Instance.ChangeSceneState(SceneState.CollectionDream);
    }

    public void ChangeStoreScene()
    {
        GameSceneManager.Instance.ChangeSceneState(SceneState.Store);
    }
}

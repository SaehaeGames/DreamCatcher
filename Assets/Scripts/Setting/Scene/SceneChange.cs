using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    //public UnityAction<string> SceneChangeWarn;

    private BottomBar bottomBar;
    private EffectChange effectChange;

    public void Start()
    {
        // 한 번만 호출
        bottomBar = this.GetComponent<BottomBar>();
        effectChange = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>();
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
        string activeSceneName = SceneManager.GetActiveScene().name;    // 현재 활성중인 씬 이름
        
        if (activeSceneName == "Making")    // 만들기 씬일 경우
        {
            // 다른 일
            //SceneChangeWarn.Invoke(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);  // 씬 로드
        }
    }

    public void ChangeMakingScene()
    {
        if (SceneManager.GetActiveScene().name == "Making")
        {
            //다른 일
            //SceneChangeWarn.Invoke("Making");
        }
        else
        {
            SceneManager.LoadScene("Making");
        }
    }
   
    public void ChangeGuideScene()
    {
        if (SceneManager.GetActiveScene().name == "Making")
        {
            //다른 일
            //SceneChangeWarn.Invoke("CollectionDream");
        }
        else
        {
            SceneManager.LoadScene("CollectionDream");
        }
    }

    public void ChangeStoreScene()
    {
        if (SceneManager.GetActiveScene().name == "Making")
        {
            //다른 일
            //SceneChangeWarn.Invoke("Store");
        }
        else
        {
            SceneManager.LoadScene("Store");
        }
    }
}

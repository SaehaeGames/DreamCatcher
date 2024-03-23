using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public UnityAction<string> SceneChangeWarn;

    private void OnEnable()
    {
        // 델리게이트 체인 추가
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 전환 효과

        this.GetComponent<BottomBar>().SetActiveCategory();
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene();   //씬 전환 효과음
        //this.gameObject.GetComponent<FadeEffect>().PlayFadeIn();   //페이드 효과
    }

    private void OnDisable()
    {
        // 델리게이트 체인 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ChangeStartScene()
    {
        SceneManager.LoadScene("Start");
    }
    public void ChangeMainScene()
    {
        if(SceneManager.GetActiveScene().name=="Making")
        {
            //다른 일
            SceneChangeWarn.Invoke("Main");
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
    }

    public void ChangeMakingScene()
    {
        if (SceneManager.GetActiveScene().name == "Making")
        {
            //다른 일
            SceneChangeWarn.Invoke("Making");
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
            SceneChangeWarn.Invoke("CollectionBook");
        }
        else
        {
            SceneManager.LoadScene("CollectionBook");
        }
    }

    public void ChangeStoreScene()
    {
        if (SceneManager.GetActiveScene().name == "Making")
        {
            //다른 일
            SceneChangeWarn.Invoke("Store");
        }
        else
        {
            SceneManager.LoadScene("Store");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private List<TutorialBase> tutorials;
    [SerializeField]
    private string nextSceneName = "";

    private TutorialBase currentTutorial = null;
    private int currentIndex = -1;
    private GameSceneManager _gameSceneManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameSceneManager = GameSceneManager.Instance;
        tutorials.Clear();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            tutorials.Add(this.transform.GetChild(i).gameObject.GetComponent<TutorialBase>());
        }
        SetNextTutorial(SceneState.None);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTutorial != null)
        {
            currentTutorial.Execute(this);
        }
    }

    public void SetNextTutorial(SceneState _sceneState)
    {
        // 현재 튜토리얼의 Exit() 메소드 호출
        if (currentTutorial != null)
        {
            currentTutorial.Exit();
        }

        // 마지막 튜토리얼을 진행했다면 CompletedAllTutorials() 메소드 호출
        if (currentIndex >= tutorials.Count - 1)
        {
            CompletedAllTutorials(_sceneState);
            return;
        }

        // 다음 튜토리얼 과정을 currentTutorial로 등록
        currentIndex++;
        currentTutorial = tutorials[currentIndex];

        // 새로 바뀐 튜토리얼의 Enter() 메소드 호출
        currentTutorial.Enter();
    }

    public void CompletedAllTutorials(SceneState _sceneState)
    {
        this.transform.parent.gameObject.GetComponent<TutorialManager>().ChangeScene();
        currentTutorial = null;

        Debug.Log("Complete Scene");
        if(_sceneState!=SceneState.None)
        {
            Debug.Log("CompletedAllTutorials and SceneChange");
            _gameSceneManager.ChangeSceneState(_sceneState);
        }
        Debug.Log("ChangeSceneNum"+this.gameObject.name);
    }
}

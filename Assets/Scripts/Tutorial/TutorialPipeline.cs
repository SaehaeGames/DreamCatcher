using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPipeline : MonoBehaviour
{
    [SerializeField]
    private List<InteractiveSequenceBase> tutorials;
    [SerializeField]
    private string nextSceneName = "";

    private InteractiveSequenceBase currentTutorial = null;
    private int currentIndex = -1;
    private GameSceneManager _gameSceneManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameSceneManager = GameSceneManager.Instance; // 게임씬매니저 초기화

        // 튜토리얼 리스트 초기화
        tutorials.Clear(); // 튜토리얼 리스트 비우기
        for (int i = 0; i < this.transform.childCount; i++)
        {
            tutorials.Add(this.transform.GetChild(i).gameObject.GetComponent<InteractiveSequenceBase>()); // 튜토리얼 리스트 채우기
        }

        // 다음 튜토리얼 불러오기
        SetNextTutorial(SceneState.None);
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 튜토리얼 진행중
        if (currentTutorial != null)
        {
            currentTutorial.Execute(this);
        }
    }

    // 다음 튜토리얼 불러오기
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
        Debug.Log("<color=red>튜토리얼 넘어감</color>");
        currentTutorial.Enter();
    }

    // 튜토리얼 리스트 완료
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

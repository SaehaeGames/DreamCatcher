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
    private bool isStarted;

    // Start is called before the first frame update
    void Start()
    {
        isStarted = true;
        InitializePipeline();
    }

    private void OnEnable()
    {
        if (isStarted)
        {
            InitializePipeline();
        }
    }

    private void OnDisable()
    {
        if (currentTutorial != null)
        {
            currentTutorial.Exit();
            currentTutorial = null;
        }
    }

    private void InitializePipeline()
    {
        _gameSceneManager = GameSceneManager.Instance; // 게임씬매니저 초기화
        currentIndex = -1;
        currentTutorial = null;

        // 튜토리얼 리스트 초기화
        if (tutorials == null)
        {
            tutorials = new List<InteractiveSequenceBase>();
        }
        tutorials.Clear(); // 튜토리얼 리스트 비우기
        for (int i = 0; i < this.transform.childCount; i++)
        {
            InteractiveSequenceBase tutorial = this.transform.GetChild(i).gameObject.GetComponent<InteractiveSequenceBase>();
            if (tutorial == null)
            {
                Debug.LogError($"[TutorialPipeline] {gameObject.name}/{transform.GetChild(i).name}에 실행 시퀀스가 없습니다.");
                return;
            }

            tutorials.Add(tutorial); // 튜토리얼 리스트 채우기
        }

        if (tutorials.Count == 0)
        {
            Debug.LogWarning($"[TutorialPipeline] {gameObject.name}에 실행할 튜토리얼이 없습니다.");
            return;
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
            currentTutorial = null;
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
        Debug.Log($"[CompletedAllTutorials 호출] 내 이름: {this.gameObject.name}");

        currentTutorial = null;
        this.transform.parent.gameObject.GetComponent<TutorialManager>().ChangeScene();

        Debug.Log("Complete Scene");
        if(_sceneState!=SceneState.None)
        {
            Debug.Log("CompletedAllTutorials and SceneChange");
            _gameSceneManager.ChangeSceneState(_sceneState);
        }
        Debug.Log("ChangeSceneNum"+this.gameObject.name);
    }
}

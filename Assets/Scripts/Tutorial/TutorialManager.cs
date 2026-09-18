using UnityEngine;

/// <summary>
/// 튜토리얼 시퀀스가 공통으로 사용하는 씬 참조를 한 번만 찾아 전달하는 읽기 전용 Context.
/// </summary>
public sealed class TutorialContext
{
    public ScriptBox ScriptBox { get; }
    public GameObject TutorialOverlay { get; }
    public GameObject UiCanvas { get; }
    public BottomBar BottomBar { get; }

    /// <summary>
    /// TutorialManager가 현재 Unity 씬에서 확인한 공용 참조를 하나의 Context로 묶음.
    /// </summary>
    public TutorialContext(
        ScriptBox scriptBox,
        GameObject tutorialOverlay,
        GameObject uiCanvas,
        BottomBar bottomBar)
    {
        ScriptBox = scriptBox;
        TutorialOverlay = tutorialOverlay;
        UiCanvas = uiCanvas;
        BottomBar = bottomBar;
    }

    /// <summary>
    /// 시퀀스의 부모 TutorialManager에서 현재 씬의 공용 Context를 반환함.
    /// </summary>
    public static TutorialContext Get(Component owner)
    {
        TutorialManager manager = owner != null
            ? owner.GetComponentInParent<TutorialManager>()
            : null;
        return manager != null ? manager.Context : null;
    }
}

/// <summary>
/// 저장된 튜토리얼 Scene 번호에 해당하는 자식 파이프라인만 활성화하고 단계 완료 시 다음 번호를 저장함.
/// 현재 Unity 씬에 다음 번호의 자식이 없으면 다른 Unity 씬에서 같은 번호를 이어서 실행할 수 있음.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public const int LastTutorialSceneIndex = 28;

    [Header("튜토리얼 시작 연출")]
    [SerializeField] private GameObject tutorialFadePanal;

    [Header("튜토리얼 공통 참조")]
    [SerializeField] private ScriptBox scriptBox;
    [SerializeField] private GameObject tutorialOverlay;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private BottomBar bottomBar;

    private TutorialRuntimeState runtime;

    public TutorialContext Context { get; private set; }

    /// <summary>
    /// 모든 단계 자식을 먼저 끄고 공용 씬 참조를 한 번만 준비함.
    /// </summary>
    private void Awake()
    {
        // 1. 저장 데이터를 읽기 전 잘못된 단계가 노출되지 않도록 모든 Scene 자식을 끔.
        DeactivateAllTutorialScenes();

        // 2. Inspector 참조가 비어 있는 항목만 현재 씬에서 보완함.
        ResolveContextReferences();

        // 3. 각 시퀀스가 반복 검색하지 않도록 읽기 전용 Context를 생성함.
        Context = new TutorialContext(scriptBox, tutorialOverlay, uiCanvas, bottomBar);
    }

    /// <summary>
    /// 저장된 진행 번호와 퀘스트 연출 상태를 확인하여 현재 Unity 씬의 해당 Tutorial Scene만 활성화함.
    /// </summary>
    private void Start()
    {
        // 1. 플레이어 데이터 관리자와 저장된 튜토리얼 번호를 가져옴.
        if (!TryInitializePlayerData())
        {
            SetFadePanelActive(false);
            return;
        }

        // 2. 튜토리얼 범위 밖이거나 퀘스트 연출 중이면 이 관리자는 실행하지 않음.
        if (!ShouldRunTutorial())
        {
            if (scriptBox != null)
            {
                scriptBox.ScriptBoxOnOff(false);
            }

            SetFadePanelActive(false);
            return;
        }

        // 3. 현재 Unity 씬에 저장 번호와 같은 Scene 자식이 있으면 해당 파이프라인을 활성화함.
        ActivateTutorialScene(runtime.CurrentScene);

        // 4. 전체 튜토리얼의 첫 단계에서만 검은 시작 페이드 패널을 켬.
        SetFadePanelActive(runtime.CurrentScene == 0);
    }

    /// <summary>
    /// 저장된 Scene 번호가 전체 튜토리얼 범위 안인지 공통으로 판정함.
    /// </summary>
    public static bool IsTutorialScene(int sceneIndex)
    {
        return sceneIndex >= 0 && sceneIndex <= LastTutorialSceneIndex;
    }

    /// <summary>
    /// 현재 Unity 씬에서 실제로 실행 중인 Tutorial Scene의 파이프라인을 반환함.
    /// </summary>
    public bool TryGetActivePipeline(out TutorialPipeline pipeline)
    {
        pipeline = null;
        if (!TryFindTutorialScene(runtime.CurrentScene, out Transform sceneTransform)
            || !sceneTransform.gameObject.activeInHierarchy)
        {
            return false;
        }

        pipeline = sceneTransform.GetComponent<TutorialPipeline>();
        return pipeline != null;
    }

    /// <summary>
    /// 다음 Tutorial Scene 번호로 진행하는 기본 호출.
    /// </summary>
    public void ChangeScene()
    {
        ChangeScene(false);
    }

    /// <summary>
    /// 현재 Scene 자식을 끄고 다음 진행 번호와 예약된 해금 변경을 함께 저장한 뒤 다음 자식을 활성화함.
    /// </summary>
    public void ChangeScene(bool notifyUnlockChanged)
    {
        // 1. 저장 관리자가 유효한지 먼저 확인하여 진행 번호 계산이 다시 로드된 값으로 덮이지 않게 함.
        if (runtime.PlayerDataManager == null && !TryInitializePlayerData())
        {
            return;
        }

        // 2. 완료된 현재 Tutorial Scene을 비활성화함.
        if (TryFindTutorialScene(runtime.CurrentScene, out Transform currentSceneTransform))
        {
            currentSceneTransform.gameObject.SetActive(false);
        }

        // 3. 다음 번호로 이동하고 해금 UI 갱신 여부와 함께 PlayerData에 저장함.
        runtime.CurrentScene++;
        runtime.PlayerDataManager.SetCurrentScene(runtime.CurrentScene, notifyUnlockChanged);

        // 4. 같은 Unity 씬에 다음 번호의 자식이 있으면 바로 이어서 활성화함.
        if (TryFindTutorialScene(runtime.CurrentScene, out Transform nextSceneTransform))
        {
            nextSceneTransform.gameObject.SetActive(true);
            return;
        }

        // 5. 다음 자식이 없으면 현재 Unity 씬의 튜토리얼 구간이 끝났음을 기록함.
        Debug.Log($"[TutorialManager] Scene {runtime.CurrentScene}이 현재 Unity 씬에 없습니다.");
    }

    /// <summary>
    /// TutorialManager의 직접 자식으로 배치된 모든 Scene 파이프라인을 비활성화함.
    /// </summary>
    private void DeactivateAllTutorialScenes()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Inspector에 연결되지 않은 공용 참조를 현재 씬에서 한 번만 찾아 보완함.
    /// </summary>
    private void ResolveContextReferences()
    {
        // 1. 컴포넌트 기반 참조는 비활성 오브젝트를 포함하여 찾음.
        if (scriptBox == null)
        {
            scriptBox = FindObjectOfType<ScriptBox>(true);
        }

        if (bottomBar == null)
        {
            bottomBar = FindObjectOfType<BottomBar>(true);
        }

        // 2. 고유 태그를 가진 UI 루트는 태그 검색으로 찾음.
        if (tutorialOverlay == null)
        {
            tutorialOverlay = FindObjectWithTag(Constants.Tag_TutorialOverlay);
        }

        if (uiCanvas == null)
        {
            uiCanvas = FindObjectWithTag(Constants.Tag_UICanvas);
        }
    }

    /// <summary>
    /// GameManager에서 PlayerDataManager와 저장된 현재 튜토리얼 번호를 가져옴.
    /// </summary>
    private bool TryInitializePlayerData()
    {
        if (GameManager.instance == null || GameManager.instance.playerDataManager == null)
        {
            Debug.LogError("[TutorialManager] PlayerDataManager를 찾을 수 없습니다.");
            return false;
        }

        runtime.PlayerDataManager = GameManager.instance.playerDataManager;
        runtime.CurrentScene = runtime.PlayerDataManager.GetCurrentScene();
        return true;
    }

    /// <summary>
    /// 현재 저장 상태가 튜토리얼 범위이며 다른 퀘스트 연출이 실행 중이지 않은지 확인함.
    /// </summary>
    private bool ShouldRunTutorial()
    {
        return IsTutorialScene(runtime.CurrentScene)
            && !runtime.PlayerDataManager.GetIsQuestActinoPlaying();
    }

    /// <summary>
    /// 지정 번호의 Tutorial Scene 자식을 활성화하고 없으면 구성 경고를 남김.
    /// </summary>
    private void ActivateTutorialScene(int sceneIndex)
    {
        if (TryFindTutorialScene(sceneIndex, out Transform sceneTransform))
        {
            sceneTransform.gameObject.SetActive(true);
            return;
        }

        Debug.LogWarning($"[TutorialManager] 현재 Unity 씬에는 'Scene {sceneIndex}' 오브젝트가 없습니다.");
    }

    /// <summary>
    /// 지정 번호를 기존 하이어라키 이름 규칙인 Scene N으로 변환하여 직접 자식을 찾음.
    /// </summary>
    private bool TryFindTutorialScene(int sceneIndex, out Transform sceneTransform)
    {
        sceneTransform = transform.Find($"Scene {sceneIndex}");
        return sceneTransform != null;
    }

    /// <summary>
    /// 시작 페이드 패널이 연결된 경우에만 활성 상태를 변경함.
    /// </summary>
    private void SetFadePanelActive(bool isActive)
    {
        if (tutorialFadePanal != null)
        {
            tutorialFadePanal.SetActive(isActive);
        }
    }

    /// <summary>
    /// 태그가 비어 있거나 프로젝트에 등록되지 않은 경우에도 초기화 전체가 중단되지 않게 안전하게 검색함.
    /// </summary>
    private static GameObject FindObjectWithTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return null;
        }

        try
        {
            return GameObject.FindGameObjectWithTag(tag);
        }
        catch (UnityException)
        {
            return null;
        }
    }

    /// <summary>
    /// 저장 관리자와 현재 튜토리얼 번호를 한곳에 묶은 값 형식 실행 상태.
    /// </summary>
    private struct TutorialRuntimeState
    {
        public PlayerDataManager PlayerDataManager;
        public int CurrentScene;
    }
}

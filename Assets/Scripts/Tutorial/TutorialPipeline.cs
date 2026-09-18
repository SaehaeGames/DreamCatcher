using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나의 TutorialManager/Scene 아래에 배치된 시퀀스 자식들을 하이어라키 순서대로 실행함.
/// </summary>
public class TutorialPipeline : MonoBehaviour
{
    [Header("실행 시퀀스")]
    [SerializeField] private List<InteractiveSequenceBase> tutorials = new List<InteractiveSequenceBase>();

    private readonly List<PendingAction> pendingActions = new List<PendingAction>();
    private PipelineRuntimeState runtime;

    /// <summary>
    /// Unity의 첫 활성화에서 파이프라인을 한 번 초기화하고 이후 OnEnable 재초기화와 구분함.
    /// </summary>
    private void Start()
    {
        runtime.HasStarted = true;
        InitializePipeline();
    }

    /// <summary>
    /// 이미 시작된 파이프라인이 다시 활성화되면 인덱스와 시퀀스 상태를 처음부터 재구성함.
    /// </summary>
    private void OnEnable()
    {
        if (runtime.HasStarted)
        {
            InitializePipeline();
        }
    }

    /// <summary>
    /// 현재 시퀀스의 임시 UI를 정리하고 Scene 완료 전의 예약 작업을 취소함.
    /// </summary>
    private void OnDisable()
    {
        // 1. 현재 시퀀스가 만든 UI·리스너·부모 변경을 Exit에서 복구함.
        ExitCurrentSequence();

        // 2. 아직 완료되지 않은 해금·소비 예약의 취소 동작을 실행함.
        CancelPendingActions();

        // 3. 다음 활성화가 첫 시퀀스부터 시작하도록 실행 위치를 초기화함.
        runtime.CurrentIndex = -1;
        runtime.IsChangingSequence = false;
    }

    /// <summary>
    /// 현재 시퀀스의 완료 조건을 매 프레임 검사함.
    /// </summary>
    private void Update()
    {
        if (runtime.CurrentSequence != null && !runtime.IsChangingSequence)
        {
            runtime.CurrentSequence.Execute(this);
        }
    }

    /// <summary>
    /// 하이어라키의 직접 자식을 실행 순서대로 수집하고 첫 시퀀스의 Enter를 호출함.
    /// </summary>
    private void InitializePipeline()
    {
        // 1. 씬 이동과 Tutorial Scene 완료를 담당할 관리자 참조를 준비함.
        runtime.SceneManager = GameSceneManager.Instance;
        runtime.TutorialManager = GetComponentInParent<TutorialManager>();

        // 2. 재활성화 전에 남은 현재 시퀀스와 미완료 예약 작업을 정리함.
        ExitCurrentSequence();
        CancelPendingActions();
        runtime.CurrentIndex = -1;
        runtime.IsChangingSequence = false;

        // 3. 하이어라키 순서대로 실행 목록을 다시 구성함.
        if (!RebuildSequenceList())
        {
            return;
        }

        // 4. 첫 번째 시퀀스를 현재 단계로 선택하고 Enter를 호출함.
        SetNextTutorial(SceneState.None);
    }

    /// <summary>
    /// 해금·보상 데이터 변경을 현재 Tutorial Scene 전체 완료 시점까지 보류하도록 등록함.
    /// </summary>
    public void RegisterCompletionAction(Action completionAction)
    {
        RegisterCompletionAction(completionAction, null);
    }

    /// <summary>
    /// 단계 도중 임시 적용한 상태를 Scene 완료 시 확정하고 중단 시 되돌릴 작업을 한 쌍으로 등록함.
    /// </summary>
    public void RegisterCompletionAction(Action completionAction, Action cancellationAction)
    {
        if (completionAction == null)
        {
            return;
        }

        pendingActions.Add(new PendingAction(completionAction, cancellationAction));
    }

    /// <summary>
    /// 현재 시퀀스를 정리한 뒤 다음 자식의 Enter를 호출하거나 전체 Scene 완료 처리를 시작함.
    /// </summary>
    public void SetNextTutorial(SceneState nextSceneState)
    {
        // 같은 프레임에 여러 완료 조건이 들어와도 한 번의 전환만 수행함.
        if (runtime.IsChangingSequence)
        {
            return;
        }

        runtime.IsChangingSequence = true;
        try
        {
            // 1. 완료된 현재 시퀀스의 임시 상태를 Exit에서 정리함.
            ExitCurrentSequence();

            // 2. 마지막 시퀀스까지 끝났다면 예약 작업과 Scene 번호를 확정함.
            if (runtime.CurrentIndex >= tutorials.Count - 1)
            {
                CompleteAllTutorials(nextSceneState);
                return;
            }

            // 3. 다음 하이어라키 순서의 시퀀스를 선택함.
            runtime.CurrentIndex++;
            runtime.CurrentSequence = tutorials[runtime.CurrentIndex];

            // 4. 새 시퀀스가 입력·UI·완료 상태를 준비하도록 Enter를 호출함.
            runtime.CurrentSequence.Enter();
        }
        finally
        {
            runtime.IsChangingSequence = false;
        }
    }

    /// <summary>
    /// 예약된 데이터 변경을 적용하고 다음 Tutorial Scene 번호와 함께 저장함.
    /// </summary>
    public void CompletedAllTutorials(SceneState nextSceneState)
    {
        CompleteAllTutorials(nextSceneState);
    }

    /// <summary>
    /// 직접 자식에 붙은 시퀀스를 하이어라키 순서대로 목록에 다시 채움.
    /// </summary>
    private bool RebuildSequenceList()
    {
        tutorials.Clear();

        // 1. 모든 직접 자식에서 공통 시퀀스 컴포넌트를 가져옴.
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            InteractiveSequenceBase sequence = child.GetComponent<InteractiveSequenceBase>();
            if (sequence == null)
            {
                Debug.LogError($"[TutorialPipeline] {gameObject.name}/{child.name}에 실행 시퀀스가 없습니다.");
                tutorials.Clear();
                return false;
            }

            tutorials.Add(sequence);
        }

        // 2. 실행할 자식이 없는 Scene 구성은 로그를 남긴 후 시작하지 않음.
        if (tutorials.Count == 0)
        {
            Debug.LogWarning($"[TutorialPipeline] {gameObject.name}에 실행할 튜토리얼이 없습니다.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 현재 시퀀스가 있으면 Exit를 한 번 호출하고 참조를 비움.
    /// </summary>
    private void ExitCurrentSequence()
    {
        if (runtime.CurrentSequence == null)
        {
            return;
        }

        runtime.CurrentSequence.Exit();
        runtime.CurrentSequence = null;
    }

    /// <summary>
    /// 현재 Tutorial Scene의 모든 시퀀스가 끝났을 때 예약 작업·진행 번호·선택적 씬 이동을 순서대로 처리함.
    /// </summary>
    private void CompleteAllTutorials(SceneState nextSceneState)
    {
        // 1. 호출 중 새 예약이 등록되어도 현재 완료 묶음과 섞이지 않도록 복사 후 원본을 먼저 비움.
        PendingAction[] actionsToCommit = pendingActions.ToArray();
        pendingActions.Clear();

        // 2. 해금·보상·소비처럼 Scene 완료까지 보류한 데이터 변경을 확정함.
        for (int i = 0; i < actionsToCommit.Length; i++)
        {
            actionsToCommit[i].Complete?.Invoke();
        }

        // 3. 예약 변경 여부와 다음 튜토리얼 번호를 PlayerData에 한 번에 저장함.
        bool notifyUnlockChanged = actionsToCommit.Length > 0;
        if (runtime.TutorialManager != null)
        {
            runtime.TutorialManager.ChangeScene(notifyUnlockChanged);
        }
        else
        {
            Debug.LogError($"[TutorialPipeline] {gameObject.name}: TutorialManager를 찾을 수 없습니다.");
        }

        // 4. 현재 단계가 Unity 씬 이동을 요청한 경우 저장이 끝난 뒤 이동함.
        if (nextSceneState != SceneState.None && runtime.SceneManager != null)
        {
            runtime.SceneManager.ChangeSceneState(nextSceneState);
        }
    }

    /// <summary>
    /// Scene 완료 전에 파이프라인이 꺼지면 임시 소비 같은 예약 상태를 원래 데이터로 되돌림.
    /// </summary>
    private void CancelPendingActions()
    {
        if (pendingActions.Count == 0)
        {
            return;
        }

        // 1. 취소 콜백이 새 작업을 등록하더라도 현재 취소 묶음과 섞이지 않도록 복사 후 원본을 비움.
        PendingAction[] actionsToCancel = pendingActions.ToArray();
        pendingActions.Clear();

        // 2. 각 예약 작업이 제공한 취소 동작을 등록 순서대로 실행함.
        for (int i = 0; i < actionsToCancel.Length; i++)
        {
            actionsToCancel[i].Cancel?.Invoke();
        }
    }

    /// <summary>
    /// 완료 시 확정할 동작과 중단 시 되돌릴 동작을 한 쌍으로 보관함.
    /// </summary>
    private readonly struct PendingAction
    {
        public Action Complete { get; }
        public Action Cancel { get; }

        public PendingAction(Action complete, Action cancel)
        {
            Complete = complete;
            Cancel = cancel;
        }
    }

    /// <summary>
    /// 현재 실행 위치와 씬 관리자 참조를 한곳에 묶은 값 형식 파이프라인 상태.
    /// </summary>
    private struct PipelineRuntimeState
    {
        public InteractiveSequenceBase CurrentSequence;
        public TutorialManager TutorialManager;
        public GameSceneManager SceneManager;
        public int CurrentIndex;
        public bool HasStarted;
        public bool IsChangingSequence;
    }
}
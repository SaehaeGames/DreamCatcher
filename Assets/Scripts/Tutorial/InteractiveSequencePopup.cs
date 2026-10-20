
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 공용 팝업을 표시하고 확인 버튼을 누르면 다음 시퀀스로 진행함.
/// </summary>
public class InteractiveSequencePopup : InteractiveSequenceBase
{
    [Header("공통 팝업")]
    [SerializeField] private GameObject unlockPopUp;
    [SerializeField] private Button okBtn;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text messageText;

    [Header("팝업 표시")]
    [SerializeField] private Sprite popupIcon;
    [SerializeField, TextArea] private string popupMessage;

    private bool isCompleted;

    /// <summary>
    /// 팝업을 표시하고 확인 버튼 입력을 기다림.
    /// </summary>
    public override void Enter()
    {
        // 1. 재실행 시 이전 완료 상태를 초기화함.
        isCompleted = false;

        // 2. 팝업과 내부 UI 참조를 확보함.
        if (!TutorialUnlockPopupUtility.Resolve(
                this,
                ref unlockPopUp,
                ref okBtn,
                ref itemImage,
                ref messageText))
        {
            return;
        }

        // 3. 아이콘과 문구를 설정하고 팝업을 표시함.
        TutorialUnlockPopupUtility.Show(
            unlockPopUp,
            itemImage,
            messageText,
            popupIcon,
            popupMessage);

        // 4. 확인 버튼 클릭 이벤트를 등록함.
        TutorialUnlockPopupUtility.BindOkButton(
            okBtn,
            OnOkButtonClicked);
    }

    /// <summary>
    /// 확인 버튼 입력이 완료되면 다음 시퀀스로 진행함.
    /// </summary>
    public override void Execute()
    {
        if (!isCompleted)
        {
            return;
        }

        // 중복으로 다음 시퀀스를 진행하지 않도록 먼저 완료 상태를 초기화함.
        isCompleted = false;

        pipeline.SetSceneState(SceneState.None);
        pipeline.MoveToNextSequence();
    }

    /// <summary>
    /// 팝업을 닫고 버튼 이벤트를 정리함.
    /// </summary>
    public override void Exit()
    {
        // 1. 공용 유틸리티를 통해 팝업을 닫음.
        TutorialUnlockPopupUtility.Hide(unlockPopUp);

        // 2. 이 시퀀스가 등록한 버튼 이벤트를 해제함.
        if (okBtn != null)
        {
            TutorialUnlockPopupUtility.UnbindOkButton(
                okBtn,
                OnOkButtonClicked);
        }

        // 3. 완료 상태를 초기화함.
        isCompleted = false;
    }

    /// <summary>
    /// 확인 버튼 클릭 시 시퀀스 완료 상태로 변경함.
    /// </summary>
    private void OnOkButtonClicked()
    {
        isCompleted = true;
    }
}

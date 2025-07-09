using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveSequenceClick : InteractiveSequenceBase
{
    private GameObject canvas;

    private ScriptBox scriptBox;
    private Transform startParent;
    private GameObject duplicatedClickBtn;
    private GameObject TutorialOverlayPanal;
    private GameObject ArrowImage;
    private GameObject BlockPanal;

    [Header("화살표 강조 ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;
    [SerializeField] private bool doClickButnDuplicate;
    [SerializeField] private Sprite arrowImg;

    [Header("클릭/드래그 대상-입력")]
    [SerializeField] private GameObject clickBtn; // 클릭/드래그 대상

    [Header("그림자 패널 사용-입력")]
    [SerializeField] private bool panelChange; // 그림자 패널 사용 여부
    [SerializeField] private int panelChangeNum; // 그림자 패널 번호
    [SerializeField] GameObject shadowPanal; // 그림자 패널
    [SerializeField] private Sprite[] shadowImages; // 그림자 패널 리소스

    private BottomBar _bottomBar;
    private GameSceneManager _gameSceneManager;
    private SceneState[] sceneStates = { SceneState.Main, SceneState.Making, SceneState.CollectionDream, SceneState.Store };

    public override void Enter()
    {
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
        if (transform.GetSiblingIndex() == 0)
            scriptBox.ScriptBoxOnOff(false);

        clickBtn.GetComponent<InteractiveButton>().SetButtonClicked(false);

        canvas = GameObject.FindGameObjectWithTag("UI Canvas");
        _bottomBar = GameObject.FindGameObjectWithTag("BottomBar").GetComponent<BottomBar>();
        _gameSceneManager = GameSceneManager.Instance;

        if (highlightArrowOnOff)
        {
            if (doClickButnDuplicate) DuplicateClickButton();
            SetupArrowHighlight();
            SetClickButtonToOverlay();
        }

        if (panelChange)
        {
            SetupShadowPanel();
        }
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (clickBtn.GetComponent<InteractiveButton>() != null)
        {
            if (doClickButnDuplicate && duplicatedClickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                Debug.Log("복제된 버튼 눌림 인식");
                Destroy(duplicatedClickBtn);
                tutorialPipeline.SetNextTutorial(sceneStates[panelChangeNum]); // 다음 튜토리얼
            }
            if (highlightArrowOnOff && clickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                Debug.Log("화살표 강조 버튼 눌림 인식");
                clickBtn.transform.SetParent(startParent);
                tutorialPipeline.SetNextTutorial(sceneStates[panelChangeNum]); // 다음 튜토리얼
            }
            if(clickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                Debug.Log("일반 버튼 눌림 인식");
                tutorialPipeline.SetNextTutorial(sceneStates[panelChangeNum]); // 다음 튜토리얼
            }
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        // 해당 버튼을 누르면
        if (clickBtn.GetComponent<InteractiveButton>() != null)
        {
            if (doClickButnDuplicate && duplicatedClickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                Debug.Log("클릭버튼 위치 이후 : " + clickBtn.transform.position);
                Destroy(duplicatedClickBtn);
                questActionPipeline.SetNextQuestAction(); // 다음 튜토리얼
            }
            else if (highlightArrowOnOff && clickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                clickBtn.transform.SetParent(startParent);
                questActionPipeline.SetNextQuestAction(); // 다음 튜토리얼
            }
            else if (clickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                questActionPipeline.SetNextQuestAction(); // 다음 튜토리얼
            }
        }
    }

    public override void Exit()
    {
        if(TutorialOverlayPanal!=null)
        {
            ArrowImage.GetComponent<Image>().sprite = null;
            ArrowImage.SetActive(false);
            BlockPanal.SetActive(false);
        }
        
        if (panelChange)
        {
            Destroy(shadowPanal);
        }
    }

    private void SetupArrowHighlight()
    {
        TutorialOverlayPanal = GameObject.FindGameObjectWithTag("TutorialOverlay");
        ArrowImage = TutorialOverlayPanal.transform.GetChild(1).gameObject;
        BlockPanal = TutorialOverlayPanal.transform.GetChild(0).gameObject;

        ArrowImage.SetActive(true);
        BlockPanal.SetActive(true);
        ArrowImage.GetComponent<Image>().sprite = arrowImg;

        ArrowImage.GetComponent<Animator>().enabled = true;
        ArrowImage.GetComponent<Animator>().Play("blinkArrow");
    }

    private void DuplicateClickButton()
    {
        duplicatedClickBtn = Instantiate(clickBtn, clickBtn.transform.position, clickBtn.transform.rotation);
    }

    private void SetClickButtonToOverlay()
    {
        if (doClickButnDuplicate)
        {
            duplicatedClickBtn.transform.SetParent(TutorialOverlayPanal.transform);
            duplicatedClickBtn.transform.localScale = Vector3.one;
        }
        else
        {
            startParent = clickBtn.transform.parent;
            clickBtn.transform.SetParent(TutorialOverlayPanal.transform);
        }
    }

    private void SetupShadowPanel()
    {
        shadowPanal = Instantiate(shadowPanal, Vector2.zero, Quaternion.identity);
        shadowPanal.transform.SetParent(canvas.transform, false);
        shadowPanal.GetComponent<Image>().sprite = shadowImages[panelChangeNum];
        _bottomBar.onClickRemove(panelChangeNum);
    }

}

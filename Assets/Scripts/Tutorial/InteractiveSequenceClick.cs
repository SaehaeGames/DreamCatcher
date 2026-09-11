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
    private Canvas clickButtonCanvas;
    private GraphicRaycaster clickButtonRaycaster;
    private bool addedClickButtonCanvas;
    private bool addedClickButtonRaycaster;
    private bool originalOverrideSorting;
    private int originalSortingOrder;
    private GameObject TutorialOverlayPanal;
    private GameObject ArrowImage;
    private GameObject BlockPanal;
    private InteractiveButton interactiveButton;

    [Header("화살표 강조 ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;
    [SerializeField] private bool doClickBtn;
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
    private int suspendedBottomBarMenu = -1;
    private SceneState[] sceneStates = { SceneState.Main, SceneState.Making, SceneState.CollectionDream, SceneState.Store };

    public override void Enter()
    {
        if (clickBtn == null)
        {
            Debug.LogError($"[InteractiveSequenceClick] {gameObject.name}의 clickBtn이 설정되지 않았습니다.");
            return;
        }

        interactiveButton = clickBtn.GetComponent<InteractiveButton>();
        if (interactiveButton == null)
        {
            Debug.LogError($"[InteractiveSequenceClick] {clickBtn.name}에 InteractiveButton이 없습니다.");
            return;
        }

        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
        if (transform.GetSiblingIndex() == 0 && scriptBox != null)
            scriptBox.ScriptBoxOnOff(false);

        interactiveButton.SetButtonClicked(false);

        canvas = GameObject.FindGameObjectWithTag("UI Canvas");
        GameObject bottomBarObject = GameObject.FindGameObjectWithTag("BottomBar");
        if (bottomBarObject != null)
        {
            _bottomBar = bottomBarObject.GetComponent<BottomBar>();
        }
        SuspendBottomBarNavigation();
        _gameSceneManager = GameSceneManager.Instance;

        if (highlightArrowOnOff)
        {
            SetupArrowHighlight();
            if (TutorialOverlayPanal != null)
            {
                SetClickButtonToOverlay();
            }
        }

        if (panelChange)
        {
            SetupShadowPanel();
        }
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        if (interactiveButton != null && interactiveButton.GetButtonClicked())
        {
            if (highlightArrowOnOff && !doClickBtn)
            {
                clickBtn.transform.SetParent(startParent);
            }

            tutorialPipeline.SetNextTutorial(sceneStates[panelChangeNum]);
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        if (interactiveButton != null && interactiveButton.GetButtonClicked())
        {
            if (highlightArrowOnOff && !doClickBtn)
            {
                clickBtn.transform.SetParent(startParent);
            }

            questActionPipeline.SetNextQuestAction();
        }
    }

    public override void Exit()
    {
        RestoreBottomBarNavigation();
        RestoreClickButtonSorting();

        if (clickBtn != null && startParent != null)
        {
            clickBtn.transform.SetParent(startParent);
        }

        if (TutorialOverlayPanal != null && ArrowImage != null && BlockPanal != null)
        {
            ArrowImage.GetComponent<Image>().sprite = null;
            ArrowImage.SetActive(false);
            BlockPanal.SetActive(false);
        }
        
        if (panelChange && shadowPanal != null)
        {
            Destroy(shadowPanal);
        }
    }

    private void SetupArrowHighlight()
    {
        TutorialOverlayPanal = GameObject.FindGameObjectWithTag("TutorialOverlay");
        if (TutorialOverlayPanal == null)
        {
            Debug.LogError($"[InteractiveSequenceClick] {gameObject.name}에서 TutorialOverlay를 찾을 수 없습니다.");
            return;
        }
        /*ArrowImage = TutorialOverlayPanal.transform.GetChild(1).gameObject;
        BlockPanal = TutorialOverlayPanal.transform.GetChild(0).gameObject;*/

        ArrowImage = TutorialOverlayPanal.transform.Find("ArrowImage").gameObject;
        BlockPanal = TutorialOverlayPanal.transform.Find("BlockPanal").gameObject;

        ArrowImage.SetActive(true);
        BlockPanal.SetActive(true);
        ArrowImage.GetComponent<Image>().sprite = arrowImg;

        ArrowImage.GetComponent<Animator>().enabled = true;
        ArrowImage.GetComponent<Animator>().Play("blinkArrow");
    }

    private void SetClickButtonToOverlay()
    {
        startParent = clickBtn.transform.parent;

        if (doClickBtn)
        {
            SetClickButtonSorting();
        }
        else
        {
            clickBtn.transform.SetParent(TutorialOverlayPanal.transform);
        }
    }

    private void SetClickButtonSorting()
    {
        clickButtonCanvas = clickBtn.GetComponent<Canvas>();
        addedClickButtonCanvas = clickButtonCanvas == null;

        if (addedClickButtonCanvas)
        {
            clickButtonCanvas = clickBtn.AddComponent<Canvas>();
        }
        else
        {
            originalOverrideSorting = clickButtonCanvas.overrideSorting;
            originalSortingOrder = clickButtonCanvas.sortingOrder;
        }

        clickButtonRaycaster = clickBtn.GetComponent<GraphicRaycaster>();
        addedClickButtonRaycaster = clickButtonRaycaster == null;
        if (addedClickButtonRaycaster)
        {
            clickButtonRaycaster = clickBtn.AddComponent<GraphicRaycaster>();
        }

        clickButtonCanvas.overrideSorting = true;
        clickButtonCanvas.sortingOrder = 100;
    }

    private void RestoreClickButtonSorting()
    {
        if (clickButtonRaycaster != null && addedClickButtonRaycaster)
        {
            Destroy(clickButtonRaycaster);
        }

        if (clickButtonCanvas != null)
        {
            if (addedClickButtonCanvas)
            {
                Destroy(clickButtonCanvas);
            }
            else
            {
                clickButtonCanvas.overrideSorting = originalOverrideSorting;
                clickButtonCanvas.sortingOrder = originalSortingOrder;
            }
        }

        clickButtonCanvas = null;
        clickButtonRaycaster = null;
        addedClickButtonCanvas = false;
        addedClickButtonRaycaster = false;
    }

    private void SuspendBottomBarNavigation()
    {
        if (_bottomBar == null || !_bottomBar.TryGetMenuIndex(clickBtn, out int menu))
        {
            return;
        }

        suspendedBottomBarMenu = menu;
        _bottomBar.onClickRemove(menu);
    }

    private void RestoreBottomBarNavigation()
    {
        if (_bottomBar != null && suspendedBottomBarMenu >= 0)
        {
            _bottomBar.OnClickAdd(suspendedBottomBarMenu);
        }

        suspendedBottomBarMenu = -1;
    }

    private void SetupShadowPanel()
    {
        if (shadowPanal == null || canvas == null || shadowImages == null
            || panelChangeNum < 0 || panelChangeNum >= shadowImages.Length)
        {
            Debug.LogError($"[InteractiveSequenceClick] {gameObject.name}의 그림자 패널 설정이 올바르지 않습니다.");
            return;
        }

        shadowPanal = Instantiate(shadowPanal, Vector2.zero, Quaternion.identity);
        shadowPanal.transform.SetParent(canvas.transform, false);
        shadowPanal.GetComponent<Image>().sprite = shadowImages[panelChangeNum];
    }

}

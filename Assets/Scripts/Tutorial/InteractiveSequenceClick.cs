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

    [Header("ȭ��ǥ ���� ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;
    [SerializeField] private bool doClickButnDuplicate;
    [SerializeField] private Sprite arrowImg;

    [Header("Ŭ��/�巡�� ���-�Է�")]
    [SerializeField] private GameObject clickBtn; // Ŭ��/�巡�� ���

    [Header("�׸��� �г� ���-�Է�")]
    [SerializeField] private bool panelChange; // �׸��� �г� ��� ����
    [SerializeField] private int panelChangeNum; // �׸��� �г� ��ȣ
    [SerializeField] GameObject shadowPanal; // �׸��� �г�
    [SerializeField] private Sprite[] shadowImages; // �׸��� �г� ���ҽ�

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
                Debug.Log("������ ��ư ���� �ν�");
                Destroy(duplicatedClickBtn);
                tutorialPipeline.SetNextTutorial(sceneStates[panelChangeNum]); // ���� Ʃ�丮��
            }
            if (highlightArrowOnOff && clickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                Debug.Log("ȭ��ǥ ���� ��ư ���� �ν�");
                clickBtn.transform.SetParent(startParent);
                tutorialPipeline.SetNextTutorial(sceneStates[panelChangeNum]); // ���� Ʃ�丮��
            }
            if(clickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                Debug.Log("�Ϲ� ��ư ���� �ν�");
                tutorialPipeline.SetNextTutorial(sceneStates[panelChangeNum]); // ���� Ʃ�丮��
            }
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        // �ش� ��ư�� ������
        if (clickBtn.GetComponent<InteractiveButton>() != null)
        {
            if (doClickButnDuplicate && duplicatedClickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                Debug.Log("Ŭ����ư ��ġ ���� : " + clickBtn.transform.position);
                Destroy(duplicatedClickBtn);
                questActionPipeline.SetNextQuestAction(); // ���� Ʃ�丮��
            }
            else if (highlightArrowOnOff && clickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                clickBtn.transform.SetParent(startParent);
                questActionPipeline.SetNextQuestAction(); // ���� Ʃ�丮��
            }
            else if (clickBtn.GetComponent<InteractiveButton>().GetButtonClicked())
            {
                questActionPipeline.SetNextQuestAction(); // ���� Ʃ�丮��
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

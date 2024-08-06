using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialClick : TutorialBase
{
    private GameObject arrow;
    private GameObject arrowPrefab; // ȭ��ǥ ������
    private GameObject canvas;

    private ScriptBox scriptBox;
    private Transform startParent;
    private GameObject duplicatedClickBtn;

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
        // ��ũ��Ʈ �ڽ� ����
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            int index = transform.GetSiblingIndex();
            if (index == 0)
            {
                scriptBox.gameObject.GetComponent<ScriptBox>().ScriptBoxOnOff(false);
            }
        }

        // ĵ���� �ҷ�����
        canvas = GameObject.FindGameObjectWithTag("UI Canvas");
        _bottomBar = GameObject.FindGameObjectWithTag("BottomBar").GetComponent<BottomBar>();
        _gameSceneManager =GameSceneManager.Instance;

        if(highlightArrowOnOff)
        {
            if(doClickButnDuplicate)
            {
                duplicatedClickBtn = Instantiate(clickBtn, clickBtn.transform.position, clickBtn.transform.rotation);
            }
            // ȭ��ǥ ����
            arrowPrefab = Resources.Load<GameObject>("Prefabs/Tutorial/HighlightArrowPref");
            arrow = Instantiate(arrowPrefab, new Vector2(0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            arrow.GetComponent<Canvas>().worldCamera = Camera.main;
            GameObject arrowChild = arrow.transform.GetChild(0).gameObject;

            // ȭ��ǥ ��ġ ����
            arrowChild.GetComponent<Image>().sprite = arrowImg;

            // Ŭ��/�巡�� ��� ���� ����
            if(doClickButnDuplicate)
            {
                duplicatedClickBtn.transform.SetParent(arrow.transform);
                duplicatedClickBtn.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                startParent = clickBtn.transform.parent;
                clickBtn.transform.SetParent(arrow.transform);
            }

            // ȭ��ǥ ������ �ִϸ��̼� ���
            arrowChild.GetComponent<Animator>().enabled = true;
            arrowChild.GetComponent<Animator>().Play("blinkArrow");
        }     

        // �г� �̵���
        if (panelChange)
        {
            // �׸��� �г� ����
            shadowPanal = Instantiate(shadowPanal, new Vector2(0f, 0f), Quaternion.identity);
            shadowPanal.transform.SetParent(canvas.transform, false);
            shadowPanal.GetComponent<Image>().sprite = shadowImages[panelChangeNum]; // �г� ���� ����
            // ��ư ����
            _bottomBar.onClickRemove(panelChangeNum);
        }
    }

    public override void Execute(TutorialController controller)
    {
        // �ش� ��ư�� ������
        if (clickBtn.GetComponent<TutorialButton>() != null)
        {
            if (doClickButnDuplicate && duplicatedClickBtn.GetComponent<TutorialButton>().buttonClicked)
            {
                Destroy(duplicatedClickBtn);
                controller.SetNextTutorial(sceneStates[panelChangeNum]); // ���� Ʃ�丮��
            }
            else if (clickBtn.GetComponent<TutorialButton>().buttonClicked)
            {
                clickBtn.transform.SetParent(startParent);
                controller.SetNextTutorial(sceneStates[panelChangeNum]); // ���� Ʃ�丮��
            }
        }
    }

    public override void Exit()
    {
        Destroy(arrow); // ȭ��ǥ ����
        if (panelChange)
        {
            Destroy(shadowPanal);
        }
    }
}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialClick : TutorialBase
{
    private GameObject arrow;
    private GameObject arrowPrefab; // ȭ��ǥ ������
    private Vector2 arrowPos;
    private GameObject canvas;
    [SerializeField] private GameObject guidImg;

    private bool isDragObj;
    private ScriptBox scriptBox;
    [Header("ȭ��ǥ ���� ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;

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
        // ���� �ʱ�ȭ
        isDragObj = false;

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

        // ȭ��ǥ ����
        

        // ȭ��ǥ ����
        arrowPrefab = Resources.Load<GameObject>("Prefabs/Tutorial/HighlightArrowPref");
        arrow = Instantiate(arrowPrefab, new Vector2(0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        arrow.GetComponent<Canvas>().worldCamera = Camera.main;
        GameObject arrowChild = arrow.transform.GetChild(0).gameObject;



        // ȭ��ǥ ��ġ ����
        RectTransform rectTransform = clickBtn.GetComponent<RectTransform>(); // Ŭ�� ��� ��ġ
        if (rectTransform == null) return;
        Vector2 objectSize = rectTransform.sizeDelta;  // ��ư ������
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);
        Vector3 topCenter = (objectCorners[1] + objectCorners[2]) / 2;
        Vector2 arrowSize = arrowChild.GetComponent<RectTransform>().sizeDelta;


        //Vector2 objectPosition = clickBtn.transform.position; //��ư ��ġ

        // ���� ��
        /*arrowPos = new Vector2(objectPosition.x, objectPosition.y + +0.3f);
        arrowChild.transform.position = arrowPos;
        arrowChild.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));*/

        // ���� ��
        /*RectTransform uiCanvasRectTransform = clickBtn.transform.root.GetComponent<RectTransform>();
        if(uiCanvasRectTransform == null) return;*/

        arrowPos = new Vector3(rectTransform.position.x, topCenter.y + arrowSize.y / 2, topCenter.z);
        Debug.Log("topCenter" + topCenter + "arrowSize" + arrowSize+"clickBtnPositionX"+rectTransform.position.x);

        //Debug.Log("objectSize : " + objectSize.y / 2 + " | arrowSize : " + arrowChild.GetComponent<RectTransform>().sizeDelta.y / 2 + " | arrowPos : " + objectPosition.y + ((objectSize.y / 2) + (arrowChild.GetComponent<RectTransform>().sizeDelta.y / 2)) + " | objPosition x : " + objectPosition.x + " | objPosition y : " + objectPosition.y);

        //arrowChild.transform.position = arrowPos;
        //arrowChild.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)rectTransform.parent, arrowPos, Camera.main, out canvasPos);
        arrowChild.GetComponent<RectTransform>().anchoredPosition = canvasPos;

        Debug.Log("afterArrowPos" + arrowChild.transform.position+"afterArrowPosLocal"+arrowChild.transform.localPosition);

        arrowChild.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

        // ȭ��ǥ ������ �ִϸ��̼� ���
        arrowChild.GetComponent<Animator>().enabled = true;
        arrowChild.GetComponent<Animator>().Play("blinkArrow");

        // �г� �̵���
        if (panelChange)
        {
            // �׸��� �г� ����
            shadowPanal = Instantiate(shadowPanal, new Vector2(0f, 0f), Quaternion.identity);
            shadowPanal.transform.SetParent(canvas.transform, false);
            shadowPanal.GetComponent<Image>().sprite = shadowImages[panelChangeNum]; // �г� ���� ����
            // ��ư ����
            _bottomBar.onClickRemove(panelChangeNum);
            clickBtn.gameObject.transform.SetParent(canvas.transform, true); // ��ư ���� ����
            clickBtn.gameObject.transform.SetAsLastSibling(); // ��ư ���� ����
        }
    }

    public override void Execute(TutorialController controller)
    {
        // �ش� ��ư�� ������
        if (clickBtn.GetComponent<TutorialButton>() != null && clickBtn.GetComponent<TutorialButton>().buttonClicked)
        {
            controller.SetNextTutorial(sceneStates[panelChangeNum]); // ���� Ʃ�丮��
        } // �ش� ������Ʈ�� �巡���ϸ�
        else if(clickBtn.GetComponent<TutorialDrag>()!=null&&clickBtn.GetComponent<TutorialDrag>().GetObjectDraged())
        {
            isDragObj = true;
            controller.SetNextTutorial(SceneState.None); // ���� Ʃ�丮��
        }
    }

    public override void Exit()
    {
        if (guidImg != null)
        {
            guidImg.SetActive(false);
        }
        if(isDragObj)
        {
            clickBtn.GetComponent<TutorialDrag>().SetObjctDraged(false);
        }
        Destroy(arrow); // ȭ��ǥ ����
        if (panelChange)
        {
            Destroy(shadowPanal);
        }
    }
}

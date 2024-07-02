using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialArrow : TutorialBase
{
    [SerializeField] private GameObject arrow; // ȭ��ǥ ������
    private Vector2 arrowPos;
    private GameObject canvas;
    [SerializeField] private GameObject guidImg;

    private bool isDragObj;

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
        isDragObj = false;
        // ĵ���� �ҷ�����
        canvas = GameObject.FindGameObjectWithTag("UI Canvas");
        _bottomBar = GameObject.FindGameObjectWithTag("BottomBar").GetComponent<BottomBar>();
        _gameSceneManager=GameSceneManager.Instance;

        // ȭ��ǥ ����
        arrow = Instantiate(arrow, arrowPos, Quaternion.Euler(new Vector3(0f, 0f, 90f)));
        arrow.transform.SetParent(canvas.transform, false); // ĵ���� �Ʒ��� �̵�

        // ȭ��ǥ ��ġ ����
        Debug.Log("ȭ��ǥ ��ġ ����");
        /*
        RectTransform rectTransform = clickBtn.GetComponent<RectTransform>();
        if (rectTransform == null) return;
        Debug.Log("ȭ��ǥ ��ġ ����111");
        Vector2 objectSize = rectTransform.rect.size;*/
        Vector2 objectPosition = clickBtn.transform.position;
        arrowPos = new Vector2(objectPosition.x, objectPosition.y + +0.3f); //clickBtn.transform.position.y+0.3f
        //arrowPos = new Vector2(objectPosition.x, objectPosition.y+objectSize.y/2+arrow.GetComponent<RectTransform>().rect.size.y/2); //clickBtn.transform.position.y+0.3f
        //Debug.Log("objectSize : " + objectSize + " | arrowSize : " + arrow.GetComponent<RectTransform>().rect.size + " | arrowPos : " + arrowPos);*/
        arrow.transform.position = arrowPos;

        // ȭ��ǥ ������ �ִϸ��̼� ���
        arrow.GetComponent<Animator>().enabled = true;
        arrow.GetComponent<Animator>().Play("blinkArrow");

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

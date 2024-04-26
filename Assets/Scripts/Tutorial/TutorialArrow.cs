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

    [Header("Ŭ��/�巡�� ���-�Է�")]
    [SerializeField] private GameObject clickBtn; // Ŭ��/�巡�� ���

    [Header("�׸��� �г� ���-�Է�")]
    [SerializeField] private bool panelChange; // �׸��� �г� ��� ����
    [SerializeField] private int panelChangeNum; // �׸��� �г� ��ȣ
    [SerializeField] GameObject shadowPanal; // �׸��� �г�
    [SerializeField] private Sprite[] shadowImages; // �׸��� �г� ���ҽ�

    public override void Enter()
    {
        // ĵ���� �ҷ�����
        canvas = GameObject.FindObjectOfType<Canvas>().gameObject;

        // ȭ��ǥ ����
        arrow = Instantiate(arrow, arrowPos, Quaternion.Euler(new Vector3(0f, 0f, 90f)));
        arrow.transform.SetParent(canvas.transform, false); // ĵ���� �Ʒ��� �̵�
        // ȭ��ǥ ��ġ ����
        arrowPos = new Vector2(clickBtn.transform.position.x, clickBtn.transform.position.y+0.3f);
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
            clickBtn.gameObject.transform.SetAsLastSibling(); // ��ư ���� ����
        }
    }

    public override void Execute(TutorialController controller)
    {
        // �ش� ��ư�� ������
        if (clickBtn.GetComponent<TutorialButton>() != null && clickBtn.GetComponent<TutorialButton>().buttonClicked)
        {
            controller.SetNextTutorial(); // ���� Ʃ�丮��
        } // �ش� ������Ʈ�� �巡���ϸ�
        else if(clickBtn.GetComponent<TutorialDrag>()!=null&&clickBtn.GetComponent<TutorialDrag>().objectDraged)
        {
            controller.SetNextTutorial(); // ���� Ʃ�丮��
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

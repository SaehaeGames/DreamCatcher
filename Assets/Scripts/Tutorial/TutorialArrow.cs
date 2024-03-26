using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialArrow : TutorialBase
{
    [SerializeField]
    private GameObject arrow;
    [SerializeField]
    private GameObject clickBtn;

    private Vector2 arrowPos;
    private GameObject canvas;

    [SerializeField]
    private bool panelChange;
    [SerializeField]
    private int panelChangeNum;
    [SerializeField]
    GameObject shadowPanal;
    [SerializeField]
    private Sprite[] shadowImages;

    public override void Enter()
    {
        // ĵ���� �ҷ�����
        canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        // ȭ��ǥ ����
        arrowPos = new Vector2(clickBtn.transform.position.x, clickBtn.transform.position.y + 85f);
        arrow = Instantiate(arrow, arrowPos, Quaternion.Euler(new Vector3(0f, 0f, 90f)));
        arrow.transform.SetParent(canvas.transform, true); // ĵ���� �Ʒ��� �̵�
        // ȭ��ǥ ������ �ִϸ��̼� ���
        arrow.GetComponent<Animator>().enabled = true;
        arrow.GetComponent<Animator>().Play("blinkArrow");
        // �г� �̵���
        if (panelChange)
        {
            shadowPanal = Instantiate(shadowPanal, new Vector2(0f, 0f), Quaternion.identity);
            shadowPanal.transform.SetParent(canvas.transform, false);
            shadowPanal.GetComponent<Image>().sprite = shadowImages[panelChangeNum];
            clickBtn.gameObject.transform.SetParent(canvas.transform, true);
        }
    }

    public override void Execute(TutorialController controller)
    {
        // �ش� ��ư�� ������
        if (clickBtn.GetComponent<TutorialButton>().buttonClicked)
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

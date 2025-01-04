using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveSequenceDrag : InteractiveSequenceBase
{
    private GameObject arrow;
    private GameObject arrowPrefab;
    private GameObject canvas;
    private Transform startParent;

    [Header("ȭ��ǥ ���� ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;
    [SerializeField] private Sprite arrowImg;

    [Header("Ŭ��/�巡�� ���-�Է�")]
    [SerializeField] private GameObject dragObj; // Ŭ��/�巡�� ���

    public override void Enter()
    {
        canvas = GameObject.FindGameObjectWithTag("UI Canvas");

        if (highlightArrowOnOff)
        {
            // ȭ��ǥ ����
            arrowPrefab = Resources.Load<GameObject>("Prefabs/Tutorial/HighlightArrowPref");
            arrow = Instantiate(arrowPrefab, new Vector2(0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            arrow.GetComponent<Canvas>().worldCamera = Camera.main;
            GameObject arrowChild = arrow.transform.GetChild(0).gameObject;

            // ȭ��ǥ ��ġ ����
            arrowChild.GetComponent<Image>().sprite = arrowImg;
            startParent = dragObj.transform.parent;
            
            dragObj.transform.SetParent(arrow.transform);

            // Ÿ�� �θ� ����
            dragObj.GetComponent<InteractiveDragObj>().SetTargetParent(arrow.transform);

            // ȭ��ǥ ������ �ִϸ��̼� ���
            arrowChild.GetComponent<Animator>().enabled = true;
            arrowChild.GetComponent<Animator>().Play("blinkArrow");
        }
        else
        {
            startParent = dragObj.transform.parent;
            dragObj.GetComponent<InteractiveDragObj>().SetTargetParent(canvas.transform);
        }
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        // �ش� ������Ʈ�� �巡�� ������Ʈ���
        if (dragObj.GetComponent<InteractiveDragObj>() != null)
        {
            // �巡�װ� �Ϸ�Ǿ��ٸ�
            if (dragObj.GetComponent<InteractiveDragObj>().GetObjectDraged())
            {
                dragObj.transform.SetParent(startParent);
                tutorialPipeline.SetNextTutorial(SceneState.None); // ���� InteractiveSequence
            }
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        // �ش� ������Ʈ�� �巡�� ������Ʈ���
        if (dragObj.GetComponent<InteractiveDragObj>() != null)
        {
            // �巡�װ� �Ϸ�Ǿ��ٸ�
            if (dragObj.GetComponent<InteractiveDragObj>().GetObjectDraged())
            {
                Debug.Log("<color=red>�巡�� Ʃ�丮�� �Ϸ�</color>");
                dragObj.transform.SetParent(startParent);
                questActionPipeline.SetNextQuestAction(); // ���� �丮��
            }
        }
    }

    public override void Exit()
    {
        dragObj.GetComponent<InteractiveDragObj>().SetObjctDraged(false);
        Destroy(arrow); // ȭ��ǥ ����
    }
}

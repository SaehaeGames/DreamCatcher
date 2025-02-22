using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class InteractiveDragObj : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    // �÷��̾� ������
    private PlayerDataManager curPlayerData;   //�÷��̾� ������ ����
    private int curScene;
    public bool objectDraged;
    [SerializeField] private GameObject[] targets;
    private Transform startParent;
    private int numberOfTargets;

    // Start is called before the first frame update
    void Start()
    {
        // ���� �ʱ�ȭ
        objectDraged = false;
    }


    // �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        objectDraged = false; // �ʱ�ȭ

        // Tutorial ������Ʈ���� Ȱ��ȭ �Ǿ� ���� ������ �ڵ� ��Ȱ��ȭ
        if (startParent == null)
        {
            this.GetComponent<InteractiveDragObj>().enabled = false;
        }
        else
        {
            this.GetComponent<InteractiveDragObj>().enabled = true;
        }
    }

    public void SetTargetParent(Transform arrowTransform)
    {
        numberOfTargets = targets.Length;
        for (int i = 0; i < numberOfTargets; i++)
        {
            startParent = targets[i].transform.parent;
            targets[i].transform.SetParent(arrowTransform);
        }
    }

    public void SetObjctDraged(bool dragSet)
    {
        objectDraged = dragSet;
    }

    public bool GetObjectDraged()
    {
        return objectDraged;
    }

    // �巡�� ��
    public void OnEndDrag(PointerEventData eventData)
    {
        numberOfTargets = targets.Length;
        for (int i=0; i < numberOfTargets; i++) 
        {
            targets[i].transform.SetParent(startParent);
            Debug.Log("<color=red>�巡�� Ÿ�� ���ڸ��� ���ư� :  "+startParent+"</color>");
        }
        Debug.Log("<color=red>" + eventData.pointerCurrentRaycast.gameObject + "</color>");

        // �ùٸ� ���� �巡�� �Ǿ��ٸ�
        switch(numberOfTargets)
        {
            case 0:
                Debug.LogError("Ÿ���� �����ϴ�.");
                break;
            case 1:
                if (eventData.pointerCurrentRaycast.gameObject == targets[0])
                {
                    objectDraged = true; // �巡�� �Ϸ� ǥ��
                }
                break;
            case 2:
                if (eventData.pointerCurrentRaycast.gameObject == targets[0] || eventData.pointerCurrentRaycast.gameObject == targets[1])
                {
                    objectDraged = true; // �巡�� �Ϸ� ǥ��
                }
                break;
            case 3:
                if (eventData.pointerCurrentRaycast.gameObject == targets[0] || eventData.pointerCurrentRaycast.gameObject == targets[1] || eventData.pointerCurrentRaycast.gameObject == targets[2])
                {
                    objectDraged = true; // �巡�� �Ϸ� ǥ��
                }
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class TutorialDragObj : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool objectDraged;
    [SerializeField] private GameObject[] targets;
    private Transform startParent;
    private int numberOfTargets;

    // Start is called before the first frame update
    void Start()
    {
        objectDraged = false;
    }


    // �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        objectDraged = false;
        Debug.Log("�巡�� ����");
    }

    public void SetTargetParent(Transform arrowTransform)
    {
        numberOfTargets = targets.Length;
        Debug.Log("�θ� ���� ����"+numberOfTargets);
        for (int i = 0; i < numberOfTargets; i++)
        {
            startParent = targets[i].transform.parent;
            targets[i].transform.SetParent(arrowTransform);
            Debug.Log("�θ� ����");
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

    public void OnDrag(PointerEventData eventData)
    {

    }

    // �巡�� ��
    public void OnEndDrag(PointerEventData eventData)
    {
        for(int i=0; i < numberOfTargets; i++) 
        {
            targets[i].transform.SetParent(startParent);
        }

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
                    Debug.Log("�巡�� ����");
                }
                break;
            case 2:
                if (eventData.pointerCurrentRaycast.gameObject == targets[0] || eventData.pointerCurrentRaycast.gameObject == targets[1])
                {
                    objectDraged = true; // �巡�� �Ϸ� ǥ��
                    Debug.Log("�巡�� ����");
                }
                break;
            case 3:
                if (eventData.pointerCurrentRaycast.gameObject == targets[0] || eventData.pointerCurrentRaycast.gameObject == targets[1] || eventData.pointerCurrentRaycast.gameObject == targets[2])
                {
                    objectDraged = true; // �巡�� �Ϸ� ǥ��
                    Debug.Log("�巡�� ����");
                }
                break;
        }
    }
}

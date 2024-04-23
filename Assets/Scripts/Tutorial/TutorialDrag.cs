using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool objectDraged;
    [SerializeField] private GameObject target;

    // �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        objectDraged = false;
        Debug.Log("�巡�� ����");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("�巡�� ��");
    }

    // �巡�� ��
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("�巡�� ��");
        // �ùٸ� ���� �巡�� �Ǿ��ٸ�
        if (eventData.pointerCurrentRaycast.gameObject==target)
        {
            objectDraged = true; // �巡�� �Ϸ� ǥ��
            Debug.Log("�巡�� ����");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("�巡�� ������Ʈ");
        objectDraged = false;
    }
}

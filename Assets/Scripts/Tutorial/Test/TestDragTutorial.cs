using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestDragTutorial : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Vector2 defaultPosition;  // ����ϸ� �ٽ� ���� ����ġ ����    

    public void OnBeginDrag(PointerEventData eventData)
    {
        //�巡�׸� ������ ���� �Լ�
        defaultPosition = this.transform.position;  // ó�� ��ġ ����
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �巡�� ���� ���� �Լ�

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 5f;
        gameObject.transform.position = mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�� ������ ���� �Լ�
        this.transform.position = defaultPosition;      // ����ġ�� ���ư���
    }
}

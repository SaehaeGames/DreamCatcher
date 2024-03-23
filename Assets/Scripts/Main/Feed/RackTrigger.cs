using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RackTrigger : MonoBehaviour
{
    // ȶ�� Ʈ���� ������Ʈ�� ���� Ŭ����
    // �ڽ��� ȶ�� ������ ������ ���̿��� �浹 ������ ������

    [Header("[RackTrigger]")]
    [SerializeField] private int triggerNumber;   // �ڽ��� ȶ�� ���� ��ȣ ����

    private void Start()
    {
        Button _btn = this.GetComponent<Button>();  // ��ư ������Ʈ
        _btn.onClick.AddListener(() => GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedPanel>().OpenPanel(TriggerNumber)); // ȶ�� ���� ��ȣ�� �Ű������� ���� ��ư �̺�Ʈ �߰�
    }

    public int TriggerNumber
    {
        // ȶ�� ��ȣ ������Ƽ �Լ�
        // ȶ�� ��ȣ�� �����ϰų� ��ȯ��

        get { return triggerNumber; }
        set { triggerNumber = value; }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //���� ������Ʈ�� ����� �� 

        if (collision.gameObject.tag == "Feed")     // ���̿� ��Ҵٸ�
        {
            Debug.Log(collision.gameObject.GetComponent<FeedDrag>().FeedNumber + " �� ���̿� ����");

            collision.gameObject.GetComponent<FeedDrag>().SelectRackNumber = triggerNumber; // �ڽ��� ȶ�� ��ȣ ������
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //���� ������Ʈ���� �������� �� 

        if (collision.gameObject.tag == "Feed")     // ���̿� ��� �־��ٸ�
        {
            FeedDrag collisionFeed = collision.gameObject.GetComponent<FeedDrag>();
            if (!collisionFeed.IsDragging)   // �巡������ �ƴ϶�� ( == �巡�׸� ������ ȶ�븦 �����ߴٸ�)
            {
                int feedNumber = collisionFeed.FeedNumber;  // �浹�� ���� ��ȣ�� ������
                Debug.Log(feedNumber + " �� ���̿� ��Ҵ� ������");
                GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedManager>().SelectFeed(triggerNumber, feedNumber);  // ȶ�� ���� ���� �Լ� ����
            }
        }
    }
}

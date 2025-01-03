using UnityEngine;
using UnityEngine.UI;

public class RackTrigger : MonoBehaviour
{
    // ȶ�� Ʈ���� ������Ʈ�� ���� Ŭ����
    // �ڽ��� ȶ�� ������ ������ ���̿��� �浹 ������ ������

    [Header("[RackTrigger]")]
    [SerializeField] private int triggerNumber;         // �ڽ��� ȶ�� ��ȣ ����

    public int TriggerNumber
    {
        // ȶ�� ��ȣ ������Ƽ �Լ�
        // ȶ�� ��ȣ�� �����ϰų� ��ȯ��

        get => triggerNumber;
        set => triggerNumber = value;
    }

    private void Start()
    {
        Button button = GetComponent<Button>();      // ��ư ������Ʈ
        button.onClick.AddListener(() => FeedPanelOpen());
    }

    private void FeedPanelOpen()
    {
        var feedManager = GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedPanel>();
        feedManager.OpenPanel(TriggerNumber);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //���� ������Ʈ�� ����� �� 

        if (collision.CompareTag("Feed"))   // ���̿� ��Ҵٸ�
        {
            var feedDrag = collision.gameObject.GetComponent<FeedDrag>();
            feedDrag.LastRackNumber = triggerNumber;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //���� ������Ʈ���� �������� �� 

        if (collision.CompareTag("Feed"))     // ���̿� ��Ҵٰ� �������ٸ�
        {
            var feedDrag = collision.gameObject.GetComponent<FeedDrag>();
            if (!feedDrag.IsDragging)          // ���� �巡������ �ƴ϶�� (== �巡�׸� ������ ȶ�븦 �����ߴٸ�)
            {
                var feedManager = GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedManager>();
                feedManager.SelectFeed(triggerNumber, feedDrag.Feed);   // ȶ�� ���� ���� �Լ� ����
            }
        }
    }
}

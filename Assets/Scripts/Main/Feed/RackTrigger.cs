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
        if (!collision.CompareTag("Feed")) return;
        var feedDrag = FindDraggingFeedDrag(collision.gameObject);
        if (feedDrag != null)
            feedDrag.LastRackNumber = triggerNumber;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Feed")) return;
        var feedDrag = FindFeedDrag(collision.gameObject);
        if (feedDrag == null || feedDrag.LastRackNumber != triggerNumber) return;

        feedDrag.LastRackNumber = -1;
        if (feedDrag.IsDragging) return;

        var feedManager = GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedManager>();
        feedManager.SelectFeed(triggerNumber, feedDrag.Feed);
    }

    private FeedDrag FindDraggingFeedDrag(GameObject obj)
    {
        Transform t = obj.transform;
        while (t != null)
        {
            var fd = t.GetComponent<FeedDrag>();
            if (fd != null && fd.IsDragging) return fd;
            t = t.parent;
        }
        return null;
    }

    private FeedDrag FindFeedDrag(GameObject obj)
    {
        Transform t = obj.transform;
        while (t != null)
        {
            var fd = t.GetComponent<FeedDrag>();
            if (fd != null) return fd;
            t = t.parent;
        }
        return null;
    }
}

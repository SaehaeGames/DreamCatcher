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
        var feedDrag = FindDroppedFeedDrag(collision.gameObject, triggerNumber);
        if (feedDrag != null)
        {
            feedDrag.LastRackNumber = -1;
            var feedManager = GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedManager>();
            feedManager.SelectFeed(triggerNumber, feedDrag.Feed);
        }
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

    private FeedDrag FindDroppedFeedDrag(GameObject obj, int rackNum)
    {
        Transform t = obj.transform;
        while (t != null)
        {
            var fd = t.GetComponent<FeedDrag>();
            if (fd != null && !fd.IsDragging && fd.LastRackNumber == rackNum) return fd;
            t = t.parent;
        }
        return null;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class RackTrigger : MonoBehaviour
{
    // ȶ�� Ʈ���� ������Ʈ�� ���� Ŭ����
    // �ڽ��� ȶ�� ������ ������ ���̿��� �浹 ������ ������

    [Header("[RackTrigger]")]
    [SerializeField] private int triggerNumber;
    [SerializeField] private FeedPanel feedPanel;         // �ڽ��� ȶ�� ��ȣ ����

    public int TriggerNumber
    {
        // ȶ�� ��ȣ ������Ƽ �Լ�
        // ȶ�� ��ȣ�� �����ϰų� ��ȯ��

        get => triggerNumber;
        set => triggerNumber = value;
    }

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(FeedPanelOpen);
    }

    public void SetDependencies(FeedPanel panel)
    {
        feedPanel = panel;
    }

    private void FeedPanelOpen()
    {
        if (feedPanel == null)
        {
            GameObject managerObject = GameObject.FindGameObjectWithTag("FeedManager");
            if (managerObject != null) feedPanel = managerObject.GetComponent<FeedPanel>();
        }

        if (feedPanel != null) feedPanel.OpenPanel(TriggerNumber);
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

        FeedDrag feedDrag = FindFeedDrag(collision.gameObject);
        if (feedDrag != null && feedDrag.LastRackNumber == triggerNumber)
        {
            feedDrag.LastRackNumber = -1;
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

using UnityEngine;
using UnityEngine.UI;

public class RackTrigger : MonoBehaviour
{
    // 횃대 트리거 오브젝트에 넣을 클래스
    // 자신의 횃대 정보를 가지고 먹이와의 충돌 정보를 관리함

    [Header("[RackTrigger]")]
    [SerializeField] private int triggerNumber;         // 자신의 횃대 번호 변수

    public int TriggerNumber
    {
        // 횃대 번호 프로퍼티 함수
        // 횃대 번호를 설정하거나 반환함

        get => triggerNumber;
        set => triggerNumber = value;
    }

    private void Start()
    {
        Button button = GetComponent<Button>();      // 버튼 컴포넌트
        button.onClick.AddListener(() => FeedPanelOpen());
    }

    private void FeedPanelOpen()
    {
        var feedManager = GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedPanel>();
        feedManager.OpenPanel(TriggerNumber);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //먹이 오브젝트에 닿았을 때 

        if (collision.CompareTag("Feed"))   // 먹이와 닿았다면
        {
            var feedDrag = collision.gameObject.GetComponent<FeedDrag>();
            feedDrag.LastRackNumber = triggerNumber;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //먹이 오브젝트에서 떨어졌을 때 

        if (collision.CompareTag("Feed"))     // 먹이와 닿았다가 떨어졌다면
        {
            var feedDrag = collision.gameObject.GetComponent<FeedDrag>();
            if (!feedDrag.IsDragging)          // 현재 드래그중이 아니라면 (== 드래그를 끝내고 횃대를 선택했다면)
            {
                var feedManager = GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedManager>();
                feedManager.SelectFeed(triggerNumber, feedDrag.Feed);   // 횃대 먹이 선택 함수 실행
            }
        }
    }
}

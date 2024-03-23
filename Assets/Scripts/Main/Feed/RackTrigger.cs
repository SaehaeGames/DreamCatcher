using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RackTrigger : MonoBehaviour
{
    // 횃대 트리거 오브젝트에 넣을 클래스
    // 자신의 횃대 정보를 가지고 먹이와의 충돌 정보를 관리함

    [Header("[RackTrigger]")]
    [SerializeField] private int triggerNumber;   // 자신의 횃대 고유 번호 변수

    private void Start()
    {
        Button _btn = this.GetComponent<Button>();  // 버튼 컴포넌트
        _btn.onClick.AddListener(() => GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedPanel>().OpenPanel(TriggerNumber)); // 횃대 고유 번호를 매개변수로 넣은 버튼 이벤트 추가
    }

    public int TriggerNumber
    {
        // 횃대 번호 프로퍼티 함수
        // 횃대 번호를 설정하거나 반환함

        get { return triggerNumber; }
        set { triggerNumber = value; }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //먹이 오브젝트에 닿았을 때 

        if (collision.gameObject.tag == "Feed")     // 먹이와 닿았다면
        {
            Debug.Log(collision.gameObject.GetComponent<FeedDrag>().FeedNumber + " 번 먹이와 닿음");

            collision.gameObject.GetComponent<FeedDrag>().SelectRackNumber = triggerNumber; // 자신의 횃대 번호 전달함
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //먹이 오브젝트에서 떨어졌을 때 

        if (collision.gameObject.tag == "Feed")     // 먹이와 닿고 있었다면
        {
            FeedDrag collisionFeed = collision.gameObject.GetComponent<FeedDrag>();
            if (!collisionFeed.IsDragging)   // 드래그중이 아니라면 ( == 드래그를 끝내고 횃대를 선택했다면)
            {
                int feedNumber = collisionFeed.FeedNumber;  // 충돌한 먹이 번호를 가져옴
                Debug.Log(feedNumber + " 번 먹이와 닿았다 떨어짐");
                GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedManager>().SelectFeed(triggerNumber, feedNumber);  // 횃대 먹이 선택 함수 실행
            }
        }
    }
}

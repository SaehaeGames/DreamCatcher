using UnityEngine;
using UnityEngine.UI;

public class BirdTouch : MonoBehaviour
{
    // 새(Bird) 오브젝트에 컴포넌트로 넣을 클래스

    [Header("[RackBird]")]
    [SerializeField] private int birdIdx;   // 새 고유 인덱스 변수 (새 종류 번호가 아닌, 몇 번째 횃대의 새인지)

    private void Start()
    {
        Button _btn = this.GetComponent<Button>();  // 버튼 컴포넌트
        _btn.onClick.AddListener(() => TouchBirdEvent()); // 새 클릭 버튼 이벤트 추가
    }

    public int BirdIdx
    {
        // 새 인덱스 프로퍼티 함수
        // 새 인덱스를 설정하거나 반환함

        get { return birdIdx; }
        set { birdIdx = value; }
    }

    public void TouchBirdEvent()
    {
        // 새 터치 이벤트 함수
        // 자신의 번호를 매개변수로 깃털 수확 함수 실행

        GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedManager>().TouchBirdGetFeather(birdIdx);
    }
}

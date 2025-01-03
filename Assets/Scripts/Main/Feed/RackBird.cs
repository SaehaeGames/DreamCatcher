using UnityEngine;
using UnityEngine.UI;

public class RackBird : MonoBehaviour
{
    // 횃대에 나타난 새 클래스
    [SerializeField] private int rackNumber;   // 횃대 번호
    [SerializeField] private int birdNumber;   // 새 번호

    public int RackNumber
    {
        get => rackNumber;
        set => rackNumber = value;
    }

    public int BirdNumber
    {
        get =>  birdNumber;
        set => birdNumber = value;
    }

    private void Start()
    {
        Button button = GetComponent<Button>();     // 버튼 컴포넌트
        button.onClick.AddListener(BirdTouchEvent);      // 새 클릭 버튼 이벤트 추가
    }

    private void OnEnable()
    {
        // 활성화되면 자신의 횃대 인덱스로 새 번호를 설정

        birdNumber = GameManager.instance.rackDataList[rackNumber].birdNumber; 
    }

    public void BirdTouchEvent()
    {
        // 새 터치 이벤트 함수

        GameObject.FindGameObjectWithTag("FeedManager").GetComponent<FeedManager>().TouchBirdGetFeather(rackNumber, birdNumber);
    }
}

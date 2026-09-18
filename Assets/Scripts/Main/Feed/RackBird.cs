using UnityEngine;
using UnityEngine.UI;

public class RackBird : MonoBehaviour
{
    // 횃대에 나타난 새 클래스
    [SerializeField] private int rackNumber;   // 횃대 번호
    [SerializeField] private int birdNumber;   // 새 번호
    [SerializeField] private FeedManager feedManager;

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
        if (button != null) button.onClick.AddListener(BirdTouchEvent);      // 새 클릭 버튼 이벤트 추가
    }

    private void OnEnable()
    {
        // 활성화되면 자신의 횃대 인덱스로 새 번호를 설정

        RefreshBirdNumber();
    }

    public void BirdTouchEvent()
    {
        // 새 터치 이벤트 함수

        if (feedManager == null)
        {
            GameObject managerObject = GameObject.FindGameObjectWithTag("FeedManager");
            if (managerObject != null) feedManager = managerObject.GetComponent<FeedManager>();
        }

        if (feedManager != null) feedManager.TouchBirdGetFeather(rackNumber, birdNumber);
    }

    public void SetDependencies(int index, FeedManager manager)
    {
        rackNumber = index;
        feedManager = manager;
        RefreshBirdNumber();
    }

    private void RefreshBirdNumber()
    {
        if (GameManager.instance == null || GameManager.instance.rackDataList == null
            || rackNumber < 0 || rackNumber >= GameManager.instance.rackDataList.Count)
        {
            return;
        }

        birdNumber = GameManager.instance.rackDataList[rackNumber].birdNumber;
    }
}

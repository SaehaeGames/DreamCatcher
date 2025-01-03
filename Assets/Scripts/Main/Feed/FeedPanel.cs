using System.Collections.Generic;
using UnityEngine;

public class FeedPanel : MonoBehaviour
{
    // 먹이 패널 클래스
    // 먹이 선택창, 특제 먹이 사용창을 열고 닫음

    [Header("[Feed Panel]")]
    public GameObject Feed_Panel;               // 먹이 선택 패널
    public GameObject SpecialFeed_Panel;        // 특제 먹이 사용 패널

    public void OpenPanel(int triggerNumber)
    {
        // 조건에 따라 먹이 선택 패널 또는 특제 먹이 패널을 여는 함수
        // 클릭한 횃대에 먹이가 없고, 새가 등장하지 않음 -> 먹이 패널 오픈
        // 클릭한 횃대에 먹이가 없고, 새가 등장함 -> 새 깃털 수확
        // 클릭한 횃대에 먹이가 있고, 새가 등장 -> 해당 경우 없음 (새가 등장하면 먹이가 없어져야 함)
        // 클릭한 횃대에 먹이가 있고, 새가 등장하지 않음 -> 특제 먹이 패널 오픈

        List<RackData> dataList = GameManager.instance.rackDataList;                    // 저장된 새 데이터를 가져옴

        if (dataList.Count == 0 || (!dataList[triggerNumber].isFed && !dataList[triggerNumber].isAppeared))
            SetFeedPanelActive(true);         // 먹이 선택 패널 오픈
        else if (dataList[triggerNumber].isFed && !dataList[triggerNumber].isAppeared)
            SetSpecialFeedPanelActive(true);  // 특제 먹이 패널 오픈
    }

    public void SetFeedPanelActive(bool isActive)
    {
        // 먹이 선택 패널을 열고 닫는 함수

        Feed_Panel.SetActive(isActive);
    }

    public void SetSpecialFeedPanelActive(bool TorF)
    {
        // 특제 먹이 패널을 열고 닫는 함수

        SpecialFeed_Panel.SetActive(TorF);
    }
}

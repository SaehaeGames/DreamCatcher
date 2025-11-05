using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialFeed : MonoBehaviour
{
    //스페셜 먹이 개수를 관리하고 사용하는 클래스

    [Header("[Special Feed]")]
    [SerializeField] private int feedCount;    //특제 먹이 개수
    [SerializeField] private int selectCount;    //선택한 먹이 수
    [SerializeField] private float decreaseTime;   //감소시키는 시간

    [SerializeField] private PlayerDataManager playerDataManager;   //상품 정보

    [Space]
    [Header("[Feed Text]")]
    [SerializeField] private Text countText;      //먹이 개수 텍스트

    void Start()
    {
        playerDataManager = GameManager.instance.playerDataManager;    //플레이어의 상단바 데이터 정보를 가져옴

        feedCount = (int)playerDataManager.GetPlayerDataByDataName(Constants.PlayerData_SpecialFeed).dataNumber;  //특제 먹이 개수를 가져옴
        decreaseTime = 300;   //특제먹이 감소 시간
        selectCount = 0;
        UpdateCountText(selectCount);
    }

    public void LeftButton()
    {
        //특제 먹이 선택 패널에서 선택 개수 감소 함수

        if (selectCount == 0)   //0개에서 감소 버튼을 누르면
        {
            //바로 최대로 사용 가능한 개수로 변경
            float leftTime = this.gameObject.GetComponent<FeedTimer>().GetLeftTime(0);    //먹이 남은 시간
            float maxCount = leftTime / decreaseTime + 1.0f; //남은 시간 내 최대로 사용 가능한 특제 먹이 수
            selectCount = Mathf.FloorToInt(maxCount); //특제 먹이 사용 개수를 최대치로 변경
        }
        else
        {
            selectCount--;
        }

        UpdateCountText(selectCount);
    }

    public void RightButton()
    {
        //특제 먹이 선택 패널에서 선택 개수 증가 함수

        if (selectCount < feedCount)
        {
            selectCount++;
            float leftTime = this.gameObject.GetComponent<FeedTimer>().GetLeftTime(0);    //먹이 남은 시간
            float maxCount = leftTime / decreaseTime + 1.0f; //남은 시간 내 최대로 사용 가능한 특제 먹이 수
            selectCount = selectCount > maxCount ? (int)maxCount : selectCount; //특제 먹이 사용 개수가 시간 내 최대 사용 가능 개수를 넘지 않도록 조정


            UpdateCountText(selectCount);
        }
    }

    public void selectSpecialFeed(int rackIndex)
    {
        //특제먹이 사용 함수

        if (feedCount >= selectCount)
        {
            //특제 먹이 사용
            // rackIndex에 사용되도록 구현하기

            float decrease = selectCount * decreaseTime;     // 특제 먹이 사용으로 감소하는 시간 계산
            int rackLevel = GameManager.instance.goodsDataManager.GetValidatedGoodsData(Constants.GoodsData_Rack).level;

            GetComponent<FeedTimer>().DecreaseFeedingTime(rackIndex, decrease);     // 선택한 횃대에 특제 먹이 사용(남은 시간 감소)
            feedCount -= selectCount;    //특제 먹이 개수 갱신
            selectCount = 0;        //선택한 먹이 개수 갱신

            // UI 업데이트 및 저장
            UpdateCountText(selectCount);
            GameObject.FindGameObjectWithTag(Constants.Tag_TopBar).GetComponent<TopBarText>().UpdateText();

            GameManager.instance.playerDataManager.GetPlayerDataByDataName(Constants.PlayerData_SpecialFeed).dataNumber = feedCount;
            GameManager.instance.jsonManager.SaveData(Constants.PlayerDataFile, playerDataManager);
            
            this.GetComponent<FeedPanel>().SetSpecialFeedPanelActive(false);  // 패널 닫기
        }
    }
    private void UpdateCountText(int count)
    {
        // 먹이 개수 텍스트 업데이트

        countText.text = count + " 개";
    }
}

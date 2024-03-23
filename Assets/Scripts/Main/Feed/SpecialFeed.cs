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

    [SerializeField] private PlayerDataContainer curPlayerData;   //상품 정보

    [Space]
    [Header("[Feed Text]")]
    [SerializeField] private Text countText;      //먹이 개수 텍스트

    void Start()
    {
        curPlayerData = GameManager.instance.loadPlayerData;    //플레이어의 상단바 데이터 정보를 가져옴
        feedCount = (int)curPlayerData.dataList[2].dataNumber;  //특제 먹이 개수를 가져옴
        selectCount = 0;
        decreaseTime = 300;   //특제먹이 감소 시간

        countText.text = selectCount + " 개";
    }

    public void LeftButton()
    {
        //특제 먹이 선택 패널에서 선택 개수 감소 함수

        if (selectCount == 0)   //0개에서 감소 버튼을 누르면
        {
            //바로 최대로 사용 가능한 개수로 변경
            float leftTime = this.gameObject.GetComponent<FeedTimer>().GetLeftTime(0);    //먹이 남은 시간
            float maxCount = leftTime / decreaseTime + 1.0f; //남은 시간 내 최대로 사용 가능한 특제 먹이 수
            selectCount = (int)maxCount; //특제 먹이 사용 개수를 최대치로 변경

            countText.text = selectCount + " 개";
        }
        else
        {
            selectCount--;
            countText.text = selectCount + " 개";
        }
    }

    public void RightButton()
    {
        //특제 먹이 선택 패널에서 선택 개수 증가 함수

        if (selectCount < feedCount)
        {
            selectCount++;

            //특제 먹이 사용 개수 수정

            float leftTime = this.gameObject.GetComponent<FeedTimer>().GetLeftTime(0);    //먹이 남은 시간
            float maxCount = leftTime / decreaseTime + 1.0f; //남은 시간 내 최대로 사용 가능한 특제 먹이 수
            selectCount = selectCount > maxCount ? (int)maxCount : selectCount; //특제 먹이 사용 개수가 시간 내 최대 사용 가능 개수를 넘지 않도록 조정


            countText.text = selectCount + " 개";
        }
    }

    public void selectSpecialFeed()
    {
        //특제먹이 사용 함수
        curPlayerData = GameManager.instance.loadPlayerData;

        if (feedCount >= selectCount)
        {
            //특제 먹이 사용
            //int birdInx = this.gameObject.GetComponent<FeedTimer>().curbirdIdx;    //시간 감소시키려는 새의 인덱스
            float decrease = selectCount * decreaseTime;     //특제 먹이 사용으로 감소하는 시간 계산

            int rackLevel = GameManager.instance.loadGoodsData.goodsList[0].goodsLevel; // 현재 플레이어의 횃대 레벨
            for (int i = 0; i < rackLevel + 1; i++)
            {
                this.gameObject.GetComponent<FeedTimer>().DecreaseFeedingTime(i, decrease);     //특제 먹이 사용(남은 시간 감소)
            }

            feedCount = feedCount - selectCount;    //특제 먹이 개수 갱신
            curPlayerData.dataList[2].dataNumber = feedCount;
            GameManager.instance.GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //특제먹이 수 변경사항 json으로 저장
            GameObject.FindGameObjectWithTag("TopBar").GetComponent<TopBarText>().UpdateText();   //상단바 업데이트

            selectCount = 0;        //선택한 먹이 개수 갱신
            countText.text = selectCount + " 개";

            GameObject.FindGameObjectWithTag("TopBar").GetComponent<TopBarText>().SetSpecialFeedText(feedCount);   //상단바 특제 먹이 개수 갱신
            this.GetComponent<FeedPanel>().OpenOrCloseSpecialFeedPanel(false);  // 패널 닫기
        }
    }
}

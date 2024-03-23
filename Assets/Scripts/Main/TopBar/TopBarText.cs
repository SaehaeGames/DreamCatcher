using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBarText : MonoBehaviour
{
    //상단바 데이터 텍스트를 보여주는 클래스d

    [Header("[Topbar Text]")]
    public Text DreamMarbleText;      //꿈구슬 텍스트
    public Text GoldText;      //골드 텍스트
    public Text SpecialFeedText;      //먹이 개수 텍스트

    private PlayerDataContainer curPlayerData;   //플레이어 데이터 정보

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        //텍스트들을 업데이트 하는 함수

        curPlayerData = GameManager.instance.loadPlayerData;    //플레이어의 상단바 데이터 정보를 가져옴

        if (DreamMarbleText.IsActive())
        {
            //만약 텍스트가 숨겨져있지않으면(스타트 화면은 숨겨져있음)
            DreamMarbleText.text = curPlayerData.dataList[0].dataNumber.ToString();
            GoldText.text = curPlayerData.dataList[1].dataNumber.ToString();    //특제 먹이 개수를 가져옴
            SpecialFeedText.text = curPlayerData.dataList[2].dataNumber.ToString();    //특제 먹이 개수를 가져옴
        }
    }

    public void SetDreamMarbleText(int marbleNum)
    {
        //꿈구슬 텍스트를 수정하는 함수

        DreamMarbleText.text = marbleNum.ToString();
    }

    public void SetSpecialFeedText(int feedNum)
    {
        //특제 먹이 개수 텍스트를 수정하는 함수

        SpecialFeedText.text = feedNum.ToString();
    }

    public void SetGoldText(int GoldNum)
    {
        //골드 텍스트를 수정하는 함수

        GoldText.text = GoldNum.ToString();
    }
}

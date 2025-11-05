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

    private PlayerDataManager playerDataManager;

    private void Start()
    {
        playerDataManager = GameManager.instance.playerDataManager;
        UpdateText();
    }

    public void UpdateText()
    {
        //플레이어 데이터 정보 텍스트들을 업데이트 하는 함수

        if (DreamMarbleText.IsActive())
        {
            //만약 텍스트가 숨겨져있지않으면(스타트 화면은 숨겨져있음)
            SetDreamMarbleText((int)playerDataManager.GetPlayerDataByDataName(Constants.PlayerData_DreamMarble).dataNumber);
            SetGoldText((int)playerDataManager.GetPlayerDataByDataName(Constants.PlayerData_Gold).dataNumber);
            SetSpecialFeedText((int)playerDataManager.GetPlayerDataByDataName(Constants.PlayerData_SpecialFeed).dataNumber);
        }
    }

    private void SetDreamMarbleText(int marbleNum)
    {
        //꿈구슬 텍스트를 수정하는 함수

        DreamMarbleText.text = marbleNum.ToString();
    }

    private void SetGoldText(int GoldNum)
    {
        //골드 텍스트를 수정하는 함수

        GoldText.text = GoldNum.ToString();
    }

    private void SetSpecialFeedText(int feedNum)
    {
        //특제 먹이 개수 텍스트를 수정하는 함수

        SpecialFeedText.text = feedNum.ToString();
    }
}

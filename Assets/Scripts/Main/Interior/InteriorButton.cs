using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteriorButton : MonoBehaviour
{
    // 인테리어 카테고리, 아이템 버튼이 가지고 있는 스크립트

    public int buttonNumber;   //패널 정보
    public int itemID;  // 버튼에 해당하는 아이템 id


    public void SettingInteriorFunction()
    {
        // 인테리어 패널 클릭 버튼 이벤트를 설정하는 함수

        this.gameObject.GetComponent<Button>().onClick.AddListener(SelectInteriorButton);
    }

    public void SettingInteriorItemFunction()
    {
        // 인테리어 아이템 클릭 버튼 이벤트를 설정하는 함수

        this.gameObject.GetComponent<Button>().onClick.AddListener(SelectInteriorItemButton);
    }

    public void SelectInteriorButton()
    {
        //인테리어 패널 버튼 클릭 함수

        GameObject.FindGameObjectWithTag("GoodsManager").GetComponent<InteriorCategory>().UpdateCatrgoryPanel(buttonNumber);   //현재 버튼 번호를 노출되는 카테고리를 설정
    }

    public void SelectInteriorItemButton()
    {
        //인테리어 아이템 클릭 함수
        GameObject.FindGameObjectWithTag("GoodsManager").GetComponent<InteriorCategory>().SelectInteriorItem(buttonNumber, itemID);   //현재 버튼으로 인테리어 아이템 이미지 변경
    }

    public void SetButtonNumber(int num)
    {
        buttonNumber = num;
    }

    public void SetButtonItemID(int num)
    {
        itemID = num;
    }
}

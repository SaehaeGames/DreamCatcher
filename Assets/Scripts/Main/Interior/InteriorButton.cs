using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteriorButton : MonoBehaviour
{
    public int buttonNumber;   // 패널 번호 또는 아이템 번호
    public int itemID;         // 해당 버튼의 아이템 ID

    private Button buttonComponent;

    private void Awake()
    {
        buttonComponent = GetComponent<Button>();
    }

    /// <summary>
    /// 인테리어 패널 버튼 설정
    /// </summary>
    public void SettingInteriorFunction()
    {
        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(SelectInteriorItemButton);
    }

    /// <summary>
    /// 인테리어 아이템 버튼 설정
    /// </summary>
    public void SetupItemButton(int number, int id)
    {
        buttonNumber = number;
        itemID = id;

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(() => SelectInteriorItemButton());
    }

/*    public void SelectInteriorButton()
    {
        GameObject.FindGameObjectWithTag("GoodsManager")
            .GetComponent<InteriorCategory>()
            .UpdateCatrgoryPanel(buttonNumber);
    }*/

    public void SelectInteriorItemButton()
    {
        GameObject.FindGameObjectWithTag("GoodsManager")
            .GetComponent<InteriorCategory>()
            .SelectInteriorDafaultItem(buttonNumber, itemID);
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
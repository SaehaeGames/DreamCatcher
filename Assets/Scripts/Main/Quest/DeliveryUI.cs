using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUI : MonoBehaviour
{
    //퀘스트 납품 창 스크립트
    private static string fileName = "Inventory";   //파일 이름
    public int currentNum = 0;
    
    public Text itemName;
    public Text itemContents;

    public GameObject questObject;
    public GameObject checkObject;  

    //가지고 있는 드림캐쳐 나오고(없을 경우 없다는 텍스트)
    //옆으로 넘기면 드림캐쳐 바뀜
    public void LeftButton()
    {
        List<Dictionary<string, object>> data_Inventory = CSVParser.ReadFromFile(fileName);  //인벤토리 데이터를 가져옴  
        //현재 드림캐쳐
        currentNum--;

        //이미지 다 바뀜
        itemName.text = data_Inventory[currentNum + 16]["name"].ToString();
        itemContents.text = data_Inventory[currentNum + 16]["name"].ToString();
    }

    public void RightButton()
    {
        List<Dictionary<string, object>> data_Inventory = CSVParser.ReadFromFile(fileName);  //인벤토리 데이터를 가져옴  

        currentNum++;

        itemName.text = data_Inventory[currentNum + 16]["name"].ToString();
        itemContents.text = data_Inventory[currentNum + 16]["name"].ToString();
    }

    public void YesButton()
    {
        // 납품 버튼을 눌렀을 때

        // 일치하는지 확인은 나중에 구현
        // 만약 일치한다면 (일단 무조건 일치하게 ㄱㄱ)
        questObject.GetComponent<QuestContents>().curQuestNumber++;
        questObject.gameObject.SetActive(false);
        checkObject.gameObject.SetActive(false);

    }
}

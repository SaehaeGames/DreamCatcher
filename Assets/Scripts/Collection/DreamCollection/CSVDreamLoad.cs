using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CSVDreamLoad : MonoBehaviour
{
    // 패널
    public GameObject DreamInfoPanel;

    // UI 오브젝트
    public GameObject[] dreamColImg = new GameObject[24];
    public Text[] dreamColName = new Text[24];

    public Text nameTxt;
    public Image dreamCatcherImg, lineImg, featherImg1, featherImg2, featherImg3;

    // 이미지
    public Sprite[] dreamImgs = new Sprite[24]; // 꿈 아이콘 이미지
    public Sprite[] dreamCatcherImgs = new Sprite[24]; // 드림캐쳐 이미지
    public Sprite[] lineImgs = new Sprite[5]; // 실 이미지
    public Sprite[] featherImgs = new Sprite[12]; // 깃털 이미지

    // 기타
    private string[] featherNames= { "멧비둘기", "염주비둘기", "공작비둘기", "딱새", "동고비", 
        "유리딱새", "청딱따구리", "까막딱따구리", "오색딱따구리", "올빼미", "매", "흰머리수리"};

    // 데이터 테이블
    public DreamInfo_Data _dreaminfo_data;

    // Start is called before the first frame update
    private void Start()
    {
        DreamInfoPanel.SetActive(false);
        LoadDreamCollection();
    }
    public void LoadDreamCollection()
    {
        for (int i = 0; i < 24; i++)
        {
            dreamColImg[i].GetComponent<Image>().sprite = dreamImgs[i];
            dreamColName[i].GetComponent<Text>().text = _dreaminfo_data.dataList[i].name;
        }
    }

    public void LoadDreamInfo(int index)
    {
        // 텍스트 설정(이름)
        nameTxt.text= _dreaminfo_data.dataList[index].name; // 이름

        // 이미지 설정(드림캐쳐, 실, 깃털)
        dreamCatcherImg.sprite = dreamCatcherImgs[index]; // 드림캐쳐
        // 실
        string lineColor = _dreaminfo_data.dataList[index].line;
        if (lineColor == "w")
        {
            lineImg.sprite = lineImgs[0];
        }
        else if(lineColor == "y")
        {
            lineImg.sprite = lineImgs[1];
        }
        else if(lineColor == "b")
        {
            lineImg.sprite = lineImgs[2];
        }
        else if(lineColor == "r")
        {
            lineImg.sprite = lineImgs[3];
        }
        else
        {
            lineImg.sprite = lineImgs[4];
        }
        // 깃털
        for (int i = 0; i < 12; i++)
        {
            if (_dreaminfo_data.dataList[index].feather1 == featherNames[i])
            {
                featherImg1.sprite = featherImgs[i];
            }

            if (_dreaminfo_data.dataList[index].feather2 == featherNames[i])
            {
                featherImg2.sprite = featherImgs[i];
            }

            if (_dreaminfo_data.dataList[index].feather3 == featherNames[i])
            {
                featherImg3.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                featherImg3.sprite = featherImgs[i];
            }
            if (_dreaminfo_data.dataList[index].feather3 == "없음")
            {
                featherImg3.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 꿈 개별 정보 패널 열기
    public void DreamInfo(int index)
    {
        DreamInfoPanel.SetActive(true);
        LoadDreamInfo(index);
    }

    // 꿈 개별 정보 패널 닫기
    public void DreamInfoBack()
    {
        DreamInfoPanel.SetActive(false);
    }

    // 새 도감으로 가기
    public void GoBirdColl()
    {
        SceneManager.LoadScene("CollectionBook");
    }
}

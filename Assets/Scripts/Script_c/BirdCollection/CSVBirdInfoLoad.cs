using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CSVBirdInfoLoad : MonoBehaviour
{
    // 패널
    public GameObject BirdInfoPanel;

    // UI 오브젝트
    public GameObject[] birdColImg = new GameObject[16];
    public Text[] birdColName = new Text[16];

    public Text numberTxt, nameTxt, expTxt, timeTxt;
    public Image foodImg, birdImg, featherImg;
    public Slider numberSlide;

    // 이미지
    public Sprite[] birdImgs = new Sprite[16];
    public Sprite[] foodImgs = new Sprite[4];
    public Sprite[] featherImgs = new Sprite[16];

    //json 관련
    private static string fileName = "FeatherNumInfo";
    //List<MyFeatherNumber> featherData = new List<MyFeatherNumber>();
    MyFeatherNumber featherData;

    //데이터 테이블 관련
    public BirdInfo_Data _birdinfo_data;

    private void Start()
    {
        featherData = GameManager.instance.loadFeatherData;
        BirdInfoPanel.SetActive(false);
        LoadBirdCollection();
    }

    // 새 전체 appear 정보 표시
    public void LoadBirdCollection()
    {
        for(int i=0; i<16; i++)
        {
            birdColImg[i].GetComponent<Image>().sprite = birdImgs[i];
            // 새가 등장한 적 없다면
            if (featherData.featherList[i].appear==0)
            {
                birdColImg[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f); // 회색 처리
                birdColImg[i].GetComponent<Button>().interactable = false; // 버튼 비활성화 처리
                birdColName[i].GetComponent<Text>().text = "???"; // 이름 물음표 처리
            }
            else // 새가 등장한 적이 있다면
            {
                birdColName[i].GetComponent<Text>().text = _birdinfo_data.datalist[i].name;
                birdColImg[i].GetComponent<Button>().interactable = true;
            }
        }
    }

    // 새 개별 정보 로드 : 새를 클릭했을 시 새의 개별 정보창에 데이터를 표시한다.
    public void LoadBirdInfo(int index)
    {
        // 텍스트 설정
        numberTxt.text = "획득 수 "+ featherData.featherList[index].feather_number+"마리";
        nameTxt.text = _birdinfo_data.datalist[index].name;
        int startTime = _birdinfo_data.datalist[index].starttime;
        int endTime = _birdinfo_data.datalist[index].endtime;
        //텍스트 설정(설명)
        string expContent = _birdinfo_data.datalist[index].exp;
        expTxt.text = expContent.Replace("nn", "\n");
        // 텍스트 설정(등장시간)
        if (startTime < 3600)
        {
            if (startTime == endTime)
            {

                timeTxt.text = startTime / 60 + "분";
            }
            else
            {
                timeTxt.text = startTime / 60 + "분~" + endTime / 60 + "분";
            }
        }
        else
        {
            if (startTime == endTime)
            {

                timeTxt.text = startTime / 3600 + "시간";
            }
            else
            {
                timeTxt.text = startTime / 3600 + "시간~" + endTime / 3600 + "시간";
            }
        }
        

        // 슬라이더 설정
        numberSlide.maxValue = _birdinfo_data.datalist[index].maxnum;
        numberSlide.value = featherData.featherList[index].feather_number;

        // 이미지 설정
        foodImg.sprite = foodImgs[index/4];
        birdImg.sprite = birdImgs[index];
        featherImg.sprite = featherImgs[index];
    }    

    // 새 개별 정보 패널 열기
    public void BirdInfo(int index)
    {
        BirdInfoPanel.SetActive(true);
        LoadBirdInfo(index);
    }

    // 새 개별 정보 패널 닫기
    public void BirdInfoBack()
    {
        BirdInfoPanel.SetActive(false);
    }

    // 꿈 도감으로 가기
    public void GoDreamCollection()
    {
        SceneManager.LoadScene("CollectionDream");
    }

    private static string getPath(string fileName)
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Saves/" + fileName + ".json";
#elif UNITY_ANDROID
        return Application.persistentDataPath+ fileName + ".json";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+ fileName + ".json";
#else
        return Application.dataPath +"/"+fileName + ".json";
#endif
    }
}

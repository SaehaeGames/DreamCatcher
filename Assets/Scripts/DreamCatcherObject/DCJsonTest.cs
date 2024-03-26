using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DCJsonTest : MonoBehaviour
{
    public Text mytext;
    public MyDreamCatcher dreamCatcherData; //MyDreamCatcher 객체 필요
    public int[] DCline = new int[64];
    public bool[] DCbead = new bool[48];

    private void OnEnable()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        dreamCatcherData = GameManager.instance.loadDreamCatcherData; //MyDreamCatcher 객체 GameManager에서 가져옴
        //시작
        mytext.text = "시작\n"; //텍스트 확인용

        // 드림캐쳐 임의 데이터 저장 테스트
        //dreamCatcherData.dreamCatcherList.Add(new DreamCatcher(DCline, DCbead, 2, 3, 2, 1));
        GameManager.instance.GetComponent<DreamCatcherDataManager>().DataSaveText(dreamCatcherData); //세이브
        mytext.text += "저장 : " + dreamCatcherData.dreamCatcherList[0].GetColor()+'\n'; //텍스트 확인용

        //드림캐쳐 임의 데이터 불러오기
        GameManager.instance.GetComponent<DreamCatcherDataManager>().DataLoadText<MyDreamCatcher>();
        dreamCatcherData = GameManager.instance.loadDreamCatcherData;
        mytext.text += "로드 : " + dreamCatcherData.GetDreamCatcherData(0).GetLine()[3] + '\n'; //텍스트 확인용
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

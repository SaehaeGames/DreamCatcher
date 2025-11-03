using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using UnityEngine.UI;
using System;

public class DCCheckManager : MonoBehaviour
{
    List<DreamCatcher> data = new List<DreamCatcher>();

    // 드림캐쳐 관련 데이터
    private int[,] DreamCatcherMap = new int[8, 8];
    private bool[] BeadMap = new bool[48];
    private int[] featherMap = new int[3];
    public int fNum = 0;
    private int colorMap = 0;

    // 색상정보
    private string[] colorDic = { "흰색", "노란색", "파란색", "빨간색", "검은색" };

    public UnityAction gemActive;
    public UnityAction<int> beadActive;

    //public BirdCSV birdCSV;

    // 보석
    public GameObject gem;

    // 행포인트
    public GameObject hangPoints;
    public bool hangPntChecker;

    //json 파일관련
    public MyFeatherNumber FNDManager;
    public MyDreamCatcher dreamCatcherData; //MyDreamCatcher 객체 필요

    //스프레드시트 관련
    public BirdInfo_Data _birdinfo_data;

    public GameObject completeDCImg;
    private DreamCatcher myDreamCatcher;

    //(임시)드림캐쳐 레벨 도감 제작용
    //public DreamCatcher colDreamCatcher;
    //(임시)테스트용
    public Text testTxt;

    // Start is called before the first frame update
    void Start()
    {
        //birdCSV = GameObject.FindWithTag("GameManager").gameObject.GetComponent<BirdCSV>();
        dreamCatcherData = GameManager.instance.dreamCatcherDataManager; //MyDreamCatcher 객체 GameManager에서 가져옴
        hangPntChecker = false;
        gem.SetActive(false);
        hangPoints.SetActive(false);
        ResetDeamCatcher();
        testTxt.text += "dreamCatcherCnt: " + dreamCatcherData.dreamCatcherCnt;
    }

    // 드림캐쳐 배열변형(2D->1D) 함수
    // : 드림캐쳐의 실모양을 나타내는 DCLine을 2차원배열에서 1차원배열로 변환하는 함수
    public int[] ConvertLineArrayTo1D(int[,] line2d)
    {
        int index = 0;
        int[] line1d = new int[64];
        // 매개변수인 dcline2d를 1차원 배열로 변환
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                line1d[index] = line2d[i, j];
                index++;
            }
        }
        return line1d;
    }

    // 드림캐쳐 라인 연결 상태 확인
    // : startNum과 endNum을 매개변수로 받아 연결되었는지 확인한다.
    public bool CheckDreamCatcher(int startNum, int endNum)
    {
        // 연결되어 있지 않을 때
        if (DreamCatcherMap[startNum, endNum] == 0)
        {
            return false;
        } // 연결되어 있을 때
        else { return true; }
    }

    // 드림캐쳐 상태기록 및 보석 활성화 위치 설정
    public void UpdateDreamCatcher(int startNum, int endNum)
    {
        if (DreamCatcherMap[startNum, endNum] == 0)
        {
            // 상태 업데이트
            DreamCatcherMap[startNum, endNum] = 1;
            DreamCatcherMap[endNum, startNum] = 1;

            // 구슬 활성화 업데이트
            BeadActive();
            // HangPoint 활성화 업데이트
            HangPointActive();
        }
    }

    // 행포인트 상태기록
    public void UpdateHangPnt(int hangPntNum, int lineStartNum, int lineEndNum)
    {
        DreamCatcherMap[lineStartNum, lineEndNum] = hangPntNum;
        DreamCatcherMap[lineEndNum, lineStartNum] = hangPntNum;
    }

    // 비즈 상태기록
    public void UpdateBead(int beadNum)
    {
        BeadMap[beadNum] = true;
    }

    // 깃털 상태기록
    public void UpdateFeather(int decoNum, int featherNum)
    {
        featherMap[decoNum] = featherNum;
        fNum++;
    }

    // 색 상태 기록
    public void UpdateColor(int colorNum)
    {
        colorMap = colorNum;
    }

    // 드림캐쳐 상태배열 초기화(리셋 버튼)
    public void ResetDeamCatcher()
    {
        //Debug.Log("리셋 실행");
        // 실 정보 초기화
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[0, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[1, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[2, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[3, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[4, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[5, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[6, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[7, i] = 0;
        }

        // 구슬 정보 초기화
        for (int i = 0; i < 48; i++)
        {
            BeadMap[i] = false;
        }

        // 깃털 정보 초기화
        fNum = 0;
        for (int i = 0; i < 3; i++)
        {
            featherMap[i] = 99;
        }

        // hangPoint 정보 초기화
        hangPntChecker = false;
    }

    // 구슬, 실 정보 리셋
    public void ResetStringBeadData()
    {
        // 행포인트 저장 확인용 스크립트
        /*for(int i=0; i<8; i++)
        {
            Debug.Log(hangMap[i, 0] + "/" + hangMap[i, 1]);
        }*/
        // 실 정보 초기화
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[0, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[1, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[2, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[3, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[4, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[5, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[6, i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            DreamCatcherMap[7, i] = 0;
        }

        // 구슬 정보 초기화
        for (int i = 0; i < 48; i++)
        {
            BeadMap[i] = false;
        }

        // hangPoint 정보 초기화
        hangPntChecker = false;
    }

    // 깃털 정보 초기화
    public void ResetFeatherData()
    {
        // 깃털 정보 초기화
        fNum = 0;
        for (int i = 0; i < 3; i++)
        {
            featherMap[i] = 99;
        }
    }

    // 깃털 정보 가져오기
    public string GetFeather()
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.Log(featherMap[i]);
        }

        Debug.Log(_birdinfo_data.dataList[1]);
        string featherTxt;
        if (fNum == 2)
        {
            if ((featherMap[0] == featherMap[1]) || (featherMap[0] == featherMap[2]))
            {
                featherTxt = _birdinfo_data.dataList[featherMap[0]].name + " 깃털 X2";
                return featherTxt;
            }
            else if(featherMap[1] == featherMap[2])
            {
                featherTxt = _birdinfo_data.dataList[featherMap[1]].name + " 깃털 X2";
                return featherTxt;
            }
            else
            {
                featherTxt = _birdinfo_data.dataList[featherMap[0]].name + " 깃털" + "\n" + _birdinfo_data.dataList[featherMap[1]].name + " 깃털";
                return featherTxt;
            }
        }
        else if (fNum == 3)
        {
            if (featherMap[0] == featherMap[1] && featherMap[0] == featherMap[2])
            {
                featherTxt = _birdinfo_data.dataList[featherMap[0]].name + " 깃털 X3";
                return featherTxt;
            }
            else if (featherMap[0] == featherMap[1] && featherMap[0] != featherMap[2])
            {
                featherTxt = _birdinfo_data.dataList[featherMap[0]].name + " 깃털 X2" + "\n" + _birdinfo_data.dataList[featherMap[2]].name + " 깃털";
                return featherTxt;
            }
            else if (featherMap[0] == featherMap[2] && featherMap[0] != featherMap[1])
            {
                featherTxt = _birdinfo_data.dataList[featherMap[0]].name + " 깃털 X2" + "\n" + _birdinfo_data.dataList[featherMap[1]].name + " 깃털";
                return featherTxt;
            }
            else if (featherMap[1] == featherMap[2] && featherMap[1] != featherMap[0])
            {
                featherTxt = _birdinfo_data.dataList[featherMap[1]].name + " 깃털 X2" + "\n" + _birdinfo_data.dataList[featherMap[0]].name + " 깃털";
                return featherTxt;
            }
            else
            {
                featherTxt = _birdinfo_data.dataList[featherMap[0]].name + " 깃털" + "\n" + _birdinfo_data.dataList[featherMap[1]].name + " 깃털" + "\n" + _birdinfo_data.dataList[featherMap[2]].name + " 깃털";
                return featherTxt;
            }
        }
        else
        {
            featherTxt = _birdinfo_data.dataList[featherMap[0]].name + " 깃털";
            return featherTxt;
        }
    }

    // 색 정보 가져오기
    public string GetColor()
    {
        return colorDic[colorMap];
    }

    // 비즈 활성화 상태 가져오기
    public bool GetBead(int beadIndex)
    {
        return BeadMap[beadIndex];
    }

    // 드림캐쳐 1차 완성
    public void CheckCompleteDreamCatcher()
    {
        //CaptureDreamCatcher();
        if (fNum == 3)
        {
            // 드림 캐쳐 정보 전달
            //Debug.Log(myDreamCatcher);

            string dreamCatcherId = "JS_" + dreamCatcherData.dreamCatcherCnt; // 드림캐쳐 아이디 가져오기
            myDreamCatcher = new DreamCatcher(dreamCatcherId, ConvertLineArrayTo1D(DreamCatcherMap), BeadMap, colorMap, featherMap[0], featherMap[1], featherMap[2]);
            //Debug.Log("CheckComplete myDreamCatcher : " + myDreamCatcher.GetLine()[0, 4]);
            completeDCImg.GetComponent<MakeDreamCatcher>().MakeDreamCatcherImg(myDreamCatcher);
        }
    }
    
    // 드림캐쳐 완성
    public void CompleteDreamCatcher()
    {
        dreamCatcherData.dreamCatcherList.Add(myDreamCatcher); // 드림캐쳐 데이터 추가
        dreamCatcherData.dreamCatcherCnt = dreamCatcherData.dreamCatcherCnt + 1; // 아이디 업데이트
        dreamCatcherData.nDreamCatcher = dreamCatcherData.nDreamCatcher + 1; // 드림캐쳐 개수 업데이트

        // Test
        Debug.Log("DCCheckManager: " + dreamCatcherData.dreamCatcherCnt);
        try
        {
            // 드림캐쳐 json 세이브
            GameManager.instance.GetComponent<DreamCatcherDataManager>().DataSaveText(dreamCatcherData); // json 세이브
            GameManager.instance.GetComponent<DreamCatcherDataManager>().DataLoadText<MyDreamCatcher>();
        }
        catch (Exception e)
        {
            testTxt.text += "json세이브 로드: " + e + "\n";
        }

        try
        {
            // Test
            testTxt.text = dreamCatcherData.dreamCatcherCnt.ToString(); // + ": " + dreamCatcherData.GetDreamCatcherData(dreamCatcherData.nDreamCatcher).DCcolor.ToString();
        }
        catch (Exception e)
        {
            testTxt.text += "테스트: " + e + "\n";
        }

        // Test
        //testTxt.text = dreamCatcherData.dreamCatcherCnt+": "+dreamCatcherData.GetDreamCatcherData(dreamCatcherData.nDreamCatcher).DCcolor.ToString();

        // 깃털 변경사항 저장
        //FNDManager.SaveFeatherJson();

        // 드림캐쳐 사용 재료 인벤토리 삭제
        for (int i = 0; i < fNum; i++)
        {
            if (featherMap[i]!=99)
            {
                DeleteInventory(featherMap[i]);
            }
        }
        ResetDeamCatcher();
    }

    // 인벤토리 아이템을 삭제하는(사용하는) 함수
    // : 인벤토리에서 깃털을 사용할 때 itemNumber에 해당하는 깃털을 삭제한다.
    public void DeleteInventory(int itemNumber)
    {
        FNDManager = GameManager.instance.featherDataManager;

        //itemNumber에 해당하는 깃털의 갯수 불러서 감소하기
        int itemCnt = FNDManager.featherList[itemNumber].feather_number - 1;
        // 깃털이 부족하다면 오류메시지 띄우기(오류)
        if(itemCnt<0)
        {
            Debug.LogError("깃털 갯수가 부족합니다.");
            return;
        }
        //FNDManager.GetFeatherIndexData(itemNumber).SetFeatherNumber(itemCnt);
        FNDManager.featherList[itemNumber].feather_number = itemCnt;
        //감소한 갯수 FeatherNumberData에 저장하기
        GameManager.instance.GetComponent<FeatherNumDataManager>().DataSaveText(FNDManager);
    }


    // HangPoint 활성화 확인
    public void HangPointActive()
    {
        if(DreamCatcherMap[0, 1] > 0 && DreamCatcherMap[1, 2] > 0 && DreamCatcherMap[2, 3] > 0 && 
            DreamCatcherMap[3, 4] > 0 && DreamCatcherMap[4, 5] > 0 && DreamCatcherMap[5, 6] > 0 && 
            DreamCatcherMap[6, 7] > 0 && DreamCatcherMap[7, 0] > 0)
        {
            // HangPoint 활성화
            hangPoints.SetActive(true);
            hangPntChecker = true;
        }
    }

    // 비즈 활성화 위치
    public void BeadActive()
    {
        if(DreamCatcherMap[7, 1] > 0 && DreamCatcherMap[0, 4] > 0) //0번 비즈 활성화
        {
            beadActive.Invoke(0);
        }
        if(DreamCatcherMap[7, 1] > 0 && DreamCatcherMap[0, 3] > 0) //1번 비즈 활성화
        {
            beadActive.Invoke(1);
        }
        if (DreamCatcherMap[7, 1] > 0 && DreamCatcherMap[0, 2] > 0) //2번 비즈 활성화
        {
            beadActive.Invoke(2);
        }
        if (DreamCatcherMap[0, 2] > 0 && DreamCatcherMap[1, 6] > 0) //3번 비즈 활성화
        {
            beadActive.Invoke(3);
        }
        if (DreamCatcherMap[0, 2] > 0 && DreamCatcherMap[1, 5] > 0) //4번 비즈 활성화
        {
            beadActive.Invoke(4);
        }
        if (DreamCatcherMap[0, 2] > 0 && DreamCatcherMap[1, 4] > 0) //5번 비즈 활성화
        {
            beadActive.Invoke(5);
        }
        if (DreamCatcherMap[0, 2] > 0 && DreamCatcherMap[1, 3] > 0) //6번 비즈 활성화
        {
            beadActive.Invoke(6);
        }
        if (DreamCatcherMap[3, 1] > 0 && DreamCatcherMap[2, 7] > 0) //7번 비즈 활성화
        {
            beadActive.Invoke(7);
        }
        if (DreamCatcherMap[3, 1] > 0 && DreamCatcherMap[2, 6] > 0) //8번 비즈 활성화
        {
            beadActive.Invoke(8);
        }
        if (DreamCatcherMap[3, 1] > 0 && DreamCatcherMap[2, 5] > 0) //9번 비즈 활성화
        {
            beadActive.Invoke(9);
        }
        if (DreamCatcherMap[3, 1] > 0 && DreamCatcherMap[2, 4] > 0) //10번 비즈 활성화
        {
            beadActive.Invoke(10);
        }
        if (DreamCatcherMap[4, 2] > 0 && DreamCatcherMap[3, 0] > 0) //11번 비즈 활성화
        {
            beadActive.Invoke(11);
        }
        if (DreamCatcherMap[4, 2] > 0 && DreamCatcherMap[3, 7] > 0) //12번 비즈 활성화
        {
            beadActive.Invoke(12);
        }
        if (DreamCatcherMap[4, 2] > 0 && DreamCatcherMap[3, 6] > 0) //13번 비즈 활성화
        {
            beadActive.Invoke(13);
        }

        if (DreamCatcherMap[4, 2] > 0 && DreamCatcherMap[3, 5] > 0) //14번 비즈 활성화
        {
            beadActive.Invoke(14);
        }
        if (DreamCatcherMap[3, 5] > 0 && DreamCatcherMap[4, 1] > 0) //15번 비즈 활성화
        {
            beadActive.Invoke(15);
        }
        if (DreamCatcherMap[3, 5] > 0 && DreamCatcherMap[4, 0] > 0) //16번 비즈 활성화
        {
            beadActive.Invoke(16);
        }
        if (DreamCatcherMap[3, 5] > 0 && DreamCatcherMap[4, 7] > 0) //17번 비즈 활성화
        {
            beadActive.Invoke(17);
        }
        if (DreamCatcherMap[3, 5] > 0 && DreamCatcherMap[4, 6] > 0) //18번 비즈 활성화
        {
            beadActive.Invoke(18);
        }
        if (DreamCatcherMap[6, 4] > 0 && DreamCatcherMap[5, 2] > 0) //19번 비즈 활성화
        {
            beadActive.Invoke(19);
        }
        if (DreamCatcherMap[6, 4] > 0 && DreamCatcherMap[5, 1] > 0) //20번 비즈 활성화
        {
            beadActive.Invoke(20);
        }
        if (DreamCatcherMap[6, 4] > 0 && DreamCatcherMap[5, 0] > 0) //21번 비즈 활성화
        {
            beadActive.Invoke(21);
        }
        if (DreamCatcherMap[6, 4] > 0 && DreamCatcherMap[5, 7] > 0) //22번 비즈 활성화
        {
            beadActive.Invoke(22);
        }
        if (DreamCatcherMap[5, 7] > 0 && DreamCatcherMap[6, 3] > 0) //23번 비즈 활성화
        {
            beadActive.Invoke(23);
        }
        if (DreamCatcherMap[5, 7] > 0 && DreamCatcherMap[6, 2] > 0) //24번 비즈 활성화
        {
            beadActive.Invoke(24);
        }
        if (DreamCatcherMap[5, 7] > 0 && DreamCatcherMap[6, 1] > 0) //25번 비즈 활성화
        {
            beadActive.Invoke(25);
        }
        if (DreamCatcherMap[5, 7] > 0 && DreamCatcherMap[6, 0] > 0) //26번 비즈 활성화
        {
            beadActive.Invoke(26);
        }
        if (DreamCatcherMap[6, 0] > 0 && DreamCatcherMap[7, 4] > 0) //27번 비즈 활성화
        {
            beadActive.Invoke(27);
        }
        if (DreamCatcherMap[6, 0] > 0 && DreamCatcherMap[7, 3] > 0) //28번 비즈 활성화
        {
            beadActive.Invoke(28);
        }
        if (DreamCatcherMap[6, 0] > 0 && DreamCatcherMap[7, 2] > 0) //29번 비즈 활성화
        {
            beadActive.Invoke(29);
        }
        if (DreamCatcherMap[6, 0] > 0 && DreamCatcherMap[7, 1] > 0) //30번 비즈 활성화
        {
            beadActive.Invoke(30);
        }
        if (DreamCatcherMap[7, 1] > 0 && DreamCatcherMap[5, 0] > 0) //31번 비즈 활성화
        {
            beadActive.Invoke(31);
        }

        if (DreamCatcherMap[0, 5] > 0 && DreamCatcherMap[7, 2] > 0) //32번 비즈 활성화
        {
            beadActive.Invoke(32);
        }
        if ((DreamCatcherMap[0, 4] > 0 && DreamCatcherMap[7, 2] > 0)
            || (DreamCatcherMap[0, 4] > 0 && DreamCatcherMap[1, 6] > 0)
            || (DreamCatcherMap[1, 6] > 0 && DreamCatcherMap[7, 2] > 0)) //33번 비즈 활성화
        {
            beadActive.Invoke(33);
        }
        if (DreamCatcherMap[1, 6] > 0 && DreamCatcherMap[0, 3] > 0) //34번 비즈 활성화
        {
            beadActive.Invoke(34);
        }
        if ((DreamCatcherMap[1, 5] > 0 && DreamCatcherMap[7, 2] > 0)
            || (DreamCatcherMap[0, 3] > 0 && DreamCatcherMap[1, 5] > 0)
            || (DreamCatcherMap[0, 3] > 0 && DreamCatcherMap[7, 2] > 0)) //35번 비즈 활성화
        {
            beadActive.Invoke(35);
        }
        if (DreamCatcherMap[7, 2] > 0 && DreamCatcherMap[1, 4] > 0) //36번 비즈 활성화
        {
            beadActive.Invoke(36);
        }
        if ((DreamCatcherMap[6, 2] > 0 && DreamCatcherMap[1, 4] > 0)
            || (DreamCatcherMap[6, 2] > 0 && DreamCatcherMap[3, 0] > 0)
            || (DreamCatcherMap[1, 4] > 0 && DreamCatcherMap[3, 0] > 0)) //37번 비즈 활성화
        {
            beadActive.Invoke(37);
        }
        if (DreamCatcherMap[5, 2] > 0 && DreamCatcherMap[0, 3] > 0) //38번 비즈 활성화
        {
            beadActive.Invoke(38);
        }
        if ((DreamCatcherMap[5, 2] > 0 && DreamCatcherMap[1, 4] > 0)
            || (DreamCatcherMap[5, 2] > 0 && DreamCatcherMap[3, 7] > 0)
            || (DreamCatcherMap[3, 7] > 0 && DreamCatcherMap[1, 4] > 0)) //39번 비즈 활성화
        {
            beadActive.Invoke(39);
        }
        if (DreamCatcherMap[6, 3] > 0 && DreamCatcherMap[4, 1] > 0) //40번 비즈 활성화
        {
            beadActive.Invoke(40);
        }
        if ((DreamCatcherMap[0, 4] > 0 && DreamCatcherMap[5, 2] > 0)
            || (DreamCatcherMap[0, 4] > 0 && DreamCatcherMap[3, 6] > 0)
            || (DreamCatcherMap[5, 2] > 0 && DreamCatcherMap[3, 6] > 0)) //41번 비즈 활성화
        {
            beadActive.Invoke(41);
        }
        if (DreamCatcherMap[5, 2] > 0 && DreamCatcherMap[7, 4] > 0) //42번 비즈 활성화
        {
            beadActive.Invoke(42);
        }
        if ((DreamCatcherMap[7, 4] > 0 && DreamCatcherMap[6, 3] > 0)
            || (DreamCatcherMap[5, 1] > 0 && DreamCatcherMap[7, 4] > 0)
            || (DreamCatcherMap[5, 1] > 0 && DreamCatcherMap[6, 3] > 0)) //43번 비즈 활성화
        {
            beadActive.Invoke(43);
        }
        if (DreamCatcherMap[0, 5] > 0 && DreamCatcherMap[6, 3] > 0) //44번 비즈 활성화
        {
            beadActive.Invoke(44);
        }
        if ((DreamCatcherMap[6, 2] > 0 && DreamCatcherMap[7, 4] > 0)
            || (DreamCatcherMap[0, 5] > 0 && DreamCatcherMap[2, 6] > 0)
            || (DreamCatcherMap[0, 5] > 0 && DreamCatcherMap[7, 4] > 0)) //45번 비즈 활성화
        {
            beadActive.Invoke(45);
        }
        if (DreamCatcherMap[1, 6] > 0 && DreamCatcherMap[7, 4] > 0) //46번 비즈 활성화
        {
            beadActive.Invoke(46);
        }
        if ((DreamCatcherMap[1, 6] > 0 && DreamCatcherMap[7, 3] > 0)
            || (DreamCatcherMap[0, 5] > 0 && DreamCatcherMap[1, 6] > 0)
            || (DreamCatcherMap[0, 5] > 0 && DreamCatcherMap[7, 3] > 0)) //47번 비즈 활성화
        {
            beadActive.Invoke(47);
        }
    }


    // (임시)도감 제작용 함수
    public void AddDCCollection(DreamCatcher mydreamcatcher)
    {
        //Json 로드하기
        //드림캐쳐 추가하기
        //Json 저장하기
    }

    private static string getPath(string fileName)
    {
#if UNITY_EDITOR
        return Application.persistentDataPath + "/Saves/" + fileName + ".json";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"/Saves/"+ fileName + ".json";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+ fileName + ".json";
#else
        return Application.persistentDataPath +"/"+fileName + ".json";
#endif
    }
}

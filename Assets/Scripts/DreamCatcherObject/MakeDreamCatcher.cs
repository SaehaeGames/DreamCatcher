using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class MakeDreamCatcher : MonoBehaviour
{
    //public int dreamcatcherNumber;
    //게임 오브젝트
    public GameObject ring;
    public GameObject feathers;
    public GameObject beads;
    public GameObject points;
    public GameObject hangPoints;
    public GameObject gem;
    public GameObject lines;

    public GameObject linePrefab;

    //드림캐쳐 데이터
    //private static string fileName = "DreamCatcherData";       //파일 이름
    //public List<DreamCatcher> madeData = new List<DreamCatcher>();
    private int[,] lineMap = new int[8, 8];
    private bool[] beadMap = new bool[48];

    //이미지 데이터
    public Sprite[] ringImgs;
    public Sprite[] featherImgs;
    public Sprite[] beadImgs;
    public Sprite[] gemImgs;
    private Color[] stringColors = { new Color(1f, 1f, 1f, 1f), new Color(1f, 0.9254903f, 0.4784314f, 1f),
        new Color(0.3333333f, 0.345098f, 0.4705883f, 1f), new Color(0.6078432f, 0.2235294f, 0.2352941f, 1f),
        new Color(0.3411765f, 0.3411765f, 0.3411765f, 1f) };

    //public GameObject txtCheck;

    public int colorNum;

    public MyDreamCatcher dreamCatcherData; //MyDreamCatcher 객체 필요

    public void LineDelete()
    {
        if (lines.transform.childCount > 0)
        {
            Debug.Log("라인 삭제");
            for (int i = 0; i > lines.transform.childCount; i++)
            {
                Destroy(lines.transform.GetChild(i));
            }
        }
    }
    // 드림캐쳐 이미지 만들기(DreamCatcher 객체 파라미터)
    public void MakeDreamCatcherImg(DreamCatcher myDreamCatcher)
    {
        // 드림캐쳐 초기화
        // :라인이 이미 있다면 초기화
        
        // 색
        colorNum = myDreamCatcher.GetColor();
        Debug.Log("라인색 : " + colorNum);

        // 링
        ring.GetComponent<Image>().sprite = ringImgs[colorNum];
        //라인
        lineMap = myDreamCatcher.ConvertLineArrayTo2D(myDreamCatcher.GetLine());

        //Debug.Log("myDreamCatcher.GetLine()[0, 4] : " + myDreamCatcher.GetLine()[0, 4]);
        //Debug.Log("lineMap[0, 4] : " + lineMap[0, 4]);

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                //lineMap 확인
                if (lineMap[i, j] > 0 && lineMap[j, i] > 0)
                {
                    Debug.Log("lineMap[0, 4] is ok : "+lineMap[0, 4]);
                    //라인 생성
                    var line = Instantiate(linePrefab);
                    line.transform.SetParent(lines.transform);
                    line.GetComponent<LineRenderer>().sortingOrder = 6;
                    //라인 색 설정
                    line.GetComponent<LineRenderer>().startColor = stringColors[colorNum];
                    line.GetComponent<LineRenderer>().endColor = stringColors[colorNum];
                    Debug.Log("라인 업데이트");
                    //라인 넓이 설정
                    line.GetComponent<LineRenderer>().startWidth = 0.01f;
                    line.GetComponent<LineRenderer>().endWidth = 0.01f;
                    if (lineMap[i, j] > 1 && lineMap[j, i] > 1)
                    {
                        //라인 연결
                        line.GetComponent<LineRenderer>().positionCount = 3;
                        line.GetComponent<LineRenderer>().SetPosition(0, points.transform.GetChild(i).position);
                        line.GetComponent<LineRenderer>().SetPosition(1, hangPoints.transform.GetChild(lineMap[i, j] - 2).position);
                        line.GetComponent<LineRenderer>().SetPosition(2, points.transform.GetChild(j).position);
                    }
                    else
                    {
                        //라인 연결
                        line.GetComponent<LineRenderer>().positionCount = 2;
                        line.GetComponent<LineRenderer>().SetPosition(0, points.transform.GetChild(i).position);
                        line.GetComponent<LineRenderer>().SetPosition(1, points.transform.GetChild(j).position);
                    }
                    //madeData 업데이트
                    //lineMap[i, j] = 0;
                    //lineMap[j, i] = 0;
                    //Debug.Log("lineMap 변경후 : " + myDreamCatcher.GetLine()[0, 4]);
                }
            }
        }

        //구슬
        beadMap = myDreamCatcher.GetBead();
        for (int i = 0; i < 48; i++)
        {
            if (beadMap[i])
            {
                beads.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = beadImgs[colorNum];
                beads.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }

        //보석
        gem.GetComponent<Image>().sprite = gemImgs[myDreamCatcher.GetColor()];

        //깃털
        for (int i = 0; i < 3; i++)
        {
            feathers.transform.GetChild(i).GetChild(3).gameObject.GetComponent<Image>().sprite = featherImgs[myDreamCatcher.GetFeather(i)];
            feathers.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = beadImgs[colorNum];
            feathers.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Image>().sprite = beadImgs[colorNum];
        }
    }

    //Json파일 불러오기
    /*public void LoadJson()
    {
        string savePath = getPath(fileName);
        if (File.Exists(savePath))
        {
            string jdata = File.ReadAllText(savePath);
            madeData = JsonHelper.FromJson<DreamCatcher>(jdata); // 드림캐쳐 데이터들 불러옴
            Debug.Log("파일을 불러온 후 data: " + madeData);
            Debug.Log("**********Json파일 불러옴*************");
        }
        else
        {
            SaveJson();
            //data = new List<DreamCatcher>();
            Debug.Log("**********Json파일 없음*************");
        }
    }

    //Json파일 저장
    public void SaveJson()
    {
        string savePath = getPath(fileName);
        string jdata = JsonHelper.ToJson(madeData, true);
        Debug.Log("data: " + madeData);
        Debug.Log("jdata: " + jdata);
        File.WriteAllText(savePath, jdata);
        Debug.Log("**********Json파일 저장************");
    }*/

    // 드림캐쳐 이미지 만들기(Json파일 인덱스 파라미터)
    public void MakeDreamCatcherImg(int dreamcatcherIndex)
    {
        // 드림캐쳐 초기화
        // :라인이 이미 있다면 초기화
        if (lines.transform.childCount > 0)
        {
            for (int i = 0; i > lines.transform.childCount; i++)
            {
                Destroy(lines.transform.GetChild(i));
            }
        }

        //Json파일 로드
        dreamCatcherData = GameManager.instance.loadDreamCatcherData; //MyDreamCatcher 객체 GameManager에서 가져옴
        GameManager.instance.GetComponent<DreamCatcherDataManager>().DataLoadText<MyDreamCatcher>();
        DreamCatcher selectDreamCatcher = dreamCatcherData.dreamCatcherList[dreamcatcherIndex];
        
        // 색
        colorNum = selectDreamCatcher.GetColor();

        // 링
        ring.GetComponent<Image>().sprite = ringImgs[colorNum];

        //라인
        lineMap = selectDreamCatcher.ConvertLineArrayTo2D(selectDreamCatcher.GetLine());
        for (int i=0; i<8; i++)
        {
            for (int j=0; j<8; j++)
            {
                //lineMap 확인
                if (lineMap[i, j] > 0 && lineMap[j, i] > 0)
                {
                    //라인 생성
                    var line = Instantiate(linePrefab);
                    line.transform.SetParent(lines.transform);
                    //라인 색 설정
                    line.GetComponent<LineRenderer>().startColor = stringColors[colorNum];
                    line.GetComponent<LineRenderer>().endColor = stringColors[colorNum];
                    //라인 넓이 설정
                    line.GetComponent<LineRenderer>().startWidth = 0.01f;
                    line.GetComponent<LineRenderer>().endWidth = 0.01f;
                    if(lineMap[i, j]>1&&lineMap[j, i]>1)
                    {
                        //라인 연결
                        line.GetComponent<LineRenderer>().positionCount = 3;
                        line.GetComponent<LineRenderer>().SetPosition(0, points.transform.GetChild(i).position);
                        line.GetComponent<LineRenderer>().SetPosition(1, hangPoints.transform.GetChild(lineMap[i, j]-2).position);
                        line.GetComponent<LineRenderer>().SetPosition(2, points.transform.GetChild(j).position);
                    }
                    else
                    {
                        //라인 연결
                        line.GetComponent<LineRenderer>().positionCount = 2;
                        line.GetComponent<LineRenderer>().SetPosition(0, points.transform.GetChild(i).position);
                        line.GetComponent<LineRenderer>().SetPosition(1, points.transform.GetChild(j).position);
                    }
                }
            }
        }

        // 구슬
        // ERROR: 드림캐쳐가 없는 상태에서 처음 만들고 재구성하면 구슬만 안뜨는 오류
        // 근데 저장된 결과보면 아무 문제 없이 저장되어 있음
        //Debug.Log(selectDreamCatcher.GetBead()[0]);
        beadMap = selectDreamCatcher.GetBead();
        for (int i = 0; i < 48; i++)
        {
            //Debug.Log("MakeDreamCatcher-비즈상태 [" + i + "]: " + beadMap[i]); //전부 false 나옴
            if (beadMap[i])
            {
                beads.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = beadImgs[colorNum];
                beads.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                Debug.Log("MakeDreamCatcher-비즈 활성화 [" + i + "]");
            }
        }

        //보석
        gem.GetComponent<Image>().sprite = gemImgs[selectDreamCatcher.GetColor()];
        //Debug.Log("보석 설정");

        //깃털
        for (int i = 0; i < 3; i++)
        {
            feathers.transform.GetChild(i).GetChild(3).gameObject.GetComponent<Image>().sprite = featherImgs[selectDreamCatcher.GetFeather(i)];
            feathers.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = beadImgs[colorNum];
            feathers.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Image>().sprite = beadImgs[colorNum];
        }
    }

    //파일위치
    private static string getPath(string fileName)
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Saves/" + fileName + ".json";
#elif UNITY_ANDROID
        return Application.persistentDataPath+ "/" + fileName + ".json";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+ fileName + ".json";
#else
        return Application.dataPath +"/"+ fileName + ".json";
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class DreamCatcherPreview : MonoBehaviour
{
    //public int dreamcatcherNumber;
    [Header("[DreamCatcher Object]")]
    [SerializeField] private GameObject ring;
    [SerializeField] private GameObject feathers;
    [SerializeField] private GameObject beads;
    [SerializeField] private GameObject points;
    [SerializeField] private GameObject hangPoints;
    [SerializeField] private GameObject gem;
    [SerializeField] private GameObject lines;

    [Header("[Line Prefab]")]
    public GameObject linePrefab;

    [Header("[Setting]")]
    public float lineWidgth;

    //이미지 데이터
    [Header("[Image Source]")]
    [SerializeField] private Sprite[] ringImgs;
    [SerializeField] private Sprite[] featherImgs;
    [SerializeField] private Sprite[] beadImgs;
    [SerializeField] private Sprite[] gemImgs;
    private Color[] stringColors = { new Color(1f, 1f, 1f, 1f), new Color(1f, 0.9254903f, 0.4784314f, 1f),
        new Color(0.3333333f, 0.345098f, 0.4705883f, 1f), new Color(0.6078432f, 0.2235294f, 0.2352941f, 1f),
        new Color(0.3411765f, 0.3411765f, 0.3411765f, 1f) };

    [Header("[Data]")]
    public DreamCatcherDataManager dreamCatcherDataManager;

    private int[,] lineMap = new int[8, 8];
    private bool[] beadMap = new bool[48];
    private int colorNum;


    public void LineDelete()
    {
        if (lines.transform.childCount > 0)
        {
            Debug.Log("라인 삭제");
            for (int i = 0; i < lines.transform.childCount; i++)
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

        // 링
        ring.GetComponent<Image>().sprite = ringImgs[colorNum];
        //라인
        lineMap = myDreamCatcher.ConvertLineArrayTo2D(myDreamCatcher.GetLine());

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                //lineMap 확인
                if (lineMap[i, j] > 0 && lineMap[j, i] > 0)
                {
                    //라인 생성
                    var line = Instantiate(linePrefab);
                    line.transform.SetParent(lines.transform);
                    line.GetComponent<LineRenderer>().sortingOrder = 6;
                    
                    //라인 색 설정
                    line.GetComponent<LineRenderer>().startColor = stringColors[colorNum];
                    line.GetComponent<LineRenderer>().endColor = stringColors[colorNum];
                    
                    //라인 넓이 설정
                    line.GetComponent<LineRenderer>().startWidth = lineWidgth;
                    line.GetComponent<LineRenderer>().endWidth = lineWidgth;
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

        // Json파일 로드
        dreamCatcherDataManager = GameManager.instance.dreamCatcherDataManager;
        DreamCatcher selectDreamCatcher = dreamCatcherDataManager.GetDreamCatcherDataByIndex(dreamcatcherIndex);
        
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMakeDreamCatcher : MonoBehaviour
{
    public int dreamcatcherNumber;
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
    private static string fileName = "DreamCatcherData";       //파일 이름
    public List<DreamCatcher> madeData = new List<DreamCatcher>();
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

    public GameObject txtCheck;

    public int colorNum;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        dreamcatcherNumber = madeData.Count - 1;
        MakeDreamCatcherImg(dreamcatcherNumber);
    }

    // 드림캐쳐 이미지 만들기
    private void MakeDreamCatcherImg(int dreamcatcherIndex)
    {
        // 색
        colorNum = madeData[dreamcatcherIndex].GetColor();

        // 링
        ring.GetComponent<Image>().sprite = ringImgs[colorNum];

        //라인
        lineMap = madeData[dreamcatcherIndex].ConvertLineArrayTo2D(madeData[dreamcatcherIndex].GetLine());
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
                    //라인 색 설정
                    line.GetComponent<LineRenderer>().startColor = stringColors[colorNum];
                    line.GetComponent<LineRenderer>().endColor = stringColors[colorNum];
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
                    lineMap[i, j] = 0;
                    lineMap[j, i] = 0;
                }
            }
        }

        //구슬
        beadMap = madeData[dreamcatcherIndex].GetBead();
        for (int i = 0; i < 48; i++)
        {
            if (beadMap[i])
            {
                beads.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = beadImgs[colorNum];
                beads.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }

        //보석
        gem.GetComponent<Image>().sprite = gemImgs[madeData[dreamcatcherIndex].GetColor()];

        //깃털
        for (int i = 0; i < 3; i++)
        {
            feathers.transform.GetChild(i).GetChild(3).gameObject.GetComponent<Image>().sprite = featherImgs[madeData[dreamcatcherIndex].GetFeather(i)];
            feathers.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = beadImgs[colorNum];
            feathers.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Image>().sprite = beadImgs[colorNum];
        }
    }
}

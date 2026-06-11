using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Android;

public class MakingUIManager : MonoBehaviour
{
    // 매니저
    private DCCheckManager DCManager;
    public FeatherDataManager FNDManager;

    // 창 관리
    public GameObject StringWindow, FeatherWindow;
    public GameObject StringBtn, FeatherBtn;
    public GameObject PopupWin;
    public GameObject materialCanvas;
    public GameObject uiCanvas;
    public DreamCatcherPreview dreamCatcherPreview;

    // 재료 관리(실)
    public GameObject[] Strings;

    // 재료 관리(깃털)
    public GameObject[] Feathers;

    // 깃털장식 관리
    public GameObject[] featherDecos;

    // 실 색 관리
    public Image StringRing;
    public Image Strap;
    public GameObject Lines;
    public Sprite[] StringRingImgs;
    private Color[] stringColors = { new Color(1f, 1f, 1f, 1f), new Color(1f, 0.9254903f, 0.4784314f, 1f), 
        new Color(0.3333333f, 0.345098f, 0.4705883f, 1f), new Color(0.6078432f, 0.2235294f, 0.2352941f, 1f), 
        new Color(0.3411765f, 0.3411765f, 0.3411765f, 1f) };
    public GameObject points;

    // 보석 관리
    public Sprite[] gemsImg;
    public Image gem;
    public Sprite[] beadsImg;
    public GameObject bead;
    public GameObject smallBeadLayer;

    // 버튼 관리
    public GameObject resetStringBtn;
    public GameObject resetFeatherBtn;
    public GameObject completeBtn;

    // 데이터 관리
    public static int colorNumber=0;
    public DreamCatcherDataManager dreamCatcherDataManager;

    private DreamCatcher myDreamCatcher;

    // HangPoint
    public GameObject hangPoints;

    public SceneState sceneState;
    public GameSceneManager _gameSceneManager;
    private bool makingLineStartCheck;
    private bool makingFeatherStartCheck;

    // 스크린샷
    [SerializeField]
    private DreamCatcherThumbnailRenderer thumbnailRenderer;

    private void OnEnable()
    {
        _gameSceneManager = GameSceneManager.Instance;
        _gameSceneManager.SceneChangeWarn += WarnWinMove;
        DCManager = GameObject.FindWithTag("CreateManager").gameObject.GetComponent<DCCheckManager>();
        //FNDManager = GameObject.FindWithTag("GameManager").gameObject.GetComponent<FeatherNumDataManager>();
        FNDManager = GameManager.instance.featherDataManager;
        DCManager.gemActive += Gem;
        DCManager.beadActive += Bead;
    }

    private void OnDisable()
    {
        _gameSceneManager.SceneChangeWarn -= WarnWinMove;
        DCManager.gemActive -= Gem;
        DCManager.beadActive -= Bead;
    }

    // Start is called before the first frame update
    void Start()
    {
        makingLineStartCheck = false;
        makingFeatherStartCheck = false;
        Debug.Log("Feather[2] Test : "+Feathers[2].transform.GetChild(1));
        StringWin();

        // 깃털 인벤토리 초기화
        Debug.Log("인벤토리 초기화 진행");
        FeatherInventory();

        for (int i=0; i<48; i++)
        {
            bead.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i=0; i<3; i++)
        {
            featherDecos[i].SetActive(false);
        }

        //GoodsContainer curGoodsData = GameManager.instance.loadGoodsData;  //가구 데이터 가져옴       
        int threadLevel = GameManager.instance.goodsDataManager.GetValidatedGoodsData(Constants.GoodsData_Thread).level;  //실의 레벨을 가져옴
        //Debug.Log(threadLevel);
        for(int i=0; i<threadLevel+1; i++)
        {
            Strings[i].SetActive(true);
        }

        // 팝업창 초기화
        PopupWin.transform.GetChild(0).gameObject.SetActive(false);
        PopupWin.transform.GetChild(1).gameObject.SetActive(false);
        PopupWin.SetActive(false);
    }

    // 실창 활성화
    public void StringWin()
    {
        // 버튼 조작
        completeBtn.SetActive(false);
        resetStringBtn.SetActive(true);
        resetFeatherBtn.SetActive(false);
        // StringWin 활성화, FeatherWin 비활성화
        StringWindow.SetActive(true);
        FeatherWindow.SetActive(false);
        // StringBtn 밝음, FeatherBtn 어둡
        StringBtn.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        FeatherBtn.GetComponent<Image>().color = new Color(0.5943396f, 0.5943396f, 0.5943396f, 1.0f);
        // 드림캐쳐 상태변화
        StringSizeChange();
        if (DCManager.hangPntChecker)
        {
            hangPoints.SetActive(true); //hangPoint 끄기
        }
        for (int i = 0; i < 3; i++)
        {
            featherDecos[i].SetActive(false);
        }
    }

    // 깃털창 활성화
    public void FeatherWin()
    {
        // 버튼 조작
        completeBtn.SetActive(true);
        resetStringBtn.SetActive(false);
        resetFeatherBtn.SetActive(true);
        // FeatherWin 활성화, StringWin 비활성화
        FeatherWindow.SetActive(true);
        StringWindow.SetActive(false);
        
        // FeatherBtn 밝음, StringBtn 어둡
        FeatherBtn.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        StringBtn.GetComponent<Image>().color = new Color(0.5943396f, 0.5943396f, 0.5943396f, 1.0f);
        // 드림캐쳐 상태변화
        FeatherSizeChange();
        hangPoints.SetActive(false); //hangPoint 끄기
        // 깃털 장식 드롭 활성화
        for (int i = 0; i < 3; i++)
        {
            featherDecos[i].SetActive(true);
            featherDecos[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = beadsImg[colorNumber];
            featherDecos[i].transform.GetChild(2).gameObject.GetComponent<Image>().sprite = beadsImg[colorNumber];
        }
        
    }

    // 깃털창 깃털 갯수 FeatherNumInfo.json과 연동
    // 깃털창에 깃털 갯수를 FeatherNumInfo.json을 불러와 적용시키는 함수
    public void FeatherInventory()
    {
        Debug.Log("FNDManager.SizeFeatherJson() : " + FNDManager.GetFeatherDataListCount());
        for (int i = 0; i < FNDManager.GetFeatherDataListCount(); i++)
        {
            if (i==3 || i==7 || i==11 || i==15) //만약 특별 새라면
            {
                //birdCnt = 0;    //횟수 초기화
                continue;   //추가하지 않고 넘어감
            }
            else //특별새가 아니라면
            {
                //해당 깃털 갯수 업데이트
                int itemcnt = FNDManager.GetFeatherCount(i);    //깃털의 수를 가져옴
                if (itemcnt > 0)
                {
                    Feathers[i].SetActive(true);
                    if (itemcnt >= 2)
                    {
                        Feathers[i].transform.GetChild(0).gameObject.SetActive(true);
                        Feathers[i].transform.GetChild(1).GetComponent<FeatherDrag>().FeatherCntReset();
                        Feathers[i].transform.GetChild(0).GetComponent<Text>().text = "X" + itemcnt;
                    }
                }
                else
                {
                    Feathers[i].SetActive(false);
                }
            }
        }
    }

    // 깃털창 드림캐쳐 변화
    public void FeatherSizeChange()
    {
        // 라인 포인트 비활성화
        points.SetActive(false);
        // 링, 스트랩 크기 변화
        StringRing.transform.localPosition = new Vector2(0f, 206f);
        StringRing.GetComponent<RectTransform>().sizeDelta = new Vector2(1440f, 743.8f);

        Strap.transform.localPosition = new Vector2(0f, 888.7f);
        Strap.GetComponent<RectTransform>().sizeDelta = new Vector2(1654.9f, 964.9f);

        // 실 크기 변화
        for (int i = 0; i < Lines.transform.childCount; i++)
        {
            Lines.transform.GetChild(i).gameObject.transform.localPosition = new Vector2(0f, 9f);
            Lines.transform.GetChild(i).gameObject.transform.localScale = new Vector2(122.0906f, 122.0906f);
            Lines.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().startWidth = 0.05f;
            Lines.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().endWidth = 0.05f;
        }

        // 구슬 크기 변화
        smallBeadLayer.SetActive(true);
        
        for (int i = 0; i < 48; i++)
        {
            
            if (DCManager.GetBead(i))
            {
                smallBeadLayer.transform.GetChild(i + 1).gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                smallBeadLayer.transform.GetChild(i + 1).gameObject.GetComponent<Image>().sprite = beadsImg[colorNumber];
            }
            else
            {
                smallBeadLayer.transform.GetChild(i + 1).gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                smallBeadLayer.transform.GetChild(i + 1).gameObject.GetComponent<Image>().sprite = beadsImg[colorNumber];
            }
        }
        bead.SetActive(false);

        // 보석 크기 변화
        gem.gameObject.SetActive(true);
        gem.sprite = gemsImg[colorNumber];
    }

    // 실창 드림캐쳐 변화
    public void StringSizeChange()
    {
        // 라인 포인트 활성화
        points.SetActive(true);
        // 링, 스트랩 크기 변화
        StringRing.transform.localPosition = new Vector2(0f, -88f);
        StringRing.GetComponent<RectTransform>().sizeDelta = new Vector2(1440f, 1563.6f);
        Strap.transform.localPosition = new Vector2(0f, 1101.898f);
        Strap.GetComponent<RectTransform>().sizeDelta = new Vector2(1654.901f, 1478.545f);

        // 실 크기 변화
        for (int i = 0; i < Lines.transform.childCount; i++)
        {
            Lines.transform.GetChild(i).gameObject.transform.localPosition = new Vector2(0f, -504.65f);
            Lines.transform.GetChild(i).gameObject.transform.localScale = new Vector2(256f, 256f);
            Lines.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().startWidth = 0.1f;
            Lines.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().endWidth = 0.1f;
        }

        // 보석 크기 변화
        gem.gameObject.SetActive(false);

        // 구슬 크기 변화
        bead.SetActive(true);
        smallBeadLayer.SetActive(false);
    }

    // 실, 구슬 색변경
    public void StringColorChange(int colorNum)
    {
        colorNumber = colorNum;
        DCManager.UpdateColor(colorNum);
        // 있던 것들 변경
        StringRing.sprite = StringRingImgs[colorNum]; // 링 색 변경
        for(int i=0; i<Lines.transform.childCount; i++)
        {
            Lines.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().startColor = stringColors[colorNum];
            Lines.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().endColor = stringColors[colorNum];
        }

        // 실 정보 변경
        for(int i=0; i<points.transform.childCount; i++)
        {
            points.transform.GetChild(i).gameObject.GetComponent<DragPoint>().StringColorSet(colorNum);
        }

        for (int i = 0; i < 48; i++)
        {
            bead.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = beadsImg[colorNum];
        }
    }

    // 보석 활성화
    void Gem()
    {
        
    }

    void Bead(int beadNum)
    {
        Debug.Log(bead.transform.GetChild(beadNum));
        bead.transform.GetChild(beadNum).gameObject.SetActive(true);
    }

    //창 이동 효과음(창 이동 버튼 이벤트)
    public void PlayTabEffect()
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
    }

    // 드림캐쳐 초기화(실창 리셋 버튼)
    public void ResetString()
    {
        // 제작 시작 알림
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음


        // 실 삭제
        for (int i = Lines.transform.childCount; i > 0; i--)
        {
            Destroy(Lines.transform.GetChild(i-1).gameObject);
        }

        // 구슬 초기화
        for (int i = 0; i < 48; i++)
        {
            bead.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            bead.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < 48; i++)
        {
            smallBeadLayer.transform.GetChild(i + 1).gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

        // 제작 시작상태 업데이트
        EndMakingLine();

        // HangPoint 비활성화
        hangPoints.SetActive(false);
    }

    // 드림캐쳐 초기화(깃털창 리셋 버튼)
    public void ResetFeather()
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
        // 깃털 초기화
        for (int i = 0; i < 3; i++)
        {
            featherDecos[i].transform.GetChild(3).gameObject.gameObject.SetActive(false);
        }
        // 제작 시작 상태 업데이트
        EndMakingFeather();
        // 깃털 재료창 초기화
        FeatherInventory();
    }

    // 드림캐쳐 제작 시작 상태 업데이트
    public void StartMakingLine()
    {
        makingLineStartCheck = true;
    }

    public void EndMakingLine()
    {
        makingLineStartCheck = false;
    }

    public void StartMakingFeather()
    {
        makingFeatherStartCheck = true;
    }

    public void EndMakingFeather()
    {
        makingFeatherStartCheck = false;
    }

    // 드림캐쳐 완성(완성 버튼)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void CompleteDreamCatcher()
    {
        // 깃털충분=>캡쳐, 깃털부족=>팝업
        if (DCManager.IsFeatherEnough())
        {
            // 드림캐쳐 생성
            myDreamCatcher = DCManager.ConfirmMyDreamCatcher();
            // 드림캐쳐 프리뷰 생성
            dreamCatcherPreview.MakeDreamCatcherImg(myDreamCatcher);
            OpenCheckDreamCatcherWin();
        }
        else
        {
            // 깃털부족 팝업창 열기
            PopupWin.SetActive(true);
            PopupWin.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void OpenCheckDreamCatcherWin()
    {
        // 완성 팝업창 열기
        PopupWin.SetActive(true);
        PopupWin.transform.GetChild(0).gameObject.SetActive(true);
        PopupWin.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Text>().text
            = DreamCatcherDescriptionBuilder.Build(DCManager.GetMyDreamCatcher());
    }

    // 드림캐쳐 확인 버튼
    public void OKDreamCatcher()
    {
        // 창 닫기
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
        PopupWin.transform.GetChild(0).gameObject.SetActive(false);
        PopupWin.SetActive(false);

        // 스샷 찍기
        string thumbnailPath = thumbnailRenderer.SaveThumbnail(myDreamCatcher);

        // 이미지 경로 드림캐쳐에 저장
        DCManager.CompleteDreamCatcher(thumbnailPath);

        // UI 초기화
        ResetFeather();
        ResetString();
    }

    // 드림캐쳐 취소 버튼
    public void CancleDreamCatcher()
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
        PopupWin.transform.GetChild(0).gameObject.SetActive(false);
        PopupWin.SetActive(false);
    }

    // 깃털 갯수 부족 팝업 닫기
    public void CancleFeatherLack()
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
        PopupWin.transform.GetChild(1).gameObject.SetActive(false);
        PopupWin.SetActive(false);
    }

    // 창 이동 경고 팝업 열기
    public void WarnWinMove(SceneState nextSceneState)
    {
        sceneState = nextSceneState;
        if (makingLineStartCheck || makingFeatherStartCheck)
        {
            PopupWin.SetActive(true);
            PopupWin.transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            _gameSceneManager.UpdateSceneState(sceneState, SceneState.Making);
            SceneManager.LoadScene(sceneState.ToString());
        }
    }

    // 창 이동 경고 팝업 닫기
    public void CancelWinMove()
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
        PopupWin.transform.GetChild(2).gameObject.SetActive(false);
        PopupWin.SetActive(false);
    }

    // 창 이동 경고 창 이동 확인
    public void ChangeWinOk()
    {
        _gameSceneManager.UpdateSceneState(sceneState, SceneState.Making);
        SceneManager.LoadScene(sceneState.ToString());
    }
}

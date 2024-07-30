using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialClick : TutorialBase
{
    private GameObject arrow;
    private GameObject arrowPrefab; // 화살표 프리팹
    private Vector2 arrowPos;
    private GameObject canvas;
    [SerializeField] private GameObject guidImg;

    private bool isDragObj;
    private ScriptBox scriptBox;
    [Header("화살표 강조 ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;

    [Header("클릭/드래그 대상-입력")]
    [SerializeField] private GameObject clickBtn; // 클릭/드래그 대상

    [Header("그림자 패널 사용-입력")]
    [SerializeField] private bool panelChange; // 그림자 패널 사용 여부
    [SerializeField] private int panelChangeNum; // 그림자 패널 번호
    [SerializeField] GameObject shadowPanal; // 그림자 패널
    [SerializeField] private Sprite[] shadowImages; // 그림자 패널 리소스
    private BottomBar _bottomBar;
    private GameSceneManager _gameSceneManager;
    private SceneState[] sceneStates = { SceneState.Main, SceneState.Making, SceneState.CollectionDream, SceneState.Store };

    public override void Enter()
    {
        // 변수 초기화
        isDragObj = false;

        // 스크립트 박스 관리
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            int index = transform.GetSiblingIndex();
            if (index == 0)
            {
                scriptBox.gameObject.GetComponent<ScriptBox>().ScriptBoxOnOff(false);
            }
        }

        // 캔버스 불러오기
        canvas = GameObject.FindGameObjectWithTag("UI Canvas");
        _bottomBar = GameObject.FindGameObjectWithTag("BottomBar").GetComponent<BottomBar>();
        _gameSceneManager =GameSceneManager.Instance;

        // 화살표 생성
        

        // 화살표 생성
        arrowPrefab = Resources.Load<GameObject>("Prefabs/Tutorial/HighlightArrowPref");
        arrow = Instantiate(arrowPrefab, new Vector2(0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        arrow.GetComponent<Canvas>().worldCamera = Camera.main;
        GameObject arrowChild = arrow.transform.GetChild(0).gameObject;



        // 화살표 위치 조정
        RectTransform rectTransform = clickBtn.GetComponent<RectTransform>(); // 클릭 대상 위치
        if (rectTransform == null) return;
        Vector2 objectSize = rectTransform.sizeDelta;  // 버튼 사이즈
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);
        Vector3 topCenter = (objectCorners[1] + objectCorners[2]) / 2;
        Vector2 arrowSize = arrowChild.GetComponent<RectTransform>().sizeDelta;


        //Vector2 objectPosition = clickBtn.transform.position; //버튼 위치

        // 수정 전
        /*arrowPos = new Vector2(objectPosition.x, objectPosition.y + +0.3f);
        arrowChild.transform.position = arrowPos;
        arrowChild.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));*/

        // 수정 중
        /*RectTransform uiCanvasRectTransform = clickBtn.transform.root.GetComponent<RectTransform>();
        if(uiCanvasRectTransform == null) return;*/

        arrowPos = new Vector3(rectTransform.position.x, topCenter.y + arrowSize.y / 2, topCenter.z);
        Debug.Log("topCenter" + topCenter + "arrowSize" + arrowSize+"clickBtnPositionX"+rectTransform.position.x);

        //Debug.Log("objectSize : " + objectSize.y / 2 + " | arrowSize : " + arrowChild.GetComponent<RectTransform>().sizeDelta.y / 2 + " | arrowPos : " + objectPosition.y + ((objectSize.y / 2) + (arrowChild.GetComponent<RectTransform>().sizeDelta.y / 2)) + " | objPosition x : " + objectPosition.x + " | objPosition y : " + objectPosition.y);

        //arrowChild.transform.position = arrowPos;
        //arrowChild.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)rectTransform.parent, arrowPos, Camera.main, out canvasPos);
        arrowChild.GetComponent<RectTransform>().anchoredPosition = canvasPos;

        Debug.Log("afterArrowPos" + arrowChild.transform.position+"afterArrowPosLocal"+arrowChild.transform.localPosition);

        arrowChild.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

        // 화살표 깜박임 애니메이션 재생
        arrowChild.GetComponent<Animator>().enabled = true;
        arrowChild.GetComponent<Animator>().Play("blinkArrow");

        // 패널 이동시
        if (panelChange)
        {
            // 그림자 패널 생성
            shadowPanal = Instantiate(shadowPanal, new Vector2(0f, 0f), Quaternion.identity);
            shadowPanal.transform.SetParent(canvas.transform, false);
            shadowPanal.GetComponent<Image>().sprite = shadowImages[panelChangeNum]; // 패널 종류 선택
            // 버튼 설정
            _bottomBar.onClickRemove(panelChangeNum);
            clickBtn.gameObject.transform.SetParent(canvas.transform, true); // 버튼 가장 위로
            clickBtn.gameObject.transform.SetAsLastSibling(); // 버튼 가장 위로
        }
    }

    public override void Execute(TutorialController controller)
    {
        // 해당 버튼을 누르면
        if (clickBtn.GetComponent<TutorialButton>() != null && clickBtn.GetComponent<TutorialButton>().buttonClicked)
        {
            controller.SetNextTutorial(sceneStates[panelChangeNum]); // 다음 튜토리얼
        } // 해당 오브젝트를 드래그하면
        else if(clickBtn.GetComponent<TutorialDrag>()!=null&&clickBtn.GetComponent<TutorialDrag>().GetObjectDraged())
        {
            isDragObj = true;
            controller.SetNextTutorial(SceneState.None); // 다음 튜토리얼
        }
    }

    public override void Exit()
    {
        if (guidImg != null)
        {
            guidImg.SetActive(false);
        }
        if(isDragObj)
        {
            clickBtn.GetComponent<TutorialDrag>().SetObjctDraged(false);
        }
        Destroy(arrow); // 화살표 삭제
        if (panelChange)
        {
            Destroy(shadowPanal);
        }
    }
}

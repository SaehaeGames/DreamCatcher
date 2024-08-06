using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialClick : TutorialBase
{
    private GameObject arrow;
    private GameObject arrowPrefab; // 화살표 프리팹
    private GameObject canvas;

    private ScriptBox scriptBox;
    private Transform startParent;
    private GameObject duplicatedClickBtn;

    [Header("화살표 강조 ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;
    [SerializeField] private bool doClickButnDuplicate;
    [SerializeField] private Sprite arrowImg;

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

        if(highlightArrowOnOff)
        {
            if(doClickButnDuplicate)
            {
                duplicatedClickBtn = Instantiate(clickBtn, clickBtn.transform.position, clickBtn.transform.rotation);
            }
            // 화살표 생성
            arrowPrefab = Resources.Load<GameObject>("Prefabs/Tutorial/HighlightArrowPref");
            arrow = Instantiate(arrowPrefab, new Vector2(0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            arrow.GetComponent<Canvas>().worldCamera = Camera.main;
            GameObject arrowChild = arrow.transform.GetChild(0).gameObject;

            // 화살표 위치 조정
            arrowChild.GetComponent<Image>().sprite = arrowImg;

            // 클릭/드래그 대상 높이 조정
            if(doClickButnDuplicate)
            {
                duplicatedClickBtn.transform.SetParent(arrow.transform);
                duplicatedClickBtn.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                startParent = clickBtn.transform.parent;
                clickBtn.transform.SetParent(arrow.transform);
            }

            // 화살표 깜박임 애니메이션 재생
            arrowChild.GetComponent<Animator>().enabled = true;
            arrowChild.GetComponent<Animator>().Play("blinkArrow");
        }     

        // 패널 이동시
        if (panelChange)
        {
            // 그림자 패널 생성
            shadowPanal = Instantiate(shadowPanal, new Vector2(0f, 0f), Quaternion.identity);
            shadowPanal.transform.SetParent(canvas.transform, false);
            shadowPanal.GetComponent<Image>().sprite = shadowImages[panelChangeNum]; // 패널 종류 선택
            // 버튼 설정
            _bottomBar.onClickRemove(panelChangeNum);
        }
    }

    public override void Execute(TutorialController controller)
    {
        // 해당 버튼을 누르면
        if (clickBtn.GetComponent<TutorialButton>() != null)
        {
            if (doClickButnDuplicate && duplicatedClickBtn.GetComponent<TutorialButton>().buttonClicked)
            {
                Destroy(duplicatedClickBtn);
                controller.SetNextTutorial(sceneStates[panelChangeNum]); // 다음 튜토리얼
            }
            else if (clickBtn.GetComponent<TutorialButton>().buttonClicked)
            {
                clickBtn.transform.SetParent(startParent);
                controller.SetNextTutorial(sceneStates[panelChangeNum]); // 다음 튜토리얼
            }
        }
    }

    public override void Exit()
    {
        Destroy(arrow); // 화살표 삭제
        if (panelChange)
        {
            Destroy(shadowPanal);
        }
    }
}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialArrow : TutorialBase
{
    [SerializeField] private GameObject arrow; // 화살표 프리팹
    private Vector2 arrowPos;
    private GameObject canvas;
    [SerializeField] private GameObject guidImg;

    private bool isDragObj;

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
        isDragObj = false;
        // 캔버스 불러오기
        canvas = GameObject.FindGameObjectWithTag("UI Canvas");
        _bottomBar = GameObject.FindGameObjectWithTag("BottomBar").GetComponent<BottomBar>();
        _gameSceneManager=GameSceneManager.Instance;

        // 화살표 생성
        arrow = Instantiate(arrow, arrowPos, Quaternion.Euler(new Vector3(0f, 0f, 90f)));
        arrow.transform.SetParent(canvas.transform, false); // 캔버스 아래로 이동

        // 화살표 위치 조정
        Debug.Log("화살표 위치 조정");
        /*
        RectTransform rectTransform = clickBtn.GetComponent<RectTransform>();
        if (rectTransform == null) return;
        Debug.Log("화살표 위치 조정111");
        Vector2 objectSize = rectTransform.rect.size;*/
        Vector2 objectPosition = clickBtn.transform.position;
        arrowPos = new Vector2(objectPosition.x, objectPosition.y + +0.3f); //clickBtn.transform.position.y+0.3f
        //arrowPos = new Vector2(objectPosition.x, objectPosition.y+objectSize.y/2+arrow.GetComponent<RectTransform>().rect.size.y/2); //clickBtn.transform.position.y+0.3f
        //Debug.Log("objectSize : " + objectSize + " | arrowSize : " + arrow.GetComponent<RectTransform>().rect.size + " | arrowPos : " + arrowPos);*/
        arrow.transform.position = arrowPos;

        // 화살표 깜박임 애니메이션 재생
        arrow.GetComponent<Animator>().enabled = true;
        arrow.GetComponent<Animator>().Play("blinkArrow");

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

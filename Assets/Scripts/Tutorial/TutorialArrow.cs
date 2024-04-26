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

    [Header("클릭/드래그 대상-입력")]
    [SerializeField] private GameObject clickBtn; // 클릭/드래그 대상

    [Header("그림자 패널 사용-입력")]
    [SerializeField] private bool panelChange; // 그림자 패널 사용 여부
    [SerializeField] private int panelChangeNum; // 그림자 패널 번호
    [SerializeField] GameObject shadowPanal; // 그림자 패널
    [SerializeField] private Sprite[] shadowImages; // 그림자 패널 리소스

    public override void Enter()
    {
        // 캔버스 불러오기
        canvas = GameObject.FindObjectOfType<Canvas>().gameObject;

        // 화살표 생성
        arrow = Instantiate(arrow, arrowPos, Quaternion.Euler(new Vector3(0f, 0f, 90f)));
        arrow.transform.SetParent(canvas.transform, false); // 캔버스 아래로 이동
        // 화살표 위치 조정
        arrowPos = new Vector2(clickBtn.transform.position.x, clickBtn.transform.position.y+0.3f);
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
            clickBtn.gameObject.transform.SetAsLastSibling(); // 버튼 가장 위로
        }
    }

    public override void Execute(TutorialController controller)
    {
        // 해당 버튼을 누르면
        if (clickBtn.GetComponent<TutorialButton>() != null && clickBtn.GetComponent<TutorialButton>().buttonClicked)
        {
            controller.SetNextTutorial(); // 다음 튜토리얼
        } // 해당 오브젝트를 드래그하면
        else if(clickBtn.GetComponent<TutorialDrag>()!=null&&clickBtn.GetComponent<TutorialDrag>().objectDraged)
        {
            controller.SetNextTutorial(); // 다음 튜토리얼
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

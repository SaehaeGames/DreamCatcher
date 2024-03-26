using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialArrow : TutorialBase
{
    [SerializeField]
    private GameObject arrow;
    [SerializeField]
    private GameObject clickBtn;

    private Vector2 arrowPos;
    private GameObject canvas;

    [SerializeField]
    private bool panelChange;
    [SerializeField]
    private int panelChangeNum;
    [SerializeField]
    GameObject shadowPanal;
    [SerializeField]
    private Sprite[] shadowImages;

    public override void Enter()
    {
        // 캔버스 불러오기
        canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        // 화살표 생성
        arrowPos = new Vector2(clickBtn.transform.position.x, clickBtn.transform.position.y + 85f);
        arrow = Instantiate(arrow, arrowPos, Quaternion.Euler(new Vector3(0f, 0f, 90f)));
        arrow.transform.SetParent(canvas.transform, true); // 캔버스 아래로 이동
        // 화살표 깜박임 애니메이션 재생
        arrow.GetComponent<Animator>().enabled = true;
        arrow.GetComponent<Animator>().Play("blinkArrow");
        // 패널 이동시
        if (panelChange)
        {
            shadowPanal = Instantiate(shadowPanal, new Vector2(0f, 0f), Quaternion.identity);
            shadowPanal.transform.SetParent(canvas.transform, false);
            shadowPanal.GetComponent<Image>().sprite = shadowImages[panelChangeNum];
            clickBtn.gameObject.transform.SetParent(canvas.transform, true);
        }
    }

    public override void Execute(TutorialController controller)
    {
        // 해당 버튼을 누르면
        if (clickBtn.GetComponent<TutorialButton>().buttonClicked)
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

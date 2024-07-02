using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPoint : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 매니저
    private DCCheckManager DCManager;
    // 프리팹
    public GameObject LinePref;

    // 오브젝트
    GameObject Line;
    public GameObject LinesGroup;
    //public GameObject Canvas;

    // 위치
    Vector3 StartPoint, EndPoint;
    private Vector3 mousePos;

    // 색
    private Color stringColor;
    private Color[] stringColors = { new Color(1f, 1f, 1f, 1f), new Color(1f, 0.9254903f, 0.4784314f, 1f),
        new Color(0.3333333f, 0.345098f, 0.4705883f, 1f), new Color(0.6078432f, 0.2235294f, 0.2352941f, 1f),
        new Color(0.3411765f, 0.3411765f, 0.3411765f, 1f) };

    // 포인터 저장
    public int PointNumber;
    private int startPointNum, endPointNum;
    private float addDegree = 0.04f;

    // 제작시작여부 판단
    private MakingUIManager makingUiManager;

    private TutorialDragPointLimit _tutorialDragPointLimit;

    private void OnEnable()
    {
        makingUiManager = GameObject.FindGameObjectWithTag("CreateManager").GetComponent<MakingUIManager>();
        _tutorialDragPointLimit = this.gameObject.GetComponent<TutorialDragPointLimit>();
    }

    // Start is called before the first frame update
    void Start()
    {
        DCManager = GameObject.FindWithTag("CreateManager").gameObject.GetComponent<DCCheckManager>();
        //StartPoint = Camera.main.ScreenToWorldPoint(this.transform.position);
        stringColor = stringColors[0];
        StartPoint = this.transform.position;
        startPointNum = PointNumber;
    }

    public void StringColorSet(int colorNum)
    {
        stringColor = stringColors[colorNum];
    }

    private Vector3 PointResetting(Vector3 StartPnt, Vector3 EndPnt)
    {
        Vector3 addPnt = (StartPnt - EndPnt).normalized;
        return addPnt;
    }

    // 오브젝트 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_MakingOrFeather();   //제작, 선긋기 효과음
        // 라인 생성
        Line = Instantiate(LinePref);
        // 색 변경
        Line.GetComponent<LineRenderer>().startColor = stringColor;
        Line.GetComponent<LineRenderer>().endColor = stringColor;
        // 자식부모 설정
        Line.transform.parent = LinesGroup.gameObject.transform;
        Line.GetComponent<LineRenderer>().sortingOrder = 1;
        // 시작점 설정
        Line.GetComponent<LineRenderer>().SetPosition(0, StartPoint);
        Debug.Log(StartPoint);
        Line.GetComponent<Line>().startNum = PointNumber;
    }

    // 오브젝트 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 90;
        Line.GetComponent<LineRenderer>().SetPosition(1, mousePos);

        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("DragPoint"))
        {
            // 닿으면 거기서 한줄 완성
            endPointNum = eventData.pointerCurrentRaycast.gameObject.GetComponent<DragPoint>().PointNumber;
            EndPoint = eventData.pointerCurrentRaycast.gameObject.transform.position;
            Line.GetComponent<LineRenderer>().SetPosition(0, StartPoint - PointResetting(StartPoint, EndPoint) * -addDegree);
            Line.GetComponent<LineRenderer>().SetPosition(1, EndPoint - PointResetting(StartPoint, EndPoint) * addDegree);
        }
    }

    // 오브젝트 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_MakingOrFeather();   //제작, 선긋기 효과음
        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("DragPoint"))
        {
            endPointNum = eventData.pointerCurrentRaycast.gameObject.GetComponent<DragPoint>().PointNumber;

            if (!DCManager.CheckDreamCatcher(startPointNum, endPointNum))
            {
                Debug.Log("처음 연결");
                // 튜토리얼
                if (_tutorialDragPointLimit != null)
                {
                    if (!_tutorialDragPointLimit.ReceiveEndPointNum(endPointNum))
                    {
                        Debug.Log("틀림");
                        Destroy(Line);
                        Line = null;
                        return;
                    }
                }

                // 닿으면 거기서 한줄 완성
                EndPoint = eventData.pointerCurrentRaycast.gameObject.transform.position;
                Line.GetComponent<LineRenderer>().SetPosition(1, EndPoint - PointResetting(StartPoint, EndPoint) * addDegree);
                Line.GetComponent<LineRenderer>().SetPosition(0, StartPoint - PointResetting(StartPoint, EndPoint) * -addDegree);

                Line.GetComponent<Line>().addColliderToLine();

                // 제작 시작 알림
                makingUiManager.StartMakingLine();
                // 라인 저장
                Line.GetComponent<Line>().endNum = eventData.pointerCurrentRaycast.gameObject.GetComponent<DragPoint>().PointNumber;
                DCManager.UpdateDreamCatcher(startPointNum, endPointNum);
            }
            else
            {
                Debug.Log("두 번 연결");
            }
        }
        else
        {
            Destroy(Line);
        }
    }
}

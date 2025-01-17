using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveSequenceDrag : InteractiveSequenceBase
{
    private GameObject arrow;
    private GameObject arrowPrefab;
    private GameObject canvas;
    private Transform startParent;

    [Header("화살표 강조 ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;
    [SerializeField] private Sprite arrowImg;

    [Header("클릭/드래그 대상-입력")]
    [SerializeField] private GameObject dragObj; // 클릭/드래그 대상

    public override void Enter()
    {
        canvas = GameObject.FindGameObjectWithTag("UI Canvas");

        if (highlightArrowOnOff)
        {
            // 화살표 생성
            arrowPrefab = Resources.Load<GameObject>("Prefabs/Tutorial/HighlightArrowPref");
            arrow = Instantiate(arrowPrefab, new Vector2(0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            arrow.GetComponent<Canvas>().worldCamera = Camera.main;
            GameObject arrowChild = arrow.transform.GetChild(0).gameObject;

            // 화살표 위치 조정
            arrowChild.GetComponent<Image>().sprite = arrowImg;
            startParent = dragObj.transform.parent;
            
            dragObj.transform.SetParent(arrow.transform);

            // 타겟 부모 조정
            dragObj.GetComponent<InteractiveDragObj>().SetTargetParent(arrow.transform);

            // 화살표 깜박임 애니메이션 재생
            arrowChild.GetComponent<Animator>().enabled = true;
            arrowChild.GetComponent<Animator>().Play("blinkArrow");
        }
        else
        {
            startParent = dragObj.transform.parent;
            dragObj.GetComponent<InteractiveDragObj>().SetTargetParent(canvas.transform);
        }
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        // 해당 오브젝트가 드래그 오브젝트라면
        if (dragObj.GetComponent<InteractiveDragObj>() != null)
        {
            // 드래그가 완료되었다면
            if (dragObj.GetComponent<InteractiveDragObj>().GetObjectDraged())
            {
                dragObj.transform.SetParent(startParent);
                tutorialPipeline.SetNextTutorial(SceneState.None); // 다음 InteractiveSequence
            }
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        // 해당 오브젝트가 드래그 오브젝트라면
        if (dragObj.GetComponent<InteractiveDragObj>() != null)
        {
            // 드래그가 완료되었다면
            if (dragObj.GetComponent<InteractiveDragObj>().GetObjectDraged())
            {
                Debug.Log("<color=red>드래그 튜토리얼 완료</color>");
                dragObj.transform.SetParent(startParent);
                questActionPipeline.SetNextQuestAction(); // 다음 토리얼
            }
        }
    }

    public override void Exit()
    {
        dragObj.GetComponent<InteractiveDragObj>().SetObjctDraged(false);
        Destroy(arrow); // 화살표 삭제
    }
}

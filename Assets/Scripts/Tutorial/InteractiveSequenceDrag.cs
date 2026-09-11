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
    private InteractiveDragObj interactiveDragObj;

    [Header("화살표 강조 ON/OFF")]
    [SerializeField] private bool highlightArrowOnOff;
    [SerializeField] private Sprite arrowImg;

    [Header("클릭/드래그 대상-입력")]
    [SerializeField] private GameObject dragObj; // 클릭/드래그 대상

    public override void Enter()
    {
        if (dragObj == null)
        {
            Debug.LogError($"[InteractiveSequenceDrag] {gameObject.name}의 dragObj가 설정되지 않았습니다.");
            return;
        }

        interactiveDragObj = dragObj.GetComponent<InteractiveDragObj>();
        if (interactiveDragObj == null)
        {
            Debug.LogError($"[InteractiveSequenceDrag] {dragObj.name}에 InteractiveDragObj가 없습니다.");
            return;
        }

        canvas = GameObject.FindGameObjectWithTag("UI Canvas");

        if (highlightArrowOnOff)
        {
            // TutorialOverlay에서 화살표 가져오기 (Click 방식과 동일)
            GameObject tutorialOverlay = GameObject.FindGameObjectWithTag("TutorialOverlay");
            arrow = tutorialOverlay.transform.GetChild(1).gameObject;
            GameObject blockPanal = tutorialOverlay.transform.GetChild(0).gameObject;

            arrow.SetActive(true);
            blockPanal.SetActive(true);
            arrow.GetComponent<Image>().sprite = arrowImg;

            startParent = dragObj.transform.parent;
            dragObj.transform.SetParent(arrow.transform);

            // 타겟 부모 조정
            interactiveDragObj.SetTargetParent(arrow.transform);

            // 화살표 깜박임 애니메이션
            arrow.GetComponent<Animator>().enabled = true;
            arrow.GetComponent<Animator>().Play("blinkArrow");
        }
        else
        {
            startParent = dragObj.transform.parent;
            interactiveDragObj.SetTargetParent(canvas.transform);
        }
    }

    public override void Execute(TutorialPipeline tutorialPipeline)
    {
        // 해당 오브젝트가 드래그 오브젝트라면
        if (interactiveDragObj != null)
        {
            // 드래그가 완료되었다면
            if (interactiveDragObj.GetObjectDraged())
            {
                dragObj.transform.SetParent(startParent);
                tutorialPipeline.SetNextTutorial(SceneState.None); // 다음 InteractiveSequence
            }
        }
    }

    public override void Execute(QuestActionPipeline questActionPipeline)
    {
        // 해당 오브젝트가 드래그 오브젝트라면
        if (interactiveDragObj != null)
        {
            // 드래그가 완료되었다면
            if (interactiveDragObj.GetObjectDraged())
            {
                Debug.Log("<color=red>드래그 튜토리얼 완료</color>");
                dragObj.transform.SetParent(startParent);
                questActionPipeline.SetNextQuestAction(); // 다음 토리얼
            }
        }
    }

    public override void Exit()
    {
        /*        dragObj.GetComponent<InteractiveDragObj>().SetObjctDraged(false);
                Destroy(arrow); // 화살표 삭제*/

        if (interactiveDragObj != null)
        {
            interactiveDragObj.SetObjctDraged(false);
            interactiveDragObj.ClearTargetConfiguration();
        }

        if (dragObj != null && startParent != null)
        {
            dragObj.transform.SetParent(startParent);
        }

        if (highlightArrowOnOff && arrow != null)
        {
            arrow.GetComponent<Image>().sprite = null;
            arrow.SetActive(false);
            GameObject tutorialOverlay = GameObject.FindGameObjectWithTag("TutorialOverlay");
            if (tutorialOverlay != null)
                tutorialOverlay.transform.GetChild(0).gameObject.SetActive(false); // BlockPanal off
        }
    }
}

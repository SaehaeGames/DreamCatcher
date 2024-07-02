using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialDragPointLimit : MonoBehaviour
{
    // 플레이어 데이터 
    private PlayerDataContainer curPlayerData;   //플레이어 데이터 정보
    private int curScene;

    // 포인트 데이터
    public int targetEndPoint1, targetEndPoint2;
    private int startPoint, endPoint;

    private TutorialConnectLine _tutorialConnectLine;

    // Start is called before the first frame update
    void Start()
    {
        _tutorialConnectLine=GameObject.FindFirstObjectByType<TutorialConnectLine>();
        // 튜토리얼 씬 상황 확인
        curPlayerData = GameManager.instance.loadPlayerData;    //플레이어의 상단바 데이터 정보를 가져옴
        curScene = (int)curPlayerData.dataList[7].dataNumber;

        // 튜토리얼 드래그 포인트 활성/비활성화
        if (curScene != 8) this.GetComponent<TutorialDragPointLimit>().enabled = false;
        Debug.Log("튜토리얼 드래그 포인트 활성화");

        // startPoint 데이터 가져오기
        startPoint = this.gameObject.GetComponent<DragPoint>().PointNumber;
    }

    public bool ReceiveEndPointNum(int endPointNum)
    {
        endPoint = endPointNum;
        if (endPoint == targetEndPoint1 || endPoint == targetEndPoint2)
        {
            _tutorialConnectLine.PlusNumberOfTimesCorrect();
            return true;
        }
        else
        {
            return false;
        }
    }
}

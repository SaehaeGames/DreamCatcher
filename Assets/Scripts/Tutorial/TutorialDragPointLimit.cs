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
        if (_tutorialConnectLine == null)
        {
            _tutorialConnectLine = GameObject.FindFirstObjectByType<TutorialConnectLine>();
        }
        endPoint = endPointNum;
        if (targetEndPoint1 == targetEndPoint2)
        {
            if (endPoint == targetEndPoint1)
            {
                if(_tutorialConnectLine==null)
                {
                    Debug.LogError("_tutorialConnectLine이 존재하지 않습니다.");
                }
                _tutorialConnectLine.PlusNumberOfTimesCorrect();
                Debug.Log("연결 성공 타겟 같음");
                return true;
            }
            else
            {
                Debug.Log("연결 실패 타겟 같음");
                return false;
            }
        }
        else
        {
            if (endPoint == targetEndPoint1 || endPoint == targetEndPoint2)
            {
                if (_tutorialConnectLine == null)
                {
                    Debug.LogError("_tutorialConnectLine이 존재하지 않습니다.");
                }
                _tutorialConnectLine.PlusNumberOfTimesCorrect();
                Debug.Log("연결 성공 타겟 다름");
                return true;
            }
            else
            {
                Debug.Log("연결 성공 타겟 다름");
                return false;
            }
        }
        
    }
}

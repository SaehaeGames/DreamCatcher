using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialDragPointLimit : MonoBehaviour
{
    // �÷��̾� ������ 
    private PlayerDataContainer curPlayerData;   //�÷��̾� ������ ����
    private int curScene;

    // ����Ʈ ������
    public int targetEndPoint1, targetEndPoint2;
    private int startPoint, endPoint;

    private TutorialConnectLine _tutorialConnectLine;

    // Start is called before the first frame update
    void Start()
    {
        _tutorialConnectLine=GameObject.FindFirstObjectByType<TutorialConnectLine>();
        // Ʃ�丮�� �� ��Ȳ Ȯ��
        curPlayerData = GameManager.instance.loadPlayerData;    //�÷��̾��� ��ܹ� ������ ������ ������
        curScene = (int)curPlayerData.dataList[7].dataNumber;

        // Ʃ�丮�� �巡�� ����Ʈ Ȱ��/��Ȱ��ȭ
        if (curScene != 8) this.GetComponent<TutorialDragPointLimit>().enabled = false;
        Debug.Log("Ʃ�丮�� �巡�� ����Ʈ Ȱ��ȭ");

        // startPoint ������ ��������
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

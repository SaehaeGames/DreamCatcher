using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractiveLimitDragPoint : MonoBehaviour
{
    // �÷��̾� ������ 
    private PlayerDataContainer curPlayerData;   //�÷��̾� ������ ����
    private int curScene;

    // ����Ʈ ������
    public int targetEndPoint1, targetEndPoint2;
    private int startPoint, endPoint;

    private InteractiveSequenceConnectLine _tutorialConnectLine;

    // Start is called before the first frame update
    void Start()
    {
        _tutorialConnectLine=GameObject.FindFirstObjectByType<InteractiveSequenceConnectLine>();
        // Ʃ�丮�� �� ��Ȳ Ȯ��
        curPlayerData = GameManager.instance.loadPlayerData;    //�÷��̾��� ��ܹ� ������ ������ ������
        curScene = (int)curPlayerData.dataList[7].dataNumber;

        // Ʃ�丮�� �巡�� ����Ʈ Ʃ�丮�� ���� �ܿ� ��Ȱ��ȭ
        if (curScene != 8) this.GetComponent<InteractiveLimitDragPoint>().enabled = false;

        // startPoint ������ ��������
        startPoint = this.gameObject.GetComponent<DragPoint>().PointNumber;
    }

    public bool ReceiveEndPointNum(int endPointNum)
    {
        if (_tutorialConnectLine == null)
        {
            _tutorialConnectLine = GameObject.FindFirstObjectByType<InteractiveSequenceConnectLine>();
        }
        endPoint = endPointNum;
        if (targetEndPoint1 == targetEndPoint2)
        {
            if (endPoint == targetEndPoint1)
            {
                if(_tutorialConnectLine==null)
                {
                    Debug.LogError("_tutorialConnectLine�� �������� �ʽ��ϴ�.");
                }
                _tutorialConnectLine.PlusNumberOfTimesCorrect();
                Debug.Log("���� ���� Ÿ�� ����");
                return true;
            }
            else
            {
                Debug.Log("���� ���� Ÿ�� ����");
                return false;
            }
        }
        else
        {
            if (endPoint == targetEndPoint1 || endPoint == targetEndPoint2)
            {
                if (_tutorialConnectLine == null)
                {
                    Debug.LogError("_tutorialConnectLine�� �������� �ʽ��ϴ�.");
                }
                _tutorialConnectLine.PlusNumberOfTimesCorrect();
                Debug.Log("���� ���� Ÿ�� �ٸ�");
                return true;
            }
            else
            {
                Debug.Log("���� ���� Ÿ�� �ٸ�");
                return false;
            }
        }
        
    }
}

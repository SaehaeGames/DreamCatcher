using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestActionManager : MonoBehaviour
{
    private PlayerDataManager _playerDataContainer;
    private int currentQuestNum;
    public Transform[] quests;

    public event Action OnSetQuestStartActive;
    public event Action OnSetQuestEndActive;


    private void Awake()
    {
        // ����Ʈ �׼� ���� �ľ�
        int questCount = this.transform.childCount;
        quests = new Transform[questCount];

        // ��� ����Ʈ �׼� ��Ȱ��ȭ
        for(int i=0; i<this.transform.childCount; i++)
        {
            quests[i] = this.transform.GetChild(i); // ����Ʈ �׼� ����
            quests[i].GetChild(1).gameObject.SetActive(false); // End ��Ȱ��ȭ
            quests[i].GetChild(0).gameObject.SetActive(false); // Start ��Ȱ��ȭ
        }
    }

    void Start()
    {
        // �÷��̾� ������(PlayerDataFile) �ε�
        _playerDataContainer = GameManager.instance.playerDataManager;

        // ���� ����Ʈ ��ȣ �ҷ�����
        currentQuestNum = (int)_playerDataContainer.dataList[8].dataNumber; // ������ ����Ʈ ��ȣ �ҷ�����
    }

    private void OnEnable()
    {
        OnSetQuestStartActive += HandleSetQuestStartActive;
        OnSetQuestEndActive += HandleSetQuestEndActive;
    }

    private void OnDisable()
    {
        OnSetQuestStartActive -= HandleSetQuestStartActive;
        OnSetQuestEndActive -= HandleSetQuestEndActive;
    }

    public void HandleSetQuestStartActive()
    {
        currentQuestNum = (int)_playerDataContainer.dataList[8].dataNumber; // ������ ����Ʈ ��ȣ �ҷ�����
        int questAccepted = (int)_playerDataContainer.dataList[9].dataNumber; // ���� ����Ʈ ���� �������� �ҷ�����
        if (currentQuestNum >= 3 && currentQuestNum < quests.Length + 3 && questAccepted == 0)
        {
            Transform startObject = quests[currentQuestNum - 3].GetChild(0);
            
            if (startObject != null)
            {
                Debug.Log("<color=cyan>Start QuestAction Ȱ��ȭ</color>");
                startObject.gameObject.SetActive(true); // Start Ȱ��ȭ

                // ����Ʈ ���� ���� ������Ʈ
                _playerDataContainer.dataList[9].dataNumber = 1; // [����Ʈ ��]���� ����
                GameManager.instance.GetComponent<JsonManager>().SaveData(Constants.PlayerDataFile, _playerDataContainer);
            }
        }
    }

    public void HandleSetQuestEndActive()
    {
        currentQuestNum = (int)_playerDataContainer.dataList[8].dataNumber; // ������ ����Ʈ ��ȣ �ҷ�����
        int questAccepted = (int)_playerDataContainer.dataList[9].dataNumber; // ���� ����Ʈ ���� �������� �ҷ�����
        if (currentQuestNum >= 3 && currentQuestNum < quests.Length + 3 && questAccepted == 1)
        {
            Transform endObject = quests[currentQuestNum - 3].GetChild(1);
            if (endObject != null)
            {
                Debug.Log("<color=cyan>Start QuestAction ��Ȱ��ȭ</color>");
                endObject.gameObject.SetActive(true); // End Ȱ��ȭ 

                // ����Ʈ ���� ���� ������Ʈ
                _playerDataContainer.dataList[9].dataNumber = 0; // [����Ʈ ����]���� ����
                GameManager.instance.GetComponent<JsonManager>().SaveData(Constants.PlayerDataFile, _playerDataContainer);
            }
        }
    }


}

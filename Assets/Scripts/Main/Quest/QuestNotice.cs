using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNotice : MonoBehaviour
{
    //퀘스트 알림 표시를 띄우고 끄는 스크립트
    [Header("[Quest Notice]")]
    public GameObject[] noticeObjects;    //알림 오브젝트들

    private PlayerDataManager playerDataManager;

    private void Awake()
    {
        playerDataManager = GameManager.instance.playerDataManager;
    }

    private void Start()
    {
        InitCheckedNotice();
    }

    private void InitCheckedNotice()
    {
        for (int i = 0; i < noticeObjects.Length; i++)
        {
            bool checkedNotice = playerDataManager.GetQuestNoticeRead((QuestType)i);
            noticeObjects[i].SetActive(checkedNotice);
        }
    }

    public void Notice(QuestType type)
    {
        playerDataManager.SetQuestNoticeRead(type, true);
        noticeObjects[(int)type].SetActive(true);
    }

    public void CloseNotice(QuestType type)
    {
        playerDataManager.SetQuestNoticeRead(type, false);
        noticeObjects[(int)type].SetActive(false);
    }
}

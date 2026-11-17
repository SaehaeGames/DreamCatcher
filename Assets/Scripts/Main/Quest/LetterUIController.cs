using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterUIController : MonoBehaviour
{
    //퀘스트 알림 표시를 띄우고 끄는 스크립트
    [Header("[Quest Notice]")]
    public GameObject[] letterObjects;
    public GameObject[] noticeObjects;    //알림 오브젝트들
    private PlayerDataManager playerDataManager;

    private void Awake()
    {
        playerDataManager = GameManager.instance.playerDataManager;
    }

    private void Start()
    {
        InitUnlockedLetter();
        InitCheckedNotice();
    }

    private void InitUnlockedLetter()
    {
        letterObjects = new GameObject[transform.childCount];
        // 오브젝트 초기화
        for (int i = 0; i < transform.childCount; i++)
        {
            letterObjects[i] = transform.GetChild(i).gameObject;
        }

        // 해금 여부에 따른 UI 표시
        letterObjects[0].SetActive(true); // 메인 퀘스트
        letterObjects[1].SetActive(playerDataManager.GetIsRepeatQuestUnlocked()); // 반복 퀘스트
        letterObjects[2].SetActive(playerDataManager.GetIsCharonLetterUnlocked()); // 카론 퀘스트
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

    public void UnlockLetter(QuestType type)
    {
        letterObjects[(int)type].SetActive(true);
    }
}

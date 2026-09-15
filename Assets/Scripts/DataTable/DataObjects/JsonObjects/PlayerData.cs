using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int gold;
    public int dreamMarble;
    public int specialFeed;

    public float bgmVolume;
    public float effectVolume;
    public bool bgmMute;
    public bool effectMute;

    public int currentScene;
    public int currentMainQuestIndex;
    public bool isMainQuestActionPlaying;

    public int currentCharonLetterIndex;
    public bool isCharonQuestActionPlaying;

    // 퀘스트 알림 확인 여부
    // 0 : 메인 퀘스트
    // 1 : 반복 퀘스트
    // 2 : 카론 퀘스트
    public bool[] questNotice;

    public UnlockData unlockData = new UnlockData();

    public PlayerData()
    {
        gold = 0;
        dreamMarble = 0;
        specialFeed = 0;
        bgmVolume = 0;
        effectVolume = 0;
        bgmMute = false;
        effectMute = false;
        currentScene = 0;
        currentMainQuestIndex = 0;
        isMainQuestActionPlaying = false;
        currentCharonLetterIndex = -1;
        isCharonQuestActionPlaying = false;

        questNotice = new bool[3] { false, false, false };

        unlockData = new UnlockData(false, false, false, 0);
    }

    public PlayerData(
        int _gold,
        int _dreamMarble,
        int _specialFeed,
        float _bgmVolume,
        float _effectVolume,
        bool _bgmMute,
        bool _effectMute,
        int _currentScene,
        int _currentMainQuestIndex,
        bool _isQuestActionPlaying,
        UnlockData _unlockData,
        int _currentCharonLetterIndex,
        bool _isCharonQuestActionPlaying,
        bool[] _questNoticeRead)
    {
        gold = _gold;
        dreamMarble = _dreamMarble;
        specialFeed = _specialFeed;
        bgmVolume = _bgmVolume;
        effectVolume = _effectVolume;
        bgmMute = _bgmMute;
        effectMute = _effectMute;

        currentScene = _currentScene;
        currentMainQuestIndex = _currentMainQuestIndex;
        isMainQuestActionPlaying = _isQuestActionPlaying;
        unlockData = _unlockData;
        currentCharonLetterIndex = _currentCharonLetterIndex;
        isCharonQuestActionPlaying = _isCharonQuestActionPlaying;

        questNotice = _questNoticeRead;
    }

    #region Set 함수

    public void SetGold(int _gold) { gold = _gold; }
    public void SetDreamMarble(int _dreamMarble) { dreamMarble = _dreamMarble; }
    public void SetSpecialFeed(int _specialFeed) { specialFeed = _specialFeed; }
    public void SetBGMVolume(float _bgmVolume) { bgmVolume = _bgmVolume; }
    public void SetEffectVolume(float _effectVolume) { effectVolume = _effectVolume; }
    public void SetBGMMute(bool _bgmMute) { bgmMute = _bgmMute; }
    public void SetEffectMute(bool _effectMute) { effectMute = _effectMute; }
    public void SetCurrentScene(int _currentScene) { currentScene = _currentScene; }
    public void SetIsQuestActionPlaying(bool _isQuestActionPlaying) { isMainQuestActionPlaying = _isQuestActionPlaying; }
    public void SetCurrentMainQuestIndex(int _currentMainQuestIndex) { currentMainQuestIndex = _currentMainQuestIndex; }
    public void SetCurrentCharonLetterIndex(int _currentCharonLetterIndex) { currentCharonLetterIndex = _currentCharonLetterIndex; }
    public void SetIsCharonQuestActionPlaying(bool _isCharonQuestActionPlaying) { isCharonQuestActionPlaying = _isCharonQuestActionPlaying; }

    public void SetQuestNoticeRead(int _questType, bool _isRead)
    {
        questNotice[_questType] = _isRead;
    }

    #endregion

    #region Get 함수

    public int GetGold() { return gold; }
    public int GetDreamMarble() { return dreamMarble; }
    public int GetSpecialFeed() { return specialFeed; }
    public float GetBgmVolume() { return bgmVolume; }
    public float GetEffectVolume() { return effectVolume; }
    public bool GetBgmMute() { return bgmMute; }
    public bool GetEffectMute() { return effectMute; }
    public int GetCurrentScene() { return currentScene; }
    public bool GetIsQuestActionPlaying() { return isMainQuestActionPlaying; }
    public int GetCurrentMainQuestIndex() { return currentMainQuestIndex; }
    public int GetCurrentCharonLetterIndex() { return currentCharonLetterIndex; }
    public bool GetCurrentIsCharonQuestActionPlaying() { return isCharonQuestActionPlaying; }

    public bool GetQuestNoticeRead(int _questType)
    {
        return questNotice[_questType];
    }

    #endregion
}
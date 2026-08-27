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
        unlockData = new UnlockData(false, false, false, 0);
        isMainQuestActionPlaying = false;
    }

    public PlayerData(int _gold, int _dreamMarble, int _specialFeed, float _bgmVolume, float _effectVolume, bool _bgmMute, bool _effectMute, int _currentScene, int _currentMainQuestIndex, bool _isQuestActionPlaying, UnlockData _unlockData) 
    { 
        gold = _gold;
        dreamMarble = _dreamMarble;
        specialFeed = _specialFeed;
        bgmVolume = _bgmVolume;
        effectVolume = _effectVolume;
        bgmMute = _bgmMute;
        effectMute = _bgmMute;
        currentScene = _currentScene;
        currentMainQuestIndex = _currentMainQuestIndex;
        isMainQuestActionPlaying = _isQuestActionPlaying;
        unlockData = _unlockData;
    }

    # region Set 함수

    public void SetGold(int _gold) { gold = _gold; }
    public void SetDreamMarble(int _dreamMarble) { dreamMarble= _dreamMarble; }
    public void SetSpecialFeed(int _specialFeed) { specialFeed = _specialFeed; }
    public void SetBGMVolume(float _bgmVolume) { bgmVolume = _bgmVolume; }
    public void SetEffectVolume(float _effectVolume) { effectVolume = _effectVolume; }
    public void SetBGMMute(bool _bgmMute) { bgmMute = _bgmMute; }
    public void SetEffectMute(bool _effectMute) { effectMute = _effectMute; }
    public void SetCurrentScene(int _currentScene) { currentScene = _currentScene; }
    public void SetIsQuestActionPlaying(bool _isQuestActionPlaying) { isMainQuestActionPlaying = _isQuestActionPlaying; }
    public void SetCurrentMainQuestIndex(int _currentMianQuestIndex) { currentMainQuestIndex = _currentMianQuestIndex; }
    # endregion

    # region Get 함수

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

    # endregion
}


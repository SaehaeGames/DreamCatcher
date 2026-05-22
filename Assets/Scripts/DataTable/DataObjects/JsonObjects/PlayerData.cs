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
    }

    public PlayerData(int _gold, int _dreamMarble, int _specialFeed, float _bgmVolume, float _effectVolume, bool _bgmMute, bool _effectMute, int _currentScene, int _currentMainQuestIndex) 
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
    public int GetCurrentMainQuestIndex() { return currentMainQuestIndex; }

    # endregion
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Diagnostics;


public class PlayerDataManager
{
    private JsonManager jsonManager = new JsonManager();
    private PlayerData playerData;


    public PlayerDataManager()
    {
        ResetData();
    }
     
    public void ResetData()
    {
        playerData = new PlayerData()
        {
            gold = 30000,
            dreamMarble = 0,
            specialFeed = 100,

            bgmVolume = 1f,
            effectVolume = 1f,

            bgmMute = false,
            effectMute = false,

            currentScene = 0,
            currentMainQuestIndex = 0,
            isQuestActionPlaying = false,

            unlockData = new UnlockData(false, false, false, 0)
        };
    }

    #region Currency

    public int GetGold()
    {
        return playerData.gold;
    }

    public void AddGold(int amount)
    {
        playerData.gold += amount;
        Save();
    }

    public bool UseGold(int amount)
    {
        if (playerData.gold < amount)
        {
            UnityEngine.Debug.LogWarning("[PlayerDataManager] Not enough gold.");
            return false;
        }

        playerData.gold -= amount;
        Save();
        return true;
    }

    public int GetDreamMarble()
    {
        return playerData.dreamMarble;
    }

    public void AddDreamMarble(int amount)
    {
        playerData.dreamMarble += amount;
        Save();
    }

    public int GetSpecialFeed()
    {
        return playerData.specialFeed;
    }

    public void AddSpecialFeed(int amount)
    {
        playerData.specialFeed += amount;
        Save();
    }

    public bool UseSpecialFeed(int amount)
    {
        if (playerData.specialFeed < amount)
        {
            UnityEngine.Debug.LogWarning("[PlayerDataManager] Not enough specialFeed.");
            return false;
        }

        playerData.specialFeed -= amount;
        Save();
        return true;
    }

    #endregion

    #region Sound

    public float GetBGMVolume()
    {
        return playerData.bgmVolume;
    }

    public void SetBGMVolume(float volume)
    {
        playerData.bgmVolume = Mathf.Clamp01(volume);
        Save();
    }

    public float GetEffectVolume()
    {
        return playerData.effectVolume;
    }

    public void SetEffectVolume(float volume)
    {
        playerData.effectVolume = Mathf.Clamp01(volume);
        Save();
    }

    public bool GetBGMMute()
    {
        return playerData.bgmMute;
    }

    public void SetBGMMute(bool mute)
    {
        playerData.bgmMute = mute;
        Save();
    }

    public bool GetEffectMute()
    {
        return playerData.effectMute;
    }

    public void SetEffectMute(bool mute)
    {
        playerData.effectMute = mute;
        Save();
    }

    #endregion

    #region Progress

    public int GetCurrentScene()
    {
        return playerData.currentScene;
    }

    public void SetCurrentScene(int sceneIndex)
    {
        playerData.currentScene = sceneIndex;
        Save();
    }

    public int GetCurrentMainQuestIndex()
    {
        return playerData.currentMainQuestIndex;
    }

    public void SetCurrentMainQuestIndex(int questIndex)
    {
        UnityEngine.Debug.Log("mainQuestIndex edit");
        playerData.currentMainQuestIndex = questIndex;
    }

    public void SetIsQuestActionPlaying(bool _isQuestActionPlaying)
    {
        playerData.isQuestActionPlaying = _isQuestActionPlaying;
    }

    public bool GetIsQuestActinoPlaying()
    {
        return playerData.isQuestActionPlaying;
    }

    #endregion

    #region Unlock
    public void UnlockRepeatQuest()
    {
        playerData.unlockData.SetIsRepeatQuestUnlocked(true);
        Save();
    }

    public void UnlockCharonLetter()
    {
        playerData.unlockData.SetIsCharonLetterUnlocked(true);
        Save();
    }

    public void UnlockDiary()
    {
        playerData.unlockData.SetIsDiaryUnlocked(true);
        Save();
    }

    public void UnlockFoodLevel(int level)
    {
        playerData.unlockData.SetFoodUnlockLevel(level);
        Save();
    }

    public bool GetIsRepeatQuestUnlocked()
    {
        return playerData.unlockData.GetIsRepeatQuestUnlocked();
    }

    public bool GetIsCharonLetterUnlocked()
    {
        return playerData.unlockData.GetIsCharonLetterUnlocked();
    }

    public bool GetIsDiaryUnlocked()
    {
        return playerData.unlockData.GetIsDiaryUnlocked();
    }

    public int GetFoodUnlockLevel()
    {
        return playerData.unlockData.GetFoodUnlockLevel();
    }
    #endregion

    public void Load()
    {
        playerData = jsonManager.LoadData<PlayerData>(Constants.PlayerDataFile);

        if (playerData == null)
        {
            UnityEngine.Debug.Log("[PlayerDataManager] Save file not found. Create default data.");
            ResetData();
            Save();
        }
    }

    public void Save()
    {
        if (playerData == null)
        {
            UnityEngine.Debug.LogError("[PlayerDataManager] Save failed. playerData is null.");
            return;
        }

        UnityEngine.Debug.Log(new StackTrace(true));
        jsonManager.SaveData(Constants.PlayerDataFile, playerData);
    }
}

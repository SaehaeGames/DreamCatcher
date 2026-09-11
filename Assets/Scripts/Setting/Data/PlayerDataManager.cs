using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Diagnostics;


public class PlayerDataManager
{
    public event Action OnUnlockChanged;

    private JsonManager jsonManager = new JsonManager();
    private PlayerData playerData;


    public PlayerDataManager()
    {
        playerData = CreateDefaultData();
    }
     
    public void ResetData()
    {
        playerData = CreateDefaultData();
        Save();
        OnUnlockChanged?.Invoke();
    }

    public void ResetTutorialProgress()
    {
        if (playerData == null)
        {
            playerData = CreateDefaultData();
        }

        playerData.currentScene = 0;
        playerData.currentMainQuestIndex = 0;
        playerData.isQuestActionPlaying = false;
        playerData.unlockData = new UnlockData(false, false, false, 0);

        Save();
        OnUnlockChanged?.Invoke();
    }

    private PlayerData CreateDefaultData()
    {
        return new PlayerData()
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
        UnlockRepeatQuestWithSpecialFeed(0);
    }

    public bool UnlockRepeatQuestWithSpecialFeed(int specialFeedReward)
    {
        EnsureUnlockData();
        if (playerData.unlockData.GetIsRepeatQuestUnlocked()) return false;

        playerData.unlockData.SetIsRepeatQuestUnlocked(true);
        playerData.specialFeed += Mathf.Max(0, specialFeedReward);
        SaveUnlockData();
        return true;
    }

    public void UnlockCharonLetter()
    {
        EnsureUnlockData();
        if (playerData.unlockData.GetIsCharonLetterUnlocked()) return;

        playerData.unlockData.SetIsCharonLetterUnlocked(true);
        SaveUnlockData();
    }

    public void UnlockDiary()
    {
        playerData.unlockData.SetIsDiaryUnlocked(true);
    }

    public void UnlockFoodLevel(int level)
    {
        EnsureUnlockData();
        level = Mathf.Clamp(level, 0, 3);
        if (level <= playerData.unlockData.GetFoodUnlockLevel()) return;

        playerData.unlockData.SetFoodUnlockLevel(level);
        SaveUnlockData();
    }

    public bool GetIsRepeatQuestUnlocked()
    {
        EnsureUnlockData();
        return playerData.unlockData.GetIsRepeatQuestUnlocked();
    }

    public bool GetIsCharonLetterUnlocked()
    {
        EnsureUnlockData();
        return playerData.unlockData.GetIsCharonLetterUnlocked();
    }

    public bool GetIsDiaryUnlocked()
    {
        EnsureUnlockData();
        return playerData.unlockData.GetIsDiaryUnlocked();
    }

    public int GetFoodUnlockLevel()
    {
        EnsureUnlockData();
        return playerData.unlockData.GetFoodUnlockLevel();
    }

    private void EnsureUnlockData()
    {
        if (playerData.unlockData == null)
        {
            playerData.unlockData = new UnlockData(false, false, false, 0);
        }
    }

    private void SaveUnlockData()
    {
        Save();
        OnUnlockChanged?.Invoke();
    }
    #endregion

    public void Load()
    {
        playerData = jsonManager.LoadData<PlayerData>(Constants.PlayerDataFile);

        if (playerData == null)
        {
            UnityEngine.Debug.Log("[PlayerDataManager] Save file not found. Create default data.");
            ResetData();
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

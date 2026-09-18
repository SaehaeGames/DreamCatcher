using UnityEngine;
using System;


public class PlayerDataManager
{
    public event Action OnUnlockChanged;
    public event Action OnCurrencyChanged;

    private JsonManager jsonManager = new JsonManager();
    private PlayerData playerData;
    private int reservedSpecialFeed;


    public PlayerDataManager()
    {
        playerData = CreateDefaultData();
    }
     
    public void ResetData()
    {
        playerData = CreateDefaultData();
        reservedSpecialFeed = 0;
        Save();
        OnUnlockChanged?.Invoke();
        OnCurrencyChanged?.Invoke();
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
        playerData.specialFeed = 0;
        playerData.unlockData = new UnlockData(false, false, false, 0);
        reservedSpecialFeed = 0;

        Save();
        OnUnlockChanged?.Invoke();
        OnCurrencyChanged?.Invoke();
    }

    private PlayerData CreateDefaultData()
    {
        return new PlayerData()
        {
            gold = 0,
            dreamMarble = 0,
            specialFeed = 0,

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
        playerData.gold = Mathf.Max(0, playerData.gold + amount);
        Save();
        OnCurrencyChanged?.Invoke();
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
        OnCurrencyChanged?.Invoke();
        return true;
    }

    public int GetDreamMarble()
    {
        return playerData.dreamMarble;
    }

    public void AddDreamMarble(int amount)
    {
        playerData.dreamMarble = Mathf.Max(0, playerData.dreamMarble + amount);
        Save();
        OnCurrencyChanged?.Invoke();
    }

    public int GetSpecialFeed()
    {
        return Mathf.Max(0, playerData.specialFeed - reservedSpecialFeed);
    }

    public void AddSpecialFeed(int amount)
    {
        playerData.specialFeed = Mathf.Max(0, playerData.specialFeed + amount);
        Save();
        OnCurrencyChanged?.Invoke();
    }

    public bool UseSpecialFeed(int amount)
    {
        if (amount <= 0 || GetSpecialFeed() < amount)
        {
            UnityEngine.Debug.LogWarning("[PlayerDataManager] Not enough specialFeed.");
            return false;
        }

        playerData.specialFeed -= amount;
        Save();
        OnCurrencyChanged?.Invoke();
        return true;
    }

    public bool TryReserveSpecialFeed(int amount)
    {
        if (amount <= 0 || GetSpecialFeed() < amount)
        {
            UnityEngine.Debug.LogWarning("[PlayerDataManager] Not enough specialFeed to reserve.");
            return false;
        }

        reservedSpecialFeed += amount;
        OnCurrencyChanged?.Invoke();
        return true;
    }

    public void CommitReservedSpecialFeed(int amount, bool saveImmediately = true)
    {
        int commitAmount = Mathf.Clamp(amount, 0, reservedSpecialFeed);
        if (commitAmount == 0) return;

        reservedSpecialFeed -= commitAmount;
        playerData.specialFeed = Mathf.Max(0, playerData.specialFeed - commitAmount);
        if (saveImmediately) Save();
        OnCurrencyChanged?.Invoke();
    }

    public void CancelReservedSpecialFeed(int amount)
    {
        int cancelAmount = Mathf.Clamp(amount, 0, reservedSpecialFeed);
        if (cancelAmount == 0) return;

        reservedSpecialFeed -= cancelAmount;
        OnCurrencyChanged?.Invoke();
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

    public void SetAudioSettings(float bgmVolume, float effectVolume, bool bgmMute, bool effectMute)
    {
        playerData.bgmVolume = Mathf.Clamp01(bgmVolume);
        playerData.effectVolume = Mathf.Clamp01(effectVolume);
        playerData.bgmMute = bgmMute;
        playerData.effectMute = effectMute;
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
        SetCurrentScene(sceneIndex, false);
    }

    public void SetCurrentScene(int sceneIndex, bool notifyUnlockChanged)
    {
        playerData.currentScene = sceneIndex;
        Save();
        if (notifyUnlockChanged)
        {
            OnUnlockChanged?.Invoke();
        }
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
        return UnlockRepeatQuestWithSpecialFeed(specialFeedReward, true);
    }

    public bool UnlockRepeatQuestWithSpecialFeed(int specialFeedReward, bool saveImmediately)
    {
        EnsureUnlockData();
        if (playerData.unlockData.GetIsRepeatQuestUnlocked()) return false;

        playerData.unlockData.SetIsRepeatQuestUnlocked(true);
        playerData.specialFeed += Mathf.Max(0, specialFeedReward);
        OnCurrencyChanged?.Invoke();
        if (saveImmediately) SaveUnlockData();
        return true;
    }

    public void UnlockCharonLetter()
    {
        UnlockCharonLetter(true);
    }

    public void UnlockCharonLetter(bool saveImmediately)
    {
        EnsureUnlockData();
        if (playerData.unlockData.GetIsCharonLetterUnlocked()) return;

        playerData.unlockData.SetIsCharonLetterUnlocked(true);
        if (saveImmediately) SaveUnlockData();
    }

    public void UnlockDiary()
    {
        playerData.unlockData.SetIsDiaryUnlocked(true);
    }

    public void UnlockFoodLevel(int level)
    {
        UnlockFoodLevel(level, true);
    }

    public void UnlockFoodLevel(int level, bool saveImmediately)
    {
        EnsureUnlockData();
        level = Mathf.Clamp(level, 0, 3);
        if (level <= playerData.unlockData.GetFoodUnlockLevel()) return;

        playerData.unlockData.SetFoodUnlockLevel(level);
        if (saveImmediately) SaveUnlockData();
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
            return;
        }

        reservedSpecialFeed = 0;
        playerData.gold = Mathf.Max(0, playerData.gold);
        playerData.dreamMarble = Mathf.Max(0, playerData.dreamMarble);
        playerData.specialFeed = Mathf.Max(0, playerData.specialFeed);
        playerData.bgmVolume = Mathf.Clamp01(playerData.bgmVolume);
        playerData.effectVolume = Mathf.Clamp01(playerData.effectVolume);
        EnsureUnlockData();
    }

    public void Save()
    {
        if (playerData == null)
        {
            UnityEngine.Debug.LogError("[PlayerDataManager] Save failed. playerData is null.");
            return;
        }

        jsonManager.SaveData(Constants.PlayerDataFile, playerData);
    }
}

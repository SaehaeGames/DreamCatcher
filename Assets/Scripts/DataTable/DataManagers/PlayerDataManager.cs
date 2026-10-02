using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


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
        playerData.isMainQuestActionPlaying = false;
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
            isMainQuestActionPlaying = false,


            unlockData = new UnlockData(false, false, false, 0),

            currentCharonLetterIndex = -1,
            isCharonQuestActionPlaying = false,
            hasReadCharonLetter = true,
            questNotice = new bool[3] { false, false, false }
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
        if (amount < 0 || playerData.gold < amount)
        {
            UnityEngine.Debug.LogWarning("[PlayerDataManager] Not enough gold.");
            return false;
        }

        playerData.gold -= amount;
        if (!jsonManager.TrySaveData(Constants.PlayerDataFile, playerData))
        {
            playerData.gold += amount;
            return false;
        }
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

    /// <summary>
    /// 데이터 복사와 저장 없이 가격, 골드 잔액, 지급 수량의 정수 범위를 검사합니다.
    /// 지급은 실제 보유 수량을 기준으로 확인하며 튜토리얼에서 예약한 먹이 수량은 변경하지 않습니다.
    /// </summary>
    public bool CanPurchase(int cost, int specialFeedReward)
    {
        return playerData != null && cost >= 0 && specialFeedReward >= 0 && playerData.gold >= cost
            && playerData.specialFeed <= int.MaxValue - specialFeedReward;
    }

    /// <summary>구매 조건을 재검사한 뒤 골드 차감과 특제먹이 지급을 적용한 복사본을 만듭니다.</summary>
    public bool TryCreatePurchaseData(int cost, int specialFeedReward, out PlayerData purchaseData)
    {
        purchaseData = null;
        if (!CanPurchase(cost, specialFeedReward)) return false;

        // 해금 상태 등 기존 필드도 함께 복사하여 저장이 확정되기 전까지 원본을 유지합니다.
        purchaseData = JsonUtility.FromJson<PlayerData>(JsonUtility.ToJson(playerData));
        purchaseData.gold -= cost;
        purchaseData.specialFeed += specialFeedReward;
        return true;
    }

    public JsonManager.SaveEntry CreatePurchaseSaveEntry(PlayerData purchaseData)
    {
        return JsonManager.CreateSaveEntry(Constants.PlayerDataFile, purchaseData);
    }

    /// <summary>구매 기록 저장 후 메모리에 반영합니다. 예약 중인 튜토리얼 먹이 수량은 유지합니다.</summary>
    public void ApplyPurchaseData(PlayerData purchaseData)
    {
        playerData = purchaseData;
        OnCurrencyChanged?.Invoke();
    }

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
        playerData.currentMainQuestIndex = questIndex;
    }

    public void SetIsQuestActionPlaying(bool _isQuestActionPlaying)
    {
        playerData.isMainQuestActionPlaying = _isQuestActionPlaying;
    }

    public bool GetIsQuestActinoPlaying()
    {
        return playerData.isMainQuestActionPlaying;
    }

    public void SetCurrentCharonLetterIndex(int _currentCharonLetterIndex)
    {
        playerData.currentCharonLetterIndex = _currentCharonLetterIndex;
    }

    public int GetCurrentCharonLetterIndex()
    {
        return playerData.currentCharonLetterIndex;
    }

    public void SetIsCharonQuestActionPlaying(bool _isCharonQuestActionPlaying)
    {
        playerData.isCharonQuestActionPlaying = _isCharonQuestActionPlaying;
    }

    public bool GetIsCharonQuestActionPlaying()
    {
        return playerData.isCharonQuestActionPlaying;
    }

    public bool GetHasReadCharonLetter()
    {
        return playerData.hasReadCharonLetter;
    }

    public void SetHasReadCharonLetter(bool _hasReadCharonLetter)
    {
        playerData.hasReadCharonLetter = _hasReadCharonLetter;
    }

    public bool GetQuestNoticeRead(QuestType questType)
    {
        return playerData.GetQuestNoticeRead((int)questType);
    }

    public void SetQuestNoticeRead(QuestType questType, bool isRead)
    {
        playerData.SetQuestNoticeRead((int)questType, isRead);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnlockData
{
    public bool isRepeatQuestUnlocked;
    public bool isCharonLetterUnlocked;
    public bool isDiaryUnlocked;
    public int foodUnlockLevel;

    public UnlockData()
    {
        isRepeatQuestUnlocked = false;
        isCharonLetterUnlocked = false;
        isDiaryUnlocked = false;
        foodUnlockLevel = 0;
    }
    public UnlockData(bool _isRepeatQuestUnlocked, bool _isCharonLetterUnlocked, bool _isDiaryUnlocked, int _foodUnlockLevel)
    {
        isRepeatQuestUnlocked = _isRepeatQuestUnlocked;
        isCharonLetterUnlocked = _isCharonLetterUnlocked;
        isDiaryUnlocked = _isDiaryUnlocked;
        foodUnlockLevel = _foodUnlockLevel;
    }

    # region Set 함수
    public void SetIsRepeatQuestUnlocked(bool _isRepeatQuestUnlocked) { isRepeatQuestUnlocked = _isRepeatQuestUnlocked; }
    public void SetIsCharonLetterUnlocked(bool _isCharonLetterUnlocked) { isCharonLetterUnlocked = _isCharonLetterUnlocked; }
    public void SetIsDiaryUnlocked(bool _isDiaryUnlocked) {isDiaryUnlocked = _isDiaryUnlocked; }
    public void SetFoodUnlockLevel(int _foodUnlockLevel) {foodUnlockLevel = _foodUnlockLevel; }
    # endregion

    # region Get 함수
    public bool GetIsRepeatQuestUnlocked() { return isRepeatQuestUnlocked; }
    public bool GetIsCharonLetterUnlocked() { return isCharonLetterUnlocked; }
    public bool GetIsDiaryUnlocked() { return isDiaryUnlocked; }
    public int GetFoodUnlockLevel() { return foodUnlockLevel; }
    # endregion
}

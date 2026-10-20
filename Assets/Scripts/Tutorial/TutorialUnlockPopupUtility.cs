using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 먹이와 편지 해금 시퀀스가 같은 TutorialOverlay 팝업을 찾고 아이콘·문구를 교체하는 공통 기능.
/// </summary>
/// <summary>
/// 먹이와 편지 해금 시퀀스가 같은 TutorialOverlay 팝업을 찾고 아이콘·문구를 교체하는 공통 기능.
/// </summary>
internal static class TutorialUnlockPopupUtility
{
    /// <summary>
    /// Inspector 참조가 비어 있으면 TutorialOverlay의 고정 경로에서 팝업 구성 요소를 찾아 사용할 수 있는지 반환함.
    /// </summary>
    public static bool Resolve(
        Component owner,
        ref GameObject unlockPopUp,
        ref Button okBtn,
        ref Image itemImage,
        ref Text messageText)
    {
        // 1. 팝업 루트가 없으면 TutorialContext와 태그 검색 순서로 공용 UnlockPopUp을 찾음.
        if (unlockPopUp == null)
        {
            GameObject tutorialOverlay = ResolveTutorialOverlay(owner);
            Transform popupTransform = tutorialOverlay != null
                ? tutorialOverlay.transform.Find("UnlockPopUp")
                : null;
            unlockPopUp = popupTransform != null ? popupTransform.gameObject : null;
        }

        if (unlockPopUp == null)
        {
            Debug.LogError($"[TutorialUnlockPopup] {owner.name}: UnlockPopUp을 찾을 수 없습니다.");
            return false;
        }

        // 2. 개별 UI 참조가 없으면 팝업 내부의 정해진 경로에서 찾음.
        if (okBtn == null)
        {
            Transform buttonTransform = unlockPopUp.transform.Find("OKBtn");
            okBtn = buttonTransform != null ? buttonTransform.GetComponent<Button>() : null;
        }

        if (itemImage == null)
        {
            Transform imageTransform = unlockPopUp.transform.Find("ItemFrame/ItemImage");
            itemImage = imageTransform != null ? imageTransform.GetComponent<Image>() : null;
        }

        if (messageText == null)
        {
            Transform textTransform = unlockPopUp.transform.Find("Text");
            messageText = textTransform != null ? textTransform.GetComponent<Text>() : null;
        }

        // 3. 진행 입력에 필요한 OK 버튼을 찾음.
        if (okBtn == null)
        {
            Debug.LogError($"[TutorialUnlockPopup] {owner.name}: OKBtn을 찾을 수 없습니다.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 공통 팝업의 이전 내용을 지우고 현재 보상의 아이콘과 문구로 교체한 뒤 표시함.
    /// </summary>
    public static void Show(
        GameObject unlockPopUp,
        Image itemImage,
        Text messageText,
        Sprite icon,
        string message)
    {
        if (unlockPopUp == null)
        {
            return;
        }

        // 1. 아이콘이 없는 보상에서 이전 팝업 아이콘이 남지 않도록 Sprite와 활성 상태를 함께 갱신함.
        if (itemImage != null)
        {
            itemImage.sprite = icon;
            itemImage.enabled = icon != null;
        }

        // 2. 빈 문구도 그대로 반영하여 이전 보상 문구가 재사용되지 않게 함.
        if (messageText != null)
        {
            messageText.text = message ?? string.Empty;
        }

        // 3. 모든 내용을 교체한 뒤 팝업을 표시함.
        unlockPopUp.SetActive(true);
    }

    /// <summary>
    /// 공용 해금 팝업을 닫음.
    /// </summary>
    public static void Hide(GameObject unlockPopUp)
    {
        if (unlockPopUp == null)
        {
            return;
        }

        unlockPopUp.SetActive(false);
    }

    public static void BindOkButton(
    Button okBtn,
    UnityAction callback)
    {
        if (okBtn == null)
            return;

        okBtn.onClick.RemoveListener(callback);
        okBtn.onClick.AddListener(callback);
    }

    public static void UnbindOkButton(
    Button okBtn,
    UnityAction callback)
    {
        if (okBtn == null)
            return;

        okBtn.onClick.RemoveListener(callback);
    }

    /// <summary>
    /// TutorialContext를 우선 사용하고 이전 씬 구성은 안전한 태그 검색으로 지원함.
    /// </summary>
    private static GameObject ResolveTutorialOverlay(Component owner)
    {
        TutorialContext context = TutorialContext.Get(owner);
        if (context != null && context.TutorialOverlay != null)
        {
            return context.TutorialOverlay;
        }

        try
        {
            return GameObject.FindGameObjectWithTag(Constants.Tag_TutorialOverlay);
        }
        catch (UnityException)
        {
            return null;
        }
    }
}
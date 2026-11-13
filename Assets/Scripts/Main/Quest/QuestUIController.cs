using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestUIController : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    //퀘스트 배경 이미지를 바꾸는 스크립트
    [Header("[Letter UI]")]
    public GameObject letters;

    [Header("[Quest UI]")]
    public GameObject wallPaper;    //배경 이미지 오브젝트
    public GameObject contentTexts;       //텍스트 오브젝트
    public GameObject questPanel;   //퀘스트 패널
    public GameObject deliveryView;     //납품 오브젝트
    public GameObject deliveryButton;
    public GameObject charonItemButton;
    public GameObject stains;
    public GameObject repeatQuestDreamCaterList;
    private StainEraser[] stainErasers;
    public GameObject magnifier;

    // 지우개 커서
    [Header("[Eraser Cursor]")]
    public RectTransform eraserCursor;
    public Vector2 cursorOffset = new Vector2(35f, 45f);

    // 투명 입력 영역
    public GameObject eraserInputArea;

    [Space]
    [Header("[Quest Resource")]
    public Sprite[] wallPapers;  //배경 이미지들(0: 흰식, 1: 노란색, 2: 검정색)
    public Sprite potion;
    public Sprite paperweight;
    public List<Sprite> threads;
    public List<Sprite> feathers;
    private Color deactiveItemColor = new Color(0f, 0f, 0f, 157f / 255f);
    private Color activeItemColor = Color.white;
    private Color itemUsingColor = new Color(100f / 255f, 100f / 255f, 100f / 255f, 1f);

    [Space]
    [Header("[Managers]")]
    public GameObject managers;
    private PlayerDataManager playerDataManager;
    private RepeatQuestDataManager repeatQuestDataManager;

    private bool isItemActive = false;
    private CharonItem charonItem = CharonItem.None;

    private void Awake()
    {
        stainErasers = stains.GetComponentsInChildren<StainEraser>();

        eraserInputArea.SetActive(false);
        eraserCursor.gameObject.SetActive(false);
        repeatQuestDataManager = GameManager.instance.repeatQuestDataManager;
    }

    public void OpenWhiteWallPaper()
    {
        managers.GetComponent<QuestManager>().SetCurrentQuestType(QuestType.MainQuest);

        //퀘스트 창을 흰색 편지지, 검정 글씨로 바꾸고 퀘스트 패널을 여는 함수
        contentTexts.SetActive(true);   //메시지 오브젝트 활성화
        deliveryView.SetActive(false);  //납품 오브젝트 비활성화
        stains.SetActive(false);

        wallPaper.GetComponent<Image>().sprite = wallPapers[0]; //배경 이미지를 바꾼다

        int textCnt = contentTexts.transform.childCount;
        for (int i = 0; i < textCnt; i++)
        {
            contentTexts.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(0, 0, 0);    //글씨 색을 바꾼다
        }

        questPanel.SetActive(true); //퀘스트창 열기
        letters.GetComponent<QuestNotice>().CloseNotice(QuestType.MainQuest);    //카론퀘스트 알림 끄기
        UpdateQuestContents();

        QuestFlowState questFlowState = managers.GetComponent<QuestManager>().GetCurrentQuestFlowState();

        if (questFlowState == QuestFlowState.DeliveryCompleted
            || questFlowState == QuestFlowState.BeforeStart)
        {
            deliveryButton.SetActive(false);
        }
        else
        {
            deliveryButton.SetActive(true);
        }

        // 퀘스트별 특별 처리
        playerDataManager = GameManager.instance.playerDataManager;
        if (playerDataManager.GetCurrentMainQuestIndex() == 7)
        {
            stains.SetActive(true);
            charonItemButton.SetActive(true);
            charonItemButton.GetComponent<Image>().sprite = potion;
            if (playerDataManager.HasReceiveCharonItem(CharonItem.Potion))
            {
                charonItem = CharonItem.Potion;
                charonItemButton.GetComponent<Image>().color = activeItemColor;
                charonItemButton.GetComponent<Button>().enabled = true;
            }
            else
            {
                charonItemButton.GetComponent<Image>().color = deactiveItemColor;
                charonItemButton.GetComponent<Button>().enabled = false;
            }
        }
        else if(playerDataManager.GetCurrentMainQuestIndex() == 15)
        {
            charonItemButton.SetActive(true);
            charonItemButton.GetComponent<Image>().sprite = paperweight;
            if (playerDataManager.HasReceiveCharonItem(CharonItem.Paperweight))
            {
                charonItem = CharonItem.Paperweight;
                charonItemButton.GetComponent<Image>().color = activeItemColor;
                charonItemButton.GetComponent<Button>().enabled = true;
            }
            else
            {
                charonItemButton.GetComponent<Image>().color = deactiveItemColor;
                charonItemButton.GetComponent<Button>().enabled = false;
            }
        }
        else
        {
            charonItemButton.SetActive(false);
        }   
    }

    public void OpenYellowWallPaper()
    {
        managers.GetComponent<QuestManager>().SetCurrentQuestType(QuestType.RepeatQuest);
        //퀘스트 창을 노란색 편지지, 검정 글씨로 바꾸고 퀘스트 패널을 여는 함수
        repeatQuestDataManager = GameManager.instance.repeatQuestDataManager;
        if (repeatQuestDataManager.GetHasReadFirstLetter())
        {
            // 퀘스트 생성
            if (!repeatQuestDataManager.GetQuestInProgress())
            {
                // 새로운 반복 퀘스트 생성
                managers.GetComponent<QuestManager>()
                    .GenerateRepeatQuestDreamCatchers();

                // 반복 퀘스트 수락 처리
                managers.GetComponent<QuestManager>()
                    .AcceptRepeatQuest();
            }

            contentTexts.SetActive(false);   //메시지 오브젝트 활성화
            deliveryView.SetActive(true);  //납품 오브젝트 비활성화
            deliveryButton.SetActive(true);
        }
        else
        {
            contentTexts.SetActive(true);   //메시지 오브젝트 활성화
            deliveryView.SetActive(false);  //납품 오브젝트 비활성화
            deliveryButton.SetActive(false);
        }
        
        charonItemButton.SetActive(false);
        stains.SetActive(false);
        eraserInputArea.SetActive(false);
        HideEraserCursor();

        wallPaper.GetComponent<Image>().sprite = wallPapers[1]; //배경 이미지를 바꾼다

        int textCnt = contentTexts.transform.childCount;
        for (int i = 0; i < textCnt; i++)
        {
            contentTexts.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(0, 0, 0);    //글씨 색을 바꾼다
        }

        questPanel.SetActive(true); //퀘스트창 열기
        UpdateQuestContents();
        letters.GetComponent<QuestNotice>().CloseNotice(QuestType.RepeatQuest);    //퀘스트 알림 끄기
    }

    public void OpenBlackWallPaper()
    {
        //퀘스트 창을 검정색 편지지, 흰색 글씨로 바꾸고 퀘스트 패널을 여는 함수
        managers.GetComponent<QuestManager>().SetCurrentQuestType(QuestType.Charon);

        contentTexts.SetActive(true);   //메시지 오브젝트 활성화
        deliveryView.SetActive(false);  //납품 오브젝트 비활성화
        charonItemButton.SetActive(false);
        stains.SetActive(false);
        eraserInputArea.SetActive(false);
        HideEraserCursor();

        wallPaper.GetComponent<Image>().sprite = wallPapers[2]; //배경 이미지를 바꾼다

        int textCnt = contentTexts.transform.childCount;
        for (int i = 0; i < textCnt; i++)
        {
            contentTexts.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(255, 255, 255);    //글씨 색을 바꾼다
        }

        questPanel.SetActive(true); //퀘스트창 열기

        UpdateQuestContents();

        deliveryButton.SetActive(false); //납품 버튼 비활성화
        letters.GetComponent<QuestNotice>().CloseNotice(QuestType.Charon);    //카론퀘스트 알림 끄기
    }

    public void UpdateQuestContents()
    {
        QuestType questType = managers.GetComponent<QuestManager>().GetCurrentQuestType();
        if (questType == QuestType.MainQuest)
        {
            // 제목
            contentTexts.transform.GetChild(0).gameObject.GetComponent<Text>().text
                = managers.GetComponent<QuestManager>().GetCurrentMainQuestTitle();

            // 내용
            contentTexts.transform.GetChild(1).gameObject.GetComponent<Text>().text
                = managers.GetComponent<QuestManager>().GetCurrentMainQuestContents();

            // 보낸 사람
            contentTexts.transform.GetChild(2).gameObject.GetComponent<Text>().text
                = managers.GetComponent<QuestManager>().GetCurrentMainQuestFrom();
        }
        else if (questType == QuestType.RepeatQuest)
        {
            if(repeatQuestDataManager.GetHasReadFirstLetter())
            {
                for (int i = 0; i < 3; i++)
                {
                    RepeatQuestDreamCatcher dreamCatcher =
                        managers.GetComponent<QuestManager>().GetRepeatQuestDreamCatcher(i);

                    if (dreamCatcher == null)
                    {
                        Debug.LogError($"[RepeatQuest] {i}번 드림캐쳐 데이터가 null입니다.");
                        continue;
                    }

                    Image thread = repeatQuestDreamCaterList.transform.GetChild(i).Find("Image_thread").gameObject.GetComponent<Image>();
                    Image feather1 = repeatQuestDreamCaterList.transform.GetChild(i).Find("Image_feather1").gameObject.GetComponent<Image>();
                    Image feather2 = repeatQuestDreamCaterList.transform.GetChild(i).Find("Image_feather2").gameObject.GetComponent<Image>();
                    Image feather3 = repeatQuestDreamCaterList.transform.GetChild(i).Find("Image_feather3").gameObject.GetComponent<Image>();

                    thread.sprite = threads[dreamCatcher.DCcolor];
                    feather1.sprite = feathers[dreamCatcher.DCfeather1];
                    feather2.sprite = feathers[dreamCatcher.DCfeather2];
                    feather3.sprite = feathers[dreamCatcher.DCfeather3];
                }
            }
            else
            {
                // 제목
                contentTexts.transform.GetChild(0).gameObject.GetComponent<Text>().text
                    = managers.GetComponent<QuestManager>().GetRepeatQuestLetterTitle();

                // 내용
                contentTexts.transform.GetChild(1).gameObject.GetComponent<Text>().text
                    = managers.GetComponent<QuestManager>().GetRepeatQuestLetterContents();

                // 보낸 사람
                contentTexts.transform.GetChild(2).gameObject.GetComponent<Text>().text
                    = managers.GetComponent<QuestManager>().GetRepeatQuestLetterFrom();
            }
        }
        else if (questType == QuestType.Charon)
        {
            // 제목
            contentTexts.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";

            // 내용
            contentTexts.transform.GetChild(1).gameObject.GetComponent<Text>().text
                = managers.GetComponent<QuestManager>().GetCurrentCharonLetterContents();

            // 보낸 사람
            contentTexts.transform.GetChild(2).gameObject.GetComponent<Text>().text = "";
        }
        
    }

    public void UseItem()
    {
        isItemActive = !isItemActive;

        if (charonItem == CharonItem.Potion)
        {

            foreach (StainEraser stain in stainErasers)
            {
                stain.SetCanErase(isItemActive);
            }

            // 지우개가 활성화된 동안만 입력 영역 활성화
            eraserInputArea.SetActive(isItemActive);

            if (!isItemActive)
                HideEraserCursor();
        }
        else if (charonItem == CharonItem.Paperweight)
        {
            magnifier.SetActive(isItemActive);
            magnifier.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text
                = managers.GetComponent<QuestManager>().GetCurrentMainQuestMagnifierContents();
        }
        
        // 버튼 UI 연출
        Image buttonImage = charonItemButton.GetComponent<Image>();

        buttonImage.color = isItemActive
            ? itemUsingColor
            : activeItemColor;
    }

    // [추가] 터치 시작
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isItemActive || charonItem != CharonItem.Potion)
            return;

        ShowEraserCursor(eventData.position, eventData.pressEventCamera);

        // 모든 얼룩에 현재 터치 위치 전달
        EraseStains(eventData.position, eventData.pressEventCamera);
    }

    // [추가] 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        if (!isItemActive || charonItem != CharonItem.Potion)
            return;

        UpdateEraserCursor(eventData.position, eventData.pressEventCamera);

        // 모든 얼룩에 현재 터치 위치 전달
        EraseStains(eventData.position, eventData.pressEventCamera);
    }

    // [추가] 터치 종료
    public void OnPointerUp(PointerEventData eventData)
    {
        HideEraserCursor();
    }

    // [추가] 모든 얼룩에 터치 좌표 전달
    private void EraseStains(Vector2 screenPosition, Camera eventCamera)
    {
        foreach (StainEraser stain in stainErasers)
        {
            if (stain == null || !stain.gameObject.activeInHierarchy)
                continue;

            stain.EraseAtScreenPosition(screenPosition, eventCamera);
        }
    }

    // 커서 표시 및 최초 위치 설정
    public void ShowEraserCursor(Vector2 screenPosition, Camera eventCamera)
    {
        if (eraserCursor == null)
            return;

        eraserCursor.gameObject.SetActive(true);

        UpdateEraserCursor(screenPosition, eventCamera);
    }

    // 커서 위치 갱신
    public void UpdateEraserCursor(Vector2 screenPosition, Camera eventCamera)
    {
        if (eraserCursor == null)
            return;

        RectTransform parentRect = eraserCursor.parent as RectTransform;

        if (parentRect == null)
            return;

        Vector2 localPosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPosition + cursorOffset,
            eventCamera,
            out localPosition))
        {
            eraserCursor.anchoredPosition = localPosition;
        }
    }

    // 커서 숨기기
    public void HideEraserCursor()
    {
        if (eraserCursor != null)
            eraserCursor.gameObject.SetActive(false);
    }

    private void ResetQuestUIState()
    {
        // 아이템 사용 상태 초기화
        isItemActive = false;
        charonItem = CharonItem.None;

        // 지우개 입력 비활성화
        eraserInputArea.SetActive(false);

        // 커서 숨기기
        HideEraserCursor();

        // 얼룩 지우기 기능 비활성화
        foreach (StainEraser stain in stainErasers)
        {
            if (stain != null)
                stain.SetCanErase(false);
        }

        // 돋보기 비활성화
        magnifier.SetActive(false);
    }

    public void CloseQuestPaper()
    {
        ResetQuestUIState();

        QuestType questType = managers.GetComponent<QuestManager>().GetCurrentQuestType();

        managers.GetComponent<QuestManager>().PlayCurrentQuestAction(questType);
    }
}

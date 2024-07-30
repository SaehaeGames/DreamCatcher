using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptBox : MonoBehaviour
{
    // 스크립트 박스 구성 게임오브젝트
    [Header("스크립트 박스 구성 게임오브젝트")]
    public GameObject scriptLogBox;
    private Text scriptLogBoxTxt; // 로그 박스 Txt
    private Text loadingTxt; // 대사 박스 Txt
    public Image characterBody; // 캐릭터 몸통 Img
    public Image characterFace; // 캐릭터 표정 Img
    public Image characterEffect; // 캐릭터 효과 Img
    public Text characterNameText; // 캐릭터 이름 Txt
    public Button nextScriptBtn; // 다음 스크립트 Btn
    public Button logBtn; // 로그 Btn

    // 이미지 리소스
    [Header("이미지 리소스")]
    public Sprite[] characterBodySprites; // 캐릭터 몸통 이미지
    public Sprite[] characterFaceSprites; // 캐릭터 표정 이미지
    public Sprite[] characterEffectSprites; // 캐릭터 효과 이미지

    // 변수
    private string scriptLog;
    private int lineNum;
    private float speed;
    private bool typing;
    private bool scriptLogOpen;
    private bool highlightChar;
    public int startId;
    public int endId;
    public string talk;
    private IEnumerator typingCoroutine;
    private bool myreturn = false;

    // 데이터 테이블
    private PlayerDataContainer curPlayerData;   //플레이어 데이터 정보
    public StoryScriptInfo_Data _storyscriptinfo_data;
    public StorySceneInfo_Data _storysceneinfo_data;

    private void Awake()
    {
        curPlayerData = GameManager.instance.loadPlayerData;    //플레이어 데이터 json
        GameManager.instance.GetComponent<PlayerDataJSON>().LoadTopBarData();
        loadingTxt = gameObject.transform.GetChild(1).GetChild(2).gameObject.GetComponent<Text>();
        scriptLogBoxTxt = scriptLogBox.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
    }

    void Start()
    {
        // 초기화
        scriptLogOpen = false; // 스크립트로그 닫기 상태
        scriptLogBox.gameObject.SetActive(false); // 스크립트로그 박스 닫기
        speed = 0.1f; // 스크립트 재생 속도
        highlightChar = false;
    }

    // 대사창 시작
    public void SetScriptBox(int startId, int endId)
    {
        // 시작 아이디 설정
        myreturn = false; // myreturn 초기화
        this.startId = startId;
        lineNum = startId % 7000; // 인덱스화
        lineNum--;
        this.endId = endId;

        SetNextDialog();  
    }

    
    // 다음 스크립트 재생
    public void NextScript()
    {
        // 텍스트 타이핑 효과가 재생중일 때
        if (typing)
        {
            StopCoroutine(typingCoroutine); // Typing 코루틴 중단

            // 줄바꿈 및 특수효과 Replace
            talk = talk.Replace("  ", "\n");
            talk = talk.Replace("<", "-<");
            talk = talk.Replace(">", ">-");
            talk = talk.Replace("-<", "<color=#ffb7a6><b>");
            talk = talk.Replace(">-", "</b></color>");

            // 텍스트 표시
            loadingTxt.text = talk;
            typing = false;
            highlightChar = false; // 하이라이트 표시 종료
            myreturn = false;
            return;
        }

        // 대화가 끝나지 않았을 때
        if (endId > 7000 + lineNum)
        {
            SetNextDialog();
        }
        else // endId까지 대화가 끝났을 때
        {
            myreturn = true;
            return;
        }

        myreturn = false;
        return;
    }

    private void SetNextDialog()
    {
        lineNum++;

        // UI 설정
        characterNameText.text = _storyscriptinfo_data.dataList[lineNum].speaker;
        characterBody.sprite = characterBodySprites[_storyscriptinfo_data.dataList[lineNum].charImage];
        characterFace.sprite = characterFaceSprites[_storyscriptinfo_data.dataList[lineNum].faceImage];
        characterEffect.sprite = characterEffectSprites[_storyscriptinfo_data.dataList[lineNum].effectImage];

        // 캐릭터 음영 처리
        if (_storyscriptinfo_data.dataList[lineNum].charImage > 0)
        {
            characterBody.color = new Color(1f, 1f, 1f, 1f);
            characterFace.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            characterBody.color = new Color(0f, 0f, 0f, 1f);
            characterFace.color = new Color(0f, 0f, 0f, 1f);
        }


        talk = _storyscriptinfo_data.dataList[lineNum].line;
        typingCoroutine = Typing();
        StartCoroutine(typingCoroutine);
    }

    public bool ReturnNextScript()
    {
        return myreturn;
    }

    // 타이핑 효과
    IEnumerator Typing()
    {
        speed = 0.1f; // 스피드 설정(기본)
        typing = true; // 타이핑 중
        loadingTxt.text = null; // 텍스트 박스 초기화

        // 줄바꿈 처리
        if (talk.Contains("  "))
        {
            talk = talk.Replace("  ", "\n");
        }

        // 타이핑 효과
        for (int i = 0; i < talk.Length; i++)
        {
            // 하이라이트 효과 확인
            if (talk[i] == '<')
            {
                highlightChar = true; // 하이라이트 표시 시작
                continue; // 대사창에는 표시X
            }
            else if (talk[i] == '>')
            {
                highlightChar = false; // 하이라이트 표시 종료
                continue; // 대사창에는 표시X
            }

            // 하이라이트 효과 적용/미적용
            if (highlightChar) // 적용
            {
                loadingTxt.text += ("<color=#ffb7a6><b>" + talk[i] + "</b></color>");
            }
            else // 미적용
            {
                loadingTxt.text += talk[i];
            }

            // 속도
            yield return new WaitForSeconds(speed);
        }

        typing = false; // 타이핑 종료
        scriptLog += talk + '\n'; // 로그 업데이트
    }


    // 로그 박스 on/off 관리
    public void LogBoxOpen()
    {
        if (!scriptLogOpen) // 스크립트박스 on
        {
            scriptLogOpen = true;
            scriptLogBox.gameObject.SetActive(true);
            scriptLogBoxTxt.text = scriptLog;
        }
        else // 스크립트박스 off
        {
            scriptLogOpen = false;
            scriptLogBox.gameObject.SetActive(false);
        }
    }

    // 로그 초기화
    public void ResetLog()
    {
        scriptLog = "";
    }

    // 대화창 on/off
    public void ScriptBoxOnOff(bool onoff)
    {
        this.gameObject.transform.GetChild(1).gameObject.SetActive(onoff);
        this.gameObject.transform.GetChild(0).gameObject.SetActive(onoff);
    }
}

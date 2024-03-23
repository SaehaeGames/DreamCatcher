using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptBox : MonoBehaviour
{
    // 스크립트 박스 구성 게임오브젝트
    public GameObject scriptLogBox;
    private Text scriptLogBoxTxt; // 로그 박스 Txt
    private Text loadingTxt; // 대사 박스 Txt
    public Image characterBody; // 캐릭터 몸통 Img
    public Image characterFace; // 캐릭터 표정 Img
    public Image characterEffect; // 캐릭터 효과 Img
    public Text characterNameText; // 캐릭터 이름 Txt
    public Button nextScriptBtn;

    private string scriptLog;
    private int lineNum;

    // 이미지 리소스
    public Sprite[] characterBodySprites; // 캐릭터 몸통 이미지
    public Sprite[] characterFaceSprites; // 캐릭터 표정 이미지
    public Sprite[] characterEffectSprites; // 캐릭터 효과 이미지

    // 변수
    private float speed;
    private bool typing;
    private bool scriptLogOpen;
    private bool highlightChar;
    public int startId;
    public int endId;
    public bool next;

    // 데이터 테이블
    private PlayerDataContainer curPlayerData;   //플레이어 데이터 정보
    public StoryScriptInfo_Data _storyscriptinfo_data;
    public StorySceneInfo_Data _storysceneinfo_data;

    private void Awake()
    {
        curPlayerData = GameManager.instance.loadPlayerData;    //플레이어 데이터 json
        GameManager.instance.GetComponent<PlayerDataJSON>().LoadTopBarData();
        loadingTxt = gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
        scriptLogBoxTxt = scriptLogBox.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
    }

    void Start()
    {
        // 초기화
        scriptLogOpen = false; // 스크립트로그 닫기 상태
        scriptLogBox.gameObject.SetActive(false); // 스크립트로그 박스 닫기
        speed = 0.1f; // 스크립트 재생 속도
        highlightChar = false;

        //startId = _storysceneinfo_data.datalist[(int)curPlayerData.dataList[7].dataNumber].startId; // 시작 아이디 불러오기
        //StartScript(startId, endId);
    }

    // 대사창 시작
    public void StartScript(int startId, int endId)
    {
        // 시작 아이디 설정
        lineNum = startId % 7000; // 인덱스화
        this.endId = endId;

        characterNameText.text = _storyscriptinfo_data.datalist[lineNum].speaker;

        characterBody.sprite = characterBodySprites[_storyscriptinfo_data.datalist[lineNum].charImage];
        characterFace.sprite = characterFaceSprites[_storyscriptinfo_data.datalist[lineNum].faceImage];
        characterEffect.sprite = characterEffectSprites[_storyscriptinfo_data.datalist[lineNum].effectImage];
        if (_storyscriptinfo_data.datalist[lineNum].charImage > 0)
        {
            characterBody.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            characterBody.color = new Color(0f, 0f, 0f, 1f);
        }

        /*characterNameText.text = _storyscriptinfo_data.datalist[lineNum].speaker;

        characterBody.sprite = characterBodySprites[_storyscriptinfo_data.datalist[lineNum].charImage];
        characterFace.sprite = characterFaceSprites[_storyscriptinfo_data.datalist[lineNum].faceImage];
        characterEffect.sprite = characterEffectSprites[_storyscriptinfo_data.datalist[lineNum].effectImage];
        if (_storyscriptinfo_data.datalist[lineNum].charImage > 0)
        {
            characterBody.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            characterBody.color = new Color(0f, 0f, 0f, 1f);
        }
        // 타이핑 효과
        StartCoroutine(Typing(_storyscriptinfo_data.datalist[lineNum].line));*/
    }

    //다음 대사 버튼

    public void NextScriptBtnHandler()
    {
        next = true;
        NextScript();
    }

    public bool NextScript()
    {
        if(next)
        {
            // 한 대화 묶음 끝 확인
            if (7000 + lineNum >= (endId + 1))
            {
                Debug.Log("챕터 끝");
                return true;
            }
            next = false;
            // 타이핑 중
            if (typing)
            {
                speed = 0.005f; // 속도 빠르게
                return false;
            }
            else // 타이핑 끝
            {
                // 대사라면
                if(_storyscriptinfo_data.datalist[lineNum].charImage!=99)
                {
                    characterNameText.text = _storyscriptinfo_data.datalist[lineNum].speaker;

                    characterBody.sprite = characterBodySprites[_storyscriptinfo_data.datalist[lineNum].charImage];
                    characterFace.sprite = characterFaceSprites[_storyscriptinfo_data.datalist[lineNum].faceImage];
                    characterEffect.sprite = characterEffectSprites[_storyscriptinfo_data.datalist[lineNum].effectImage];
                    if (_storyscriptinfo_data.datalist[lineNum].charImage > 0)
                    {
                        characterBody.color = new Color(1f, 1f, 1f, 1f);
                    }
                    else
                    {
                        characterBody.color = new Color(0f, 0f, 0f, 1f);
                    }
                    StartCoroutine(Typing(_storyscriptinfo_data.datalist[lineNum].line));
                    lineNum++;
                } // 대사가 아니라면(클릭 명령)
                else
                {
                    lineNum++;
                }

                // 새로운 씬으로 넘어가면 => 저장
                if(_storyscriptinfo_data.datalist[lineNum].sceneNum != (int)curPlayerData.dataList[7].dataNumber)
                {
                    // 현재 씬 업데이트
                    curPlayerData.dataList[7].dataNumber = _storyscriptinfo_data.datalist[lineNum].sceneNum;
                    GameManager.instance.GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);
                }
                return false;
            }
        }
        return false;
    }

    // 타이핑 효과
    IEnumerator Typing(string talk)
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
            if (talk[i]=='<')
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
            if(highlightChar) // 적용
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

        //lineNum++; // 다음 대사 번호
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
}

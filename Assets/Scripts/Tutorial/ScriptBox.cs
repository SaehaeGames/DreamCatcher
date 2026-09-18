using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 튜토리얼 대사 데이터의 지정 구간을 한 줄씩 재생하고 타이핑·강조·대사 로그 UI를 관리함.
/// InteractiveSequenceDialog는 이 클래스의 완료 상태를 확인하여 다음 시퀀스로 진행함.
/// </summary>
public class ScriptBox : MonoBehaviour
{
    private const int StoryScriptIdOffset = 7000;
    private const float DefaultTypingSpeed = 0.1f;
    private const string HighlightOpenTag = "<color=#ffb7a6><b>";
    private const string HighlightCloseTag = "</b></color>";

    [Header("스크립트 박스 UI")]
    [SerializeField] private GameObject scriptLogBox;
    [SerializeField] private Image characterBody;
    [SerializeField] private Image characterFace;
    [SerializeField] private Image characterEffect;
    [SerializeField] private Text characterNameText;

    [Header("이미지 리소스")]
    [SerializeField] private Sprite[] characterBodySprites;
    [SerializeField] private Sprite[] characterFaceSprites;
    [SerializeField] private Sprite[] characterEffectSprites;

    [Header("대사 데이터")]
    [SerializeField] private StoryScriptInfo_Data _storyscriptinfo_data;

    private Text scriptLogBoxTxt;
    private Text loadingTxt;
    private GameObject dialogueBackground;
    private GameObject dialogueContent;
    private ScriptManager scriptManager;
    private Coroutine typingCoroutine;
    private WaitForSeconds typingDelay;

    private DialogRuntimeState runtime;
    private readonly StringBuilder typingBuilder = new StringBuilder(256);
    private readonly StringBuilder formattedLineBuilder = new StringBuilder(256);
    private readonly StringBuilder logBuilder = new StringBuilder(512);

    /// <summary>
    /// 대사 재생에 필요한 UI 참조와 전역 로그 관리자를 실제 사용 전에 가져옴.
    /// </summary>
    private void Awake()
    {
        // 1. 기존 프리팹 계층에서 대사 본문·배경·로그 Text를 찾아 캐시함.
        ResolveUiReferences();

        // 2. 씬 이동 후 유지되는 ScriptManager 인스턴스를 가져옴.
        scriptManager = ScriptManager.instance;
        typingDelay = new WaitForSeconds(DefaultTypingSpeed);

        // 3. 필수 UI나 데이터 연결이 빠졌다면 재생 전에 구성 오류를 알림.
        ValidateRequiredReferences();
    }

    /// <summary>
    /// 로그창·타이핑 속도·강조 상태를 초기 화면 상태로 맞춤.
    /// </summary>
    private void Start()
    {
        // 1. 실행 상태를 초기화함.
        runtime.IsLogOpen = false;
        runtime.IsHighlighting = false;

        // 2. 첫 대사 전에는 로그 UI를 닫아둠.
        if (scriptLogBox != null)
        {
            scriptLogBox.SetActive(false);
        }
    }

    /// <summary>
    /// 기존 대사를 중단하고 지정한 데이터 ID 범위의 첫 대사 재생을 시작함.
    /// </summary>
    public void SetScriptBox(int startId, int endId)
    {
        // 1. 이전 단계의 타이핑과 완료 상태를 정리함.
        StopCurrentDialog();
        runtime.IsCompleted = false;
        runtime.EndId = endId;

        // 2. 7000번대 대사 ID를 StoryScriptInfo 데이터 리스트 인덱스로 변환함.
        runtime.LineIndex = startId % StoryScriptIdOffset;
        runtime.LineIndex--;

        // 3. 계산된 첫 행을 UI에 적용하고 타이핑을 시작함.
        SetNextDialog();
    }

    /// <summary>
    /// 타이핑 중이면 현재 문장을 즉시 완성하고, 아니면 다음 줄 또는 전체 대화 종료를 처리함.
    /// </summary>
    public void NextScript()
    {
        // 1. 글자가 출력 중인 첫 클릭은 문장 전체를 즉시 보여 주는 데 사용함.
        if (runtime.IsTyping)
        {
            CompleteCurrentLineImmediately();
            return;
        }

        // 2. 마지막 ID 전이라면 다음 대사 행을 재생함.
        if (runtime.EndId > StoryScriptIdOffset + runtime.LineIndex)
        {
            SetNextDialog();
            return;
        }

        // 3. 마지막 대사 확인 후 완료 상태를 기록하고 대사 UI를 닫음.
        runtime.IsCompleted = true;
        ScriptBoxOnOff(false);
    }

    /// <summary>
    /// 다음 데이터 행의 화자·캐릭터 이미지·대사를 UI에 적용하고 타이핑 코루틴을 시작함.
    /// </summary>
    private void SetNextDialog()
    {
        // 1. 다음 데이터 인덱스를 계산하고 유효한 StoryScriptInfo 행인지 검사함.
        runtime.LineIndex++;
        if (!TryGetScriptRow(runtime.LineIndex, out StoryScriptInfo_Object scriptRow))
        {
            runtime.IsCompleted = true;
            return;
        }

        // 2. 화자와 캐릭터의 몸·표정·효과 이미지를 현재 행 값으로 교체함.
        ApplyCharacterVisuals(scriptRow);

        // 3. 현재 대사를 로그에 먼저 기록하여 타이핑 도중에도 로그창에서 확인할 수 있게 함.
        runtime.Talk = scriptRow.line ?? string.Empty;
        ResolveScriptManager();
        if (scriptManager != null)
        {
            scriptManager.AddLog(runtime.Talk);
        }

        // 4. 새 타이핑 코루틴을 시작하고 참조를 저장해 중단 시 정확히 정리함.
        typingCoroutine = StartCoroutine(TypingRoutine());
    }

    /// <summary>
    /// InteractiveSequenceDialog가 지정 범위의 전체 대화 완료 여부를 확인할 수 있게 반환함.
    /// </summary>
    public bool ReturnNextScript()
    {
        return runtime.IsCompleted;
    }

    /// <summary>
    /// 대사를 글자 단위로 출력하며 꺾쇠 사이의 글자에 색상과 굵기 태그를 적용함.
    /// </summary>
    private IEnumerator TypingRoutine()
    {
        // 1. 출력 상태와 텍스트 버퍼를 새 문장 기준으로 초기화함.
        runtime.IsTyping = true;
        runtime.IsHighlighting = false;
        typingBuilder.Clear();
        if (loadingTxt != null)
        {
            loadingTxt.text = string.Empty;
        }

        if (typingDelay == null)
        {
            typingDelay = new WaitForSeconds(DefaultTypingSpeed);
        }

        // 2. 데이터에서 연속된 공백 두 개를 줄바꿈 문자로 변환함.
        runtime.Talk = ReplaceDoubleSpacesWithLineBreak(runtime.Talk);

        // 3. 제어 문자 <, >는 화면에서 제외하고 그 사이 글자에 Rich Text 태그를 붙임.
        for (int i = 0; i < runtime.Talk.Length; i++)
        {
            char currentCharacter = runtime.Talk[i];
            if (currentCharacter == '<')
            {
                runtime.IsHighlighting = true;
                continue;
            }

            if (currentCharacter == '>')
            {
                runtime.IsHighlighting = false;
                continue;
            }

            AppendCharacter(typingBuilder, currentCharacter, runtime.IsHighlighting);
            if (loadingTxt != null)
            {
                loadingTxt.text = typingBuilder.ToString();
            }

            yield return typingDelay;
        }

        // 4. 마지막 글자 출력 뒤 코루틴과 타이핑 상태를 완료 상태로 정리함.
        runtime.IsTyping = false;
        runtime.IsHighlighting = false;
        typingCoroutine = null;
    }

    /// <summary>
    /// 최근 대사 목록을 Rich Text로 조합하여 로그창을 열거나 이미 열려 있으면 닫음.
    /// </summary>
    public void LogBoxOpen()
    {
        // 1. 열려 있는 로그창은 추가 작업 없이 닫음.
        if (runtime.IsLogOpen)
        {
            runtime.IsLogOpen = false;
            if (scriptLogBox != null)
            {
                scriptLogBox.SetActive(false);
            }
            return;
        }

        // 2. 전역 로그 관리자를 확인하고 최근 대사의 복사본을 가져옴.
        ResolveScriptManager();
        if (scriptManager == null || scriptLogBox == null || scriptLogBoxTxt == null)
        {
            Debug.LogWarning("[ScriptBox] 로그 UI 또는 ScriptManager가 준비되지 않았습니다.");
            return;
        }

        List<string> logs = scriptManager.GetLogs();

        // 3. 각 대사에 줄바꿈·강조 효과를 적용하여 하나의 로그 문자열로 조합함.
        logBuilder.Clear();
        for (int i = 0; i < logs.Count; i++)
        {
            AppendFormattedLine(logBuilder, logs[i]);
            logBuilder.Append('\n');
        }

        // 4. 조합한 로그를 적용한 뒤 로그창을 표시함.
        scriptLogBoxTxt.text = logBuilder.ToString();
        runtime.IsLogOpen = true;
        scriptLogBox.SetActive(true);
    }

    /// <summary>
    /// 대사 UI 구성 오브젝트를 함께 켜거나 끄고, 끌 때 실행 중인 타이핑도 중단함.
    /// </summary>
    public void ScriptBoxOnOff(bool isActive)
    {
        // 1. 대사창을 닫을 때 진행 중인 코루틴과 강조 상태를 먼저 정리함.
        if (!isActive)
        {
            StopCurrentDialog();
        }

        // 2. 기존 프리팹의 배경과 본문 오브젝트를 같은 상태로 전환함.
        if (dialogueContent != null)
        {
            dialogueContent.SetActive(isActive);
        }

        if (dialogueBackground != null)
        {
            dialogueBackground.SetActive(isActive);
        }
    }

    /// <summary>
    /// 현재 타이핑 코루틴과 관련 상태를 정리하여 닫힌 대사가 뒤늦게 UI를 갱신하지 않게 함.
    /// </summary>
    public void StopCurrentDialog()
    {
        // 1. 실행 중인 코루틴이 있으면 해당 인스턴스만 중단함.
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // 2. 다음 대사 시작에 영향을 주지 않도록 타이핑·강조 상태를 초기화함.
        runtime.IsTyping = false;
        runtime.IsHighlighting = false;
    }

    /// <summary>
    /// 씬 전환이나 상위 오브젝트 비활성화 시 남은 타이핑 코루틴을 정리함.
    /// </summary>
    private void OnDisable()
    {
        StopCurrentDialog();
    }

    /// <summary>
    /// 기존 프리팹 계층에서 자주 사용하는 UI 오브젝트를 한 번만 찾아 저장함.
    /// </summary>
    private void ResolveUiReferences()
    {
        // 1. 대사 배경과 본문은 기존 자식 순서를 유지하되 이후에는 캐시된 참조를 사용함.
        if (transform.childCount > 0)
        {
            dialogueBackground = transform.GetChild(0).gameObject;
        }

        if (transform.childCount > 1)
        {
            dialogueContent = transform.GetChild(1).gameObject;
            if (dialogueContent.transform.childCount > 2)
            {
                loadingTxt = dialogueContent.transform.GetChild(2).GetComponent<Text>();
            }
        }

        // 2. 로그 Text는 기존 프리팹의 계층을 순회하여 찾음.
        Transform logTextTransform = scriptLogBox != null ? scriptLogBox.transform : null;
        for (int depth = 0; depth < 4 && logTextTransform != null; depth++)
        {
            logTextTransform = logTextTransform.childCount > 0
                ? logTextTransform.GetChild(0)
                : null;
        }

        if (logTextTransform != null)
        {
            scriptLogBoxTxt = logTextTransform.GetComponent<Text>();
        }
    }

    /// <summary>
    /// 대사 재생에 필수인 UI와 데이터 연결이 누락되었는지 확인함.
    /// </summary>
    private void ValidateRequiredReferences()
    {
        if (_storyscriptinfo_data == null)
        {
            Debug.LogError($"[ScriptBox] {gameObject.name}: StoryScriptInfo 데이터가 설정되지 않았습니다.");
        }

        if (loadingTxt == null || characterNameText == null)
        {
            Debug.LogError($"[ScriptBox] {gameObject.name}: 대사 Text 참조가 올바르지 않습니다.");
        }

        if (characterBody == null || characterFace == null || characterEffect == null)
        {
            Debug.LogError($"[ScriptBox] {gameObject.name}: 캐릭터 Image 참조가 올바르지 않습니다.");
        }
    }

    /// <summary>
    /// 지정 인덱스의 대사 행이 존재하는지 확인하여 안전하게 반환함.
    /// </summary>
    private bool TryGetScriptRow(int index, out StoryScriptInfo_Object scriptRow)
    {
        scriptRow = null;
        if (_storyscriptinfo_data == null
            || _storyscriptinfo_data.dataList == null
            || index < 0
            || index >= _storyscriptinfo_data.dataList.Count)
        {
            Debug.LogError($"[ScriptBox] 대사 인덱스 {index}가 StoryScriptInfo 범위를 벗어났습니다.");
            return false;
        }

        scriptRow = _storyscriptinfo_data.dataList[index];
        return scriptRow != null;
    }

    /// <summary>
    /// 현재 대사 행의 화자·캐릭터 Sprite·효과를 UI에 적용함.
    /// </summary>
    private void ApplyCharacterVisuals(StoryScriptInfo_Object scriptRow)
    {
        // 1. 화자 이름과 각 이미지 배열의 안전한 인덱스 결과를 적용함.
        if (characterNameText != null)
        {
            characterNameText.text = scriptRow.speaker;
        }

        if (characterBody != null)
        {
            characterBody.sprite = GetSprite(characterBodySprites, scriptRow.charImage, "몸통");
        }

        if (characterFace != null)
        {
            characterFace.sprite = GetSprite(characterFaceSprites, scriptRow.faceImage, "표정");
        }

        if (characterEffect != null)
        {
            characterEffect.sprite = GetSprite(characterEffectSprites, scriptRow.effectImage, "효과");
        }

        // 2. charImage 0은 실루엣 연출, 그 외 값은 원래 색상으로 표시함.
        Color characterColor = scriptRow.charImage > 0 ? Color.white : Color.black;
        if (characterBody != null)
        {
            characterBody.color = characterColor;
        }

        if (characterFace != null)
        {
            characterFace.color = characterColor;
        }
    }

    /// <summary>
    /// Sprite 배열의 인덱스를 검사하고 유효한 이미지만 반환함.
    /// </summary>
    private Sprite GetSprite(Sprite[] sprites, int index, string imageRole)
    {
        if (sprites != null && index >= 0 && index < sprites.Length)
        {
            return sprites[index];
        }

        Debug.LogWarning($"[ScriptBox] {imageRole} 이미지 인덱스 {index}가 범위를 벗어났습니다.");
        return null;
    }

    /// <summary>
    /// 타이핑 중인 현재 문장을 Rich Text가 적용된 완성 문장으로 즉시 표시함.
    /// </summary>
    private void CompleteCurrentLineImmediately()
    {
        // 1. 실행 중인 타이핑 코루틴을 중단함.
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // 2. 전체 문장에 줄바꿈과 강조 태그를 적용하여 한 번에 표시함.
        formattedLineBuilder.Clear();
        AppendFormattedLine(formattedLineBuilder, runtime.Talk);
        if (loadingTxt != null)
        {
            loadingTxt.text = formattedLineBuilder.ToString();
        }

        // 3. 다음 클릭이 다음 대사 또는 종료로 처리되도록 상태를 바꿈.
        runtime.IsTyping = false;
        runtime.IsHighlighting = false;
        runtime.IsCompleted = false;
    }

    /// <summary>
    /// 문자열의 연속 공백 두 개를 줄바꿈 하나로 바꿈.
    /// </summary>
    private static string ReplaceDoubleSpacesWithLineBreak(string text)
    {
        return string.IsNullOrEmpty(text) ? string.Empty : text.Replace("  ", "\n");
    }

    /// <summary>
    /// 대사의 줄바꿈과 강조 제어 문자를 해석하여 지정한 StringBuilder에 Rich Text를 추가함.
    /// </summary>
    private static void AppendFormattedLine(StringBuilder builder, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        bool isHighlighting = false;
        for (int i = 0; i < text.Length; i++)
        {
            // 연속 공백 두 개는 데이터 테이블에서 사용하는 줄바꿈 표기.
            if (text[i] == ' ' && i + 1 < text.Length && text[i + 1] == ' ')
            {
                builder.Append('\n');
                i++;
                continue;
            }

            if (text[i] == '<')
            {
                isHighlighting = true;
                continue;
            }

            if (text[i] == '>')
            {
                isHighlighting = false;
                continue;
            }

            AppendCharacter(builder, text[i], isHighlighting);
        }
    }

    /// <summary>
    /// 강조 여부에 따라 한 글자 또는 한 글자를 감싼 Rich Text 태그를 버퍼에 추가함.
    /// </summary>
    private static void AppendCharacter(StringBuilder builder, char character, bool isHighlighting)
    {
        if (isHighlighting)
        {
            builder.Append(HighlightOpenTag);
            builder.Append(character);
            builder.Append(HighlightCloseTag);
            return;
        }

        builder.Append(character);
    }

    /// <summary>
    /// ScriptManager가 씬 초기화 순서 때문에 비어 있으면 실제 사용 시점에 다시 가져옴.
    /// </summary>
    private void ResolveScriptManager()
    {
        if (scriptManager == null)
        {
            scriptManager = ScriptManager.instance;
        }
    }

    /// <summary>
    /// 현재 대사 범위와 타이핑·로그 상태를 한곳에 묶은 값 형식 실행 상태.
    /// </summary>
    private struct DialogRuntimeState
    {
        public int LineIndex;
        public int EndId;
        public string Talk;
        public bool IsTyping;
        public bool IsLogOpen;
        public bool IsHighlighting;
        public bool IsCompleted;
    }
}

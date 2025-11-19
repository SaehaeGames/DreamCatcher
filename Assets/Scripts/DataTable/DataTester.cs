using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataTester : MonoBehaviour
{
    [Header("테스트 옵션")]
    public bool testScriptableObjects = false;
    public bool testJsonFiles = false;

    [Header("UI 설정")]
    public Text resultText;
    public Color passColor = Color.green;
    public Color failColor = Color.red;
    public Color warnColor = Color.yellow;

    private int passCount = 0;
    private int failCount = 0;
    private int warnCount = 0;

    public void RunAllTests()
    {
        ResetCounts();
        resultText.text = "데이터 테스트 시작...\n";

        if (testScriptableObjects)
            TestScriptableObjects();
        else
            LogResult("ScriptableObject 테스트 비활성화됨", warnColor);

        if (testJsonFiles)
            TestJsonFiles();
        else
            LogResult("JSON 테스트 비활성화됨", warnColor);

        PrintSummary();
    }
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR
        // 빌드 시 자동 실행
        RunAllTests();
#endif
    }

    private void TestScriptableObjects()
    {
        
    }

    private void TestJsonFiles()
    {
        string jsonFolder;
    }

    private void ResetCounts()
    {
        passCount = 0;
        failCount = 0;
        warnCount = 0;
        if (resultText != null)
            resultText.text = "";
    }

    private void LogResult(string message, Color color)
    {
        Debug.Log(message);
        if (resultText)
        {
            resultText.text += $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>\n";
        }
    }

    private void PrintSummary()
    {
        string summary = $"\n테스트 완료\n" +
                         $"PASS: {passCount}\n" +
                         $"WARN: {warnCount}\n" +
                         $"FAIL: {failCount}";

        Color summaryColor = failCount > 0 ? failColor :
                             (warnCount > 0 ? warnColor : passColor);

        LogResult(summary, summaryColor);
    }
}

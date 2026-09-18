using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최근 대사 로그를 씬 이동 후에도 유지하는 전역 관리자.
/// 로그 수를 제한하여 튜토리얼 대사 기록이 계속 누적되는 것을 막음.
/// </summary>
public class ScriptManager : MonoBehaviour
{
    private const int MaxLogCount = 10;

    public static ScriptManager instance;

    private readonly Queue<string> logMessages = new Queue<string>();

    /// <summary>
    /// 첫 인스턴스만 전역으로 유지하고 이후 씬에서 생성된 중복 관리자는 제거함.
    /// </summary>
    private void Awake()
    {
        // 1. 아직 관리자가 없으면 현재 인스턴스를 등록하고 씬 이동 후에도 유지함.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        // 2. 이미 다른 인스턴스가 있으면 중복 오브젝트를 제거함.
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 현재 인스턴스가 제거될 때 정적 참조가 파괴된 오브젝트를 가리키지 않게 정리함.
    /// </summary>
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// 새 대사를 큐 끝에 추가하고 제한 수를 넘으면 가장 오래된 항목을 제거함.
    /// </summary>
    public void AddLog(string message)
    {
        // 1. 표시할 대사를 최신 로그 끝에 추가함.
        logMessages.Enqueue(message ?? string.Empty);

        // 2. 최대 개수를 넘은 오래된 로그를 앞에서 제거함.
        while (logMessages.Count > MaxLogCount)
        {
            logMessages.Dequeue();
        }
    }

    /// <summary>
    /// 내부 큐가 외부에서 변경되지 않도록 현재 로그의 복사본을 반환함.
    /// </summary>
    public List<string> GetLogs()
    {
        return new List<string>(logMessages);
    }
}
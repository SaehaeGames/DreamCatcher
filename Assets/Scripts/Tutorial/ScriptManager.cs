using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager instance;
    private Queue<string> logMessages = new Queue<string>();
    private const int maxLogCount = 10;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddLog(string message)
    {
        logMessages.Enqueue(message);

        if(logMessages.Count > maxLogCount)
        {
            logMessages.Dequeue();
        }
    }

    public List<string> GetLogs()
    {
        return new List<string>(logMessages);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

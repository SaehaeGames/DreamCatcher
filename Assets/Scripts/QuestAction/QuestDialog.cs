using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDialog : MonoBehaviour
{
    private ScriptBox scriptBox;
    [Header("대사 시작 아이디-입력")]
    public int startId;
    [Header("대사 끝 아이디-입력")]
    public int endId;

    // Start is called before the first frame update
    void Start()
    {
        scriptBox = GameObject.FindObjectOfType<ScriptBox>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isCompleted = scriptBox.ReturnNextScript();
        if(isCompleted)
        {
            scriptBox.ScriptBoxOnOff(false);
        }
    }

    public void QuestDialogOn(int startId, int endId)
    {
        scriptBox.ScriptBoxOnOff(true);
        scriptBox.SetScriptBox(startId, endId);
    }
}

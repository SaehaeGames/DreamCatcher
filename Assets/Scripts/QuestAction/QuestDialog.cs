using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDialog : MonoBehaviour
{
    private ScriptBox scriptBox;
    [Header("��� ���� ���̵�-�Է�")]
    public int startId;
    [Header("��� �� ���̵�-�Է�")]
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

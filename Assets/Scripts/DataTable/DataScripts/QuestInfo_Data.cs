using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using UnityEditor;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/QuestInfo")]
[Serializable]
public class QuestInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1W0ML_fkdwZLb9MR4dElftbOhuofJutOvdotkpdRijQQ";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:D";
    private static string objectName = "QuestInfo";
    
    public List<QuestInfo_Object> dataList = new List<QuestInfo_Object>();

    public void UpdateQuestInfoData()
    {
        // BirdInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        dataList = GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<QuestInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet);

        AssetDatabase.SaveAssetIfDirty(this);   // 변동사항이 있다면 저장
    }

    public void InitializeInteriorInfoData()
    {
        // BirdInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<QuestInfo_Data>(CreateInstance<QuestInfo_Data>(), objectName);
    }
}

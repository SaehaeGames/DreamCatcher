using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using UnityEditor;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/QuestItem")]
[Serializable]
public class QuestItem_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1W0ML_fkdwZLb9MR4dElftbOhuofJutOvdotkpdRijQQ";
    private static long spreadSheetWorksheet = 131200927;
    private static string spreadSheetRange = "A2:C";
    private static string objectName = "QuestItem";

    public List<QuestItem_Object> dataList = new List<QuestItem_Object>();


    public void UpdateQuestItemData(Action onUpdateComplete)
    {
        // QuestItemInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<QuestItem_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // 변동사항 저장
            onUpdateComplete?.Invoke(); //onUpdateComplete 콜백 호출
        });
    }

    public void InitializeQuestItemData()
    {
        // QuestItemInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<QuestItem_Data>(CreateInstance<QuestItem_Data>(), objectName);
    }
}
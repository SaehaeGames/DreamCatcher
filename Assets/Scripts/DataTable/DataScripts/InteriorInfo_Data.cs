using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using UnityEditor;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/InteriorInfo")]
public class InteriorInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1F3nQXJxatyzvJHbba9H0B832GQ6aQh0cigMeJJxSLx4";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:E";
    private static string objectName = "InteriorInfo";

    public List<InteriorInfo_Object> dataList = new List<InteriorInfo_Object>();

    public void UpdateInteriorInfoData(Action onUpdateComplete)
    {
        // InteriorInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<InteriorInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // 변동사항 저장
            onUpdateComplete?.Invoke(); //onUpdateComplete 콜백 호출
        });
    }

    public void InitializeInteriorInfoData()
    {
        // InteriorInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<InteriorInfo_Data>(CreateInstance<InteriorInfo_Data>(), objectName);
    }
}

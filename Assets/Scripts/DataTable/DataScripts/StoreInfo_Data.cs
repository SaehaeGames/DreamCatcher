using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using UnityEditor;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/StoreInfo")]
public class StoreInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1Kfo_r8_sHcCpR3YIue9AwwLu6eY7IMGYuRwH7hwPPus";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:F";
    private static string objectName = "StoreInfo";

    public List<StoreInfo_Object> dataList = new List<StoreInfo_Object>();

    public void UpdateStoreInfoData(Action onUpdateComplete)
    {
        // StoreInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<StoreInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // 변동사항 저장
            onUpdateComplete?.Invoke(); //onUpdateComplete 콜백 호출
        });
    }

    public void InitializeStoreInfoData()
    {
        // StoreInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<StoreInfo_Data>(CreateInstance<StoreInfo_Data>(), objectName);
    }
}

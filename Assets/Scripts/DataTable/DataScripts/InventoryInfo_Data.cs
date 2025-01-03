using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/InventoryInfo")]    // 스크립터블 오브젝트 객체 생성
[Serializable]
public class InventoryInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1NDFau0x2iO2P8xjpw-8qKdu329kZz4oWsINolNrtFqM";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:C";
    private static string objectName = "InventoryInfo";

    public List<InventoryInfo_Object> dataList = new List<InventoryInfo_Object>();

    public void UpdateInventoryInfoData(Action onUpdateComplete)
    {
        // InventoryInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<InventoryInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // 변동사항 저장
            onUpdateComplete?.Invoke(); //onUpdateComplete 콜백 호출
        });
    }

    public void InitializeInventoryInfoData()
    {
        // InventoryInfo 스크립터블 오브젝트를 삭제하고 재생성하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<InventoryInfo_Data>(CreateInstance<InventoryInfo_Data>(), objectName);
    }
}

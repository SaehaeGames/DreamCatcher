using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
using UnityEditor;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/DreamInfo")]
public class DreamInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1mfsbRolc27mzfBjuKDV2WFLPayOgJWdpJEPKXtQcjhs";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:H";
    private static string objectName = "DreamInfo";

    public List<DreamInfo_Object> dataList = new List<DreamInfo_Object>();

    public void UpdateDreamInfoData()
    {
        // DreamInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<DreamInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            AssetDatabase.SaveAssetIfDirty(this);   // 변동사항이 있다면 저장
        });
    }

    public void InitializeBirdInfoData()
    {
        // DreamInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<DreamInfo_Data>(CreateInstance<DreamInfo_Data>(), objectName);
    }
}

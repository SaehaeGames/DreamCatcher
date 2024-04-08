using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName="DataTable", menuName="Scriptable Object Asset/StoryScriptInfo")]
[Serializable]
public class StoryScriptInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "10-I4KgO4n3KZMx-NcmLVCEmTT_W6Uj9I_-ynXLTNolg";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:I";
    private static string objectName = "StoryScriptInfo";

    public List<StoryScriptInfo_Object> dataList = new List<StoryScriptInfo_Object>();
    public void UpdateStoryScriptInfoData()
    {
        // StoryScriptInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<StoryScriptInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            AssetDatabase.SaveAssetIfDirty(this);   // 변동사항이 있다면 저장
        });

        AssetDatabase.SaveAssetIfDirty(this);   // 변동사항이 있다면 저장
    }

    public void InitializeStoryScriptInfoData()
    {
        // StoryScriptInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<StoryScriptInfo_Data>(CreateInstance<StoryScriptInfo_Data>(), objectName);
    }
}

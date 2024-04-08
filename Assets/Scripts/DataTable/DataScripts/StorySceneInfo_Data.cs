using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="DataTable", menuName ="Scriptable Object Asset/StorySceneInfo")]
[Serializable]
public class StorySceneInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "14moYHgh4H_et4Dj0CY6UtzB7eaOoR_C7Oi4GL5qeZqE";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:D";
    private static string objectName = "StorySceneInfo";

    public List<StorySceneInfo_Object> dataList = new List<StorySceneInfo_Object>();
    public void UpdateStorySceneInfoData()
    {
        // StorySceneInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<StorySceneInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            AssetDatabase.SaveAssetIfDirty(this);   // 변동사항이 있다면 저장
        });

        AssetDatabase.SaveAssetIfDirty(this);   // 변동사항이 있다면 저장
    }

    public void InitializeStorySceneInfoData()
    {
        // StorySceneInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<BirdInfo_Data>(CreateInstance<StorySceneInfo_Data>(), objectName);
    }
}

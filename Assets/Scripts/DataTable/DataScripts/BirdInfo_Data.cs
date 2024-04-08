using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/BirdInfo")]    // 스크립터블 오브젝트 객체 생성
[Serializable]
public class BirdInfo_Data : ScriptableObject
{

    private static string spreadSheetAddress = "1YOwI5F5kJMbnze0xKXb7rbffva1ZPjqRsKsX5U9wmHE";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:H";
    private static string objectName = "BirdInfo";

    public List<BirdInfo_Object> dataList = new List<BirdInfo_Object>();

    public void UpdateBirdInfoData()
    {
        // BirdInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<BirdInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) => 
        {
            dataList = _loadedDataList;
            AssetDatabase.SaveAssetIfDirty(this);   // 변동사항이 있다면 저장
        });
    }

    public void InitializeBirdInfoData()
    {
        // BirdInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<BirdInfo_Data>(CreateInstance<BirdInfo_Data>(), objectName); 
    }
}
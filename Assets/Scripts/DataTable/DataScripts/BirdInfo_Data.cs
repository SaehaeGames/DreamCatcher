using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/BirdInfo")]    // 스크립터블 오브젝트 객체 생성
[Serializable]
public class BirdInfo_Data : ScriptableObject
{

    private static string spreadSheetAddress = "1YOwI5F5kJMbnze0xKXb7rbffva1ZPjqRsKsX5U9wmHE";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:H";
    private static string objectName = "BirdInfo";

    public List<BirdInfo_Object> dataList = new List<BirdInfo_Object>();

    public void UpdateBirdInfoData(Action onUpdateComplete)
    {
        // BirdInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<BirdInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) => 
        {
            dataList = _loadedDataList;
            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // 변동사항 저장
            onUpdateComplete?.Invoke(); //onUpdateComplete 콜백 호출
        });
    }

    public void InitializeBirdInfoData()
    {
        // BirdInfo 스크립터블 오브젝트를 삭제하고 재생성하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<BirdInfo_Data>(CreateInstance<BirdInfo_Data>(), objectName); 
    }
}
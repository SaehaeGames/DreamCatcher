using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/DiaryInfo")]
[Serializable]

public class DiaryInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1_xBMgdtjaCxmzynKCLlLGgYYzeI6esmOSepfpRczaWI";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:B";
    private static string objectName = "DiaryInfo";

    public List<DiaryInfo_Object> dataList = new List<DiaryInfo_Object>();

    public void UpdateDiaryInfoData(Action onUpdateComplete)
    {
        // QuestInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<DiaryInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // 변동사항 저장
            onUpdateComplete?.Invoke(); //onUpdateComplete 콜백 호출
        });
    }

    public void InitializeInteriorInfoData()
    {
        // QuestInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<DiaryInfo_Data>(CreateInstance<DiaryInfo_Data>(), objectName);
    }

    public DiaryInfo_Object GetCharonInfo(string id)
    {
        DiaryInfo_Object data = dataList.Find(x => x.id == id);

        if (data == null)
        {
            Debug.LogError($"DiaryInfo not found : {id}");
            return null;
        }

        return data;
    }
}

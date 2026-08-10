using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/CharonInfo")]
[Serializable]

public class CharonInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1S4iUJUMjzXE_fY4VjHhkNVwWb0tC4pD5MZSavRnYqBo";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:F";
    private static string objectName = "CharonInfo";

    public List<CharonInfo_Object> dataList = new List<CharonInfo_Object>();

    public void UpdateCharonInfoData(Action onUpdateComplete)
    {
        // QuestInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<CharonInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // 변동사항 저장
            onUpdateComplete?.Invoke(); //onUpdateComplete 콜백 호출
        });
    }

    public void InitializeInteriorInfoData()
    {
        // QuestInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<CharonInfo_Data>(CreateInstance<CharonInfo_Data>(), objectName);
    }

    public CharonInfo_Object GetCharonInfo(string id)
    {
        CharonInfo_Object data = dataList.Find(x => x.id == id);

        if (data == null)
        {
            Debug.LogError($"CharonInfo not found : {id}");
            return null;
        }

        return data;
    }
}

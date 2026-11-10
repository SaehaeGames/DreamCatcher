using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/RepeatQuestInfo")]
[Serializable]
public class RepeatQuestInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "12ZZpqCNPiOZuMZLQKxyB66TMXlT9cgfB6Bkt_jIXFec";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:D";
    private static string objectName = "RepeatQuestInfo";

    public List<RepeatQuestInfo_Object> dataList = new List<RepeatQuestInfo_Object>();

    public void UpdateRpeatQuestInfoData(Action onUpdateComplete)
    {
        // QuestInfo 스크립터블 오브젝트 데이터를 업데이트하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<RepeatQuestInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // 변동사항 저장
            onUpdateComplete?.Invoke(); //onUpdateComplete 콜백 호출
        });
    }

    public void InitializeInteriorInfoData()
    {
        // QuestInfo 스크립터블 오브젝트를 초기화하는 함수

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<RepeatQuestInfo_Data>(CreateInstance<RepeatQuestInfo_Data>(), objectName);
    }

    public RepeatQuestInfo_Object GetRepeatQuestInfo(string id)
    {
        RepeatQuestInfo_Object data = dataList.Find(x => x.id == id);

        if (data == null)
        {
            Debug.LogError($"RepeatQuestInfo not found : {id}");
            return null;
        }

        return data;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;


[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/QuestDreamCatcherInfo")]
[Serializable]
public class QuestDreamCatcherInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1j7yujnOtluVKHMKu2zC_EBZ7mJUfYtPd1Wjad3aE1XY";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:G";
    private static string objectName = "QuestDreamCatcherInfo";

    public List<QuestDreamCatcherInfo_Object> dataList = new List<QuestDreamCatcherInfo_Object>();

    public void UpdateQuestDreamCatcherInfoData(Action onUpdateComplete)
    {
        Debug.Log("UpdateQuestDreamCatcherInfoData 호출됨");
        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<QuestDreamCatcherInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            

            Debug.Log("스프레드시트에서 로드 완료: " + _loadedDataList.Count);
            dataList = _loadedDataList;

            foreach (var item in dataList)
            {
                item.Parse();
            }

            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // 변동사항 저장

            Debug.Log("ScriptableObject 저장 완료");
            onUpdateComplete?.Invoke(); //onUpdateComplete 콜백 호출
        });
    }

    public void InitializeQuestDreamCatcherInfoData()
    {
        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<QuestDreamCatcherInfo_Data>(CreateInstance<QuestDreamCatcherInfo_Data>(), objectName);
    }
}

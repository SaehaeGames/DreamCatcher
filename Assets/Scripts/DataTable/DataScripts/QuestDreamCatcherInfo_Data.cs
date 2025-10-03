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
        Debug.Log("UpdateQuestDreamCatcherInfoData ȣ���");
        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<QuestDreamCatcherInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            

            Debug.Log("���������Ʈ���� �ε� �Ϸ�: " + _loadedDataList.Count);
            dataList = _loadedDataList;

            foreach (var item in dataList)
            {
                item.Parse();
            }

            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // �������� ����

            Debug.Log("ScriptableObject ���� �Ϸ�");
            onUpdateComplete?.Invoke(); //onUpdateComplete �ݹ� ȣ��
        });
    }

    public void InitializeQuestDreamCatcherInfoData()
    {
        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<QuestDreamCatcherInfo_Data>(CreateInstance<QuestDreamCatcherInfo_Data>(), objectName);
    }
}

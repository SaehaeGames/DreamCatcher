using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/InventoryInfo")]    // ��ũ���ͺ� ������Ʈ ��ü ����
[Serializable]
public class InventoryInfo_Data : ScriptableObject
{
    private static string spreadSheetAddress = "1NDFau0x2iO2P8xjpw-8qKdu329kZz4oWsINolNrtFqM";
    private static long spreadSheetWorksheet = 0;
    private static string spreadSheetRange = "A2:C";
    private static string objectName = "InventoryInfo";

    public List<InventoryInfo_Object> dataList = new List<InventoryInfo_Object>();

    public void UpdateInventoryInfoData(Action onUpdateComplete)
    {
        // InventoryInfo ��ũ���ͺ� ������Ʈ �����͸� ������Ʈ�ϴ� �Լ�

        GameManager.instance.GetComponent<ScriptableObjectManager>().GetScriptableObjectToObjectList<InventoryInfo_Object>(spreadSheetAddress, spreadSheetRange, spreadSheetWorksheet, (_loadedDataList) =>
        {
            dataList = _loadedDataList;
            GameManager.instance.GetComponent<ScriptableObjectManager>().SaveScriptableObjectAtPath(objectName);    // �������� ����
            onUpdateComplete?.Invoke(); //onUpdateComplete �ݹ� ȣ��
        });
    }

    public void InitializeInventoryInfoData()
    {
        // InventoryInfo ��ũ���ͺ� ������Ʈ�� �����ϰ� ������ϴ� �Լ�

        GameManager.instance.GetComponent<ScriptableObjectManager>().InitializeScriptableObject<InventoryInfo_Data>(CreateInstance<InventoryInfo_Data>(), objectName);
    }
}

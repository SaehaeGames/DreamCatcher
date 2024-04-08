using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour
{
    // ��ũ���ͺ� ������Ʈ ���� Ŭ����


    private static string objectAddress_start = "Assets/Scripts/DataTable/ScritableObjects/";
    private static string objectAddress_end = ".asset";


    public void InitializeScriptableObject<T>(UnityEngine.Object asset, string objectName)
    {
        // ��ũ���ͺ� ������Ʈ�� �ʱ�ȭ�ϴ� �Լ�
        // �������� ��Ʈ ���� �� �����

        string objectAddress = objectAddress_start + objectName + objectAddress_end;   // ��ũ���ͺ� ������Ʈ�� �ּ�

        AssetDatabase.DeleteAsset(objectAddress_start);  // ���� ��ũ���ͺ� ������Ʈ ����
        AssetDatabase.CreateAsset(asset, objectAddress_start);   // ��ũ���ͺ� ������Ʈ �ű� ����
        AssetDatabase.SaveAssets(); // ���� ����
    }

    public void GetScriptableObjectToObjectList<T>(string address, string range, long sheetID, Action<List<T>> onDataLoaded)
    {
        // �������� ��Ʈ���� ������ �����͸� ��ũ���ͺ� ������Ʈ�� �����ϴ� �Լ�
        // �������� ��Ʈ�� ������ ����Ʈ�� ������

        this.GetComponent<SpreadSheetManager>().GetSpreadSheetDataToObject<T>(address, range, sheetID, (dataList) =>
        {
            onDataLoaded(dataList);
        });
    }
}

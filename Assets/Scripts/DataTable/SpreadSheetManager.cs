using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class SpreadSheetManager : MonoBehaviour
{
    // �������� ��Ʈ ������ �Ŵ���

    private static string defaultAddress = "https://docs.google.com/spreadsheets/d/"; // �������� ��Ʈ �ּ��� ���� �κ�

    public List<T> GetSpreadSheetDataToObject<T>(string address, string range, long sheetID)
    {
        // �ּ�, ��Ʈ�� ����, ��ƮID�� �Է��ϸ� �������� ��Ʈ�� �����͸� ��ü ����Ʈ�� ��ȯ�ϴ� �Լ�

        return GetSpreadSheetDatas<T>(GetSpreadSheetDataToText(address, range, sheetID));
    }

    public List<T> GetSpreadSheetDatas<T>(string data)
    {
        // ��Ʈ���� ��Ʈ Ŭ���� �ν��Ͻ�ȭ �� �����͸� ����Ʈȭ
        
        List<T> returnList = new List<T>();
        string[] splitedData = data.Split('\n');    // \n�� �������� �и�

        foreach(string element in splitedData)
        {
            string[] datas = element.Split('\t');   // \t�� �������� �и�
            returnList.Add(GetSpreadSheetData<T>(datas));   // ����Ʈ�� �߰�
        }

        return returnList;
    }


    public T GetSpreadSheetData<T>(string[] datas)
    {
        // �������� ��Ʈ �ؽ�Ʈ ������ Ŭ���� �ν��Ͻ�ȭ

        object data = Activator.CreateInstance(typeof(T));

        // Ŭ������ �ִ� �������� ������� ������ �迭
        FieldInfo[] fields = typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        for (int i = 0; i < datas.Length; i++)
        {
            try
            {
                // string > parse
                Type type = fields[i].FieldType;

                if (string.IsNullOrEmpty(datas[i])) continue;

                // ������ �´� �ڷ������� �Ľ��ؼ� ����
                if (type == typeof(int))
                    fields[i].SetValue(data, int.Parse(datas[i]));
                else if (type == typeof(float))
                    fields[i].SetValue(data, float.Parse(datas[i]));
                else if (type == typeof(bool))
                    fields[i].SetValue(data, bool.Parse(datas[i]));
                else if (type == typeof(string))
                    fields[i].SetValue(data, datas[i]);
                //enum�̶��
                else
                    fields[i].SetValue(data, Enum.Parse(type, datas[i]));
            }
            catch (Exception e)
            {
                Debug.LogError($"�������� ��Ʈ �Ľ� ���� : {e.Message}");
            }
        }

        return (T)data;
    }

    public string GetSpreadSheetDataToText(string address, string range, long sheetID)
    {
        // �ּ�, ��Ʈ�� ����, ��ƮID�� �Է��ϸ� �������� ��Ʈ�� �����͸� �ϳ��� �ؽ�Ʈ�� ��ȯ�ϴ� �Լ�

        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(address, range, sheetID));
        return www.downloadHandler.text;
    }

    public static string GetTSVAddress(string address, string range, long sheetID)
    {
        // �ּ�, ��Ʈ�� ����, ��ƮID�� �Է��ϸ� TSV �ּҷ� ��ȯ�ϴ� �Լ�

        return defaultAddress + $"{address}/export?format=tsv&range={range}&grid={sheetID}";
    }
}

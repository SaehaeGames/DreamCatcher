using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonManager
{
    // Json ������ �ε�/���̺� ���� Ŭ����

    public T LoadData<T>(string fileName) where T : new()
    {
        // �����͸� �ε��ϴ� �Լ�

        string savedPath = GetPath(fileName);
        if (!File.Exists(savedPath))              // ������ �������� �ʴ´ٸ� ����
        {
            T data = new T();
            SaveData<T>(fileName, data);
            return data;
        }
        else                                     // ������ �����Ѵٸ� �ε�
        {
            string data = File.ReadAllText(savedPath);
            return JsonUtility.FromJson<T>(data);
        }
    }

    public List<T> LoadDataList<T> (string fileName) where T : new()
    {
        // ������ ����Ʈ�� �ε��ϴ� �Լ�

        string savedPath = GetPath(fileName); 
        if (!File.Exists(savedPath))             // ������ �������� �ʴ´ٸ� ����
        {
            List<T> dataList = new List<T>();
            SaveDataList(fileName, dataList);
            return dataList;
        }
        else                                     // ������ �����Ѵٸ� �ε�
        {
            string data = File.ReadAllText(savedPath);
            return JsonUtility.FromJson<Wrapper<T>>(data).datalist;
        }
    }

    public void SaveData<T>(string fileName, T data)
    {
        // �����͸� Json���� �����ϴ� �Լ�

        string savedPath = GetPath(fileName); 
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(savedPath, jsonData);
    }

    public void SaveDataList<T>(string fileName, List<T> dataList)
    {
        // ������ ����Ʈ�� Json���� �����ϴ� �Լ�

        string savedPath = GetPath(fileName);

        Wrapper<T> wrapper = new Wrapper<T> { datalist = dataList };
        string jsonData = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(savedPath, jsonData);
    }

    private string GetPath(string _fileName)
    {
        // ���� ���� ���
        string folderPath;

#if UNITY_EDITOR
        folderPath = Application.dataPath + "/Saves/";
#elif UNITY_ANDROID
        folderPath = Application.persistentDataPath + "/Saves/";
#elif UNITY_IPHONE
        folderPath = Application.persistentDataPath + "/Saves/";
#else
        folderPath = Application.dataPath + "/Saves/";
#endif

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return folderPath + _fileName + ".json";
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> datalist;
    }
}

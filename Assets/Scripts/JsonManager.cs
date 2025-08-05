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

#if DEVELOPMENT_BUILD // ���� ������ �� ������ ���ҽ����� �ε�
        Debug.Log("Json : Development Build - ������ Resource���� �ҷ��ɴϴ�.");
        TextAsset defaultJson = Resources.Load<TextAsset>("DefaultJsonData/" + fileName);
        if (defaultJson != null)
        {
            T data = JsonUtility.FromJson<T>(defaultJson.text);
            SaveData<T>(fileName, data);
            return data;
        }
        else
        {
            T data = new T();
            SaveData<T>(fileName, data);
            return data;
        }
#else // ������ ������ ���� ���� ���� ���� �켱
        if (!File.Exists(savedPath))
        {
            Debug.Log("Json : playerData can't find in savePath");
            TextAsset defaultJson = Resources.Load<TextAsset>("DefaultJsonData/" + fileName);
            if (defaultJson != null)
            {
                Debug.Log("Json : defaultJson find in Resource");
                T data = JsonUtility.FromJson<T>(defaultJson.text);
                SaveData<T>(fileName, data);
                return data;
            }
            else
            {
                Debug.Log("Json : defaultJson can't find in Resource");
                T data = new T();
                SaveData<T>(fileName, data);
                return data;
            }
        }
        else
        {
            Debug.Log("Json : playerData find in savePath");
            Debug.Log("Json : Application.persistentDataPath: " + Application.persistentDataPath);
            Debug.Log("Json : GetPath: " + GetPath(fileName));
            string data = File.ReadAllText(savedPath);
            return JsonUtility.FromJson<T>(data);
        }
#endif
    }

    public List<T> LoadDataList<T> (string fileName) where T : new()
    {
        // ������ ����Ʈ�� �ε��ϴ� �Լ�
        string savedPath = GetPath(fileName);

#if DEVELOPMENT_BUILD // ���� ������ �� �׻� Resource���� ���� �ʱ�ȭ
            Debug.Log("Json List : Development Build - ������ Resource���� �ҷ��ɴϴ�.");
        TextAsset defaultJson = Resources.Load<TextAsset>("DefaultJsonData/" + fileName);
        if (defaultJson != null)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(defaultJson.text);
            List<T> dataList = wrapper.datalist;
            SaveDataList(fileName, dataList);
            return dataList;
        }
        else
        {
            List<T> dataList = new List<T>();
            SaveDataList(fileName, dataList);
            return dataList;
        }
#else // ������ ������ ��� ����� ������ �켱
        if (!File.Exists(savedPath))
        {
            Debug.Log("Json List : playerData can't find in savePath");
            TextAsset defaultJson = Resources.Load<TextAsset>("DefaultJsonData/" + fileName);
            if (defaultJson != null)
            {
                Debug.Log("Json List : defaultJson find in Resource");
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(defaultJson.text);
                List<T> dataList = wrapper.datalist;
                SaveDataList(fileName, dataList);
                return dataList;
            }
            else
            {
                Debug.Log("Json List : defaultJson can't find in Resource");
                List<T> dataList = new List<T>();
                SaveDataList(fileName, dataList);
                return dataList;
            }
        }
        else
        {
            Debug.Log("Json List : playerData find in savePath");
            Debug.Log("Json List : Application.persistentDataPath: " + Application.persistentDataPath);
            Debug.Log("Json List : GetPath: " + GetPath(fileName));
            string data = File.ReadAllText(savedPath);
            return JsonUtility.FromJson<Wrapper<T>>(data).datalist;
        }
#endif
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

    public static void ClearSavedData()
    {
        string folderPath = Application.persistentDataPath + "/Saves/";

        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }

            Debug.Log("��� ���� �����͸� �ʱ�ȭ�߽��ϴ�.");
        }
        else
        {
            Debug.Log("���� ������ �������� �ʽ��ϴ�.");
        }
    }
}

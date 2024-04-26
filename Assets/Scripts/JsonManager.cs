using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonManager
{
    public T LoadData<T>(string _fileName) where T : new()
    {
        // �����͸� �ε��ϴ� �Լ�
        // T�� �����ڸ� ���� Ŭ������� ���׸� �Ű����� ���� �߰�

        string savePath = getPath(_fileName);    //���� ���� ���

        if (!File.Exists(savePath))  //������ �������� �ʴ´ٸ�
        {
            Debug.Log("������ �������� ����");
            return new T();
        }
        else    //������ �����Ѵٸ�
        {
            return DataLoadText<T>(_fileName); //���� �ε�
        }
    }

    public T GetData<T>(string _fileName) where T : new()
    {
        //�����͸� ��ȯ�ϴ� �Լ�

        T container = LoadData<T>(_fileName);
        return container;
    }

    public void DataSaveText<T>(string _fileName, T data)
    {
        //�����͸� Json���� �����ϴ� �Լ�

        try
        {
            string savePath = getPath(_fileName);    //���� ���� ���
            string saveJson = JsonUtility.ToJson(data, true);

            File.WriteAllText(savePath, saveJson);
        }
        catch (FileNotFoundException e)
        {
            Debug.Log("The file was not found:" + e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.Log("The directory was not found: " + e.Message);
        }
        catch (IOException e)
        {
            Debug.Log("The file could not be opened:" + e.Message);
        }
    }

    public T DataLoadText<T>(string _fileName)
    {
        //Json �����͸� �ҷ����� �Լ�

        try
        {
            string savePath = getPath(_fileName);    //���� ���� ���
            string loadJson = File.ReadAllText(savePath);

            T t = JsonUtility.FromJson<T>(loadJson);
            return t;
        }
        catch (FileNotFoundException e)
        {
            Debug.Log("The file was not found:" + e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.Log("The directory was not found: " + e.Message);
        }
        catch (IOException e)
        {
            Debug.Log("The file could not be opened:" + e.Message);
        }
        return default;
    }

    private static string getPath(string _fileName)
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Saves/" + _fileName + ".json";
#elif UNITY_ANDROID
        return Application.persistentDataPath + "/Saves/" + _fileName + ".json";
#elif UNITY_IPHONE
        return Application.persistentDataPath + "/Saves/" + _fileName + ".json";
#else
        return Application.dataPath + "/Saves/" + _fileName + ".json";
#endif
    }
}

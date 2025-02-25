using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonManager
{
    // Json 데이터 로드/세이브 관리 클래스

    public T LoadData<T>(string fileName) where T : new()
    {
        // 데이터를 로드하는 함수

        string savedPath = GetPath(fileName);
        if (!File.Exists(savedPath))              // 파일이 존재하지 않는다면 생성
        {
            T data = new T();
            SaveData<T>(fileName, data);
            return data;
        }
        else                                     // 파일이 존재한다면 로드
        {
            string data = File.ReadAllText(savedPath);
            return JsonUtility.FromJson<T>(data);
        }
    }

    public List<T> LoadDataList<T> (string fileName) where T : new()
    {
        // 데이터 리스트를 로드하는 함수

        string savedPath = GetPath(fileName); 
        if (!File.Exists(savedPath))             // 파일이 존재하지 않는다면 생성
        {
            List<T> dataList = new List<T>();
            SaveDataList(fileName, dataList);
            return dataList;
        }
        else                                     // 파일이 존재한다면 로드
        {
            string data = File.ReadAllText(savedPath);
            return JsonUtility.FromJson<Wrapper<T>>(data).datalist;
        }
    }

    public void SaveData<T>(string fileName, T data)
    {
        // 데이터를 Json으로 저장하는 함수

        string savedPath = GetPath(fileName); 
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(savedPath, jsonData);
    }

    public void SaveDataList<T>(string fileName, List<T> dataList)
    {
        // 데이터 리스트를 Json으로 저장하는 함수

        string savedPath = GetPath(fileName);

        Wrapper<T> wrapper = new Wrapper<T> { datalist = dataList };
        string jsonData = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(savedPath, jsonData);
    }

    private string GetPath(string _fileName)
    {
        // 파일 저장 경로
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

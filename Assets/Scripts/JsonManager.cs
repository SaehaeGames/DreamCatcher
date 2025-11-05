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

#if DEVELOPMENT_BUILD && !UNITY_EDITOR// 개발 빌드일 땐 무조건 리소스에서 로드
        Debug.Log("Json : Development Build - 강제로 Resource에서 불러옵니다.");
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
#else // 릴리즈 빌드일 때만 기존 저장 파일 우선
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
        // 데이터 리스트를 로드하는 함수
        string savedPath = GetPath(fileName);

#if DEVELOPMENT_BUILD // 개발 빌드일 땐 항상 Resource에서 강제 초기화
            Debug.Log("Json List : Development Build - 강제로 Resource에서 불러옵니다.");
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
#else // 릴리즈 빌드일 경우 저장된 데이터 우선
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

            Debug.Log("모든 저장 데이터를 초기화했습니다.");
        }
        else
        {
            Debug.Log("저장 폴더가 존재하지 않습니다.");
        }
    }
}

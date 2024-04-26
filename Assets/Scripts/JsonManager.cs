using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonManager
{
    public T LoadData<T>(string _fileName) where T : new()
    {
        // 데이터를 로드하는 함수
        // T가 생성자를 가진 클래스라는 제네릭 매개변수 제약 추가

        string savePath = getPath(_fileName);    //저장 파일 경로

        if (!File.Exists(savePath))  //파일이 존재하지 않는다면
        {
            Debug.Log("파일이 존재하지 않음");
            return new T();
        }
        else    //파일이 존재한다면
        {
            return DataLoadText<T>(_fileName); //파일 로드
        }
    }

    public T GetData<T>(string _fileName) where T : new()
    {
        //데이터를 반환하는 함수

        T container = LoadData<T>(_fileName);
        return container;
    }

    public void DataSaveText<T>(string _fileName, T data)
    {
        //데이터를 Json으로 저장하는 함수

        try
        {
            string savePath = getPath(_fileName);    //저장 파일 경로
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
        //Json 데이터를 불러오는 함수

        try
        {
            string savePath = getPath(_fileName);    //저장 파일 경로
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

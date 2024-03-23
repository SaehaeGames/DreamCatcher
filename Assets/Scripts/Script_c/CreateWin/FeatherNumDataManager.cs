using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FeatherNumDataManager : MonoBehaviour
{
    private static string fileName = "FeatherNumInfo";
    public MyFeatherNumber featherData = new MyFeatherNumber();    //플레이어 깃털 정보

    public void LoadFeatherJson()
    {
        string savePath = getPath(fileName);

        if (!File.Exists(savePath))
        {
            Debug.Log("파일 존재하지 않음 -> 파일 새로 만들기");
            this.featherData = new MyFeatherNumber();
            DataSaveText<MyFeatherNumber>(featherData);
        }
        else
        {
            this.featherData = DataLoadText<MyFeatherNumber>();
            Debug.Log("Json 파일 로드 성공");
        }
    }

    // Json파일 Get 함수
    public MyFeatherNumber GetFeatherData()
    {
        LoadFeatherJson();
        return featherData;
    }

    public T DataLoadText<T>()
    {
        //Json 데이터를 불러오는 함수

        try
        {
            string savePath = getPath(fileName);    //저장 파일 경로
            string jdata = File.ReadAllText(savePath);

           T t = JsonUtility.FromJson<T>(jdata);
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

    public void DataSaveText<T>(T data)
    {
        //데이터를 Json으로 저장하는 함수

        try
        {
            string savePath = getPath(fileName);    //저장 파일 경로
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


    private static string getPath(string fileName)
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Saves/" + fileName + ".json";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"/Saves/"+ fileName + ".json";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+ fileName + ".json";
#else
        return Application.dataPath +"/"+fileName + ".json";
#endif
    }
}

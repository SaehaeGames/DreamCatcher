using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DreamCatcherDataManager : MonoBehaviour
{
    private static string fileName = "DreamCatcherListData";
    public MyDreamCatcher dreamCatcherData = new MyDreamCatcher();

    public void LoadDreamCatcherJson()
    {
        string savePath = getPath(fileName);

        if(!File.Exists(savePath))
        {
            Debug.Log("파일 존재하지 않음 -> 파일 새로 만들기");
            this.dreamCatcherData = new MyDreamCatcher();
            DataSaveText<MyDreamCatcher>(dreamCatcherData);
        }
        else
        {
            this.dreamCatcherData = DataLoadText<MyDreamCatcher>();
            Debug.Log("DreamCatcherListData : Json 파일 로드 성공");
        }
    }

    public T DataLoadText<T>()
    {
        try
        {
            string savePath = getPath(fileName);
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
            string savePath = getPath(fileName);
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

    public MyDreamCatcher GetDreamCatcherData()
    {
        LoadDreamCatcherJson();
        return dreamCatcherData;
    }

    //파일위치
    private static string getPath(string fileName)
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Saves/" + fileName + ".json";
#elif UNITY_ANDROID
        return Application.persistentDataPath+ "/" + fileName + ".json";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+ fileName + ".json";
#else
        return Application.dataPath +"/"+ fileName + ".json";
#endif
    }
}

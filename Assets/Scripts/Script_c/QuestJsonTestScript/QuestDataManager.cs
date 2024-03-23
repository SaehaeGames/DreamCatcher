using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QuestDataManager : MonoBehaviour
{
    private static string fileName = "QuestData";
    public MyQuest questData = new MyQuest();

    public void LoadJson()
    {
        string savePath = getPath(fileName);

        if (!File.Exists(savePath))
        {
            Debug.Log("파일 존재하지 않음 -> 파일 새롬 만들기");
            this.questData = new MyQuest();
            DataSaveText<MyQuest>(questData);
        }
        else
        {
            this.questData = DataLoadText<MyQuest>();
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

    public MyQuest GetQuestData()
    {
        LoadJson();
        return questData;
    }
    //파일위치
    private static string getPath(string fileName)
    {
#if UNITY_EDITOR
        return Application.persistentDataPath + "/Saves/" + fileName + ".json";
#elif UNITY_ANDROID
        return Application.persistentDataPath+ "/" + fileName + ".json";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+ fileName + ".json";
#else
        return Application.persistentDataPath +"/"+ fileName + ".json";
#endif
    }
}

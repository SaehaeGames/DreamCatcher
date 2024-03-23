using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BirdJSON : MonoBehaviour
{
    //새 정보를 저장하는 스크립트(새가 나타났는지, 무슨 새인지)

    private static string fileName = "BirdDataFile";       //파일 이름
    BirdContainer birdContainer;    //새 데이터 변수

    public void LoadTimerData()
    {
        //데이터를 로드하는 함수

        string savePath = getPath(fileName);    //저장 파일 경로

        if (!File.Exists(savePath))  //파일이 존재하지 않는다면
        {
            this.birdContainer = new BirdContainer();  //객체 생성
        }
        else    //파일이 존재한다면
        {
            this.birdContainer = DataLoadText<BirdContainer>(); //파일 로드
        }
    }

    public BirdContainer GetBirdData()
    {
        //데이터를 반환하는 함수

        LoadTimerData();
        return birdContainer;
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

    public T DataLoadText<T>()
    {
        //Json 데이터를 불러오는 함수

        try
        {
            string savePath = getPath(fileName);    //저장 파일 경로
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

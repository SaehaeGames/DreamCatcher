using UnityEngine;
using System.Collections;
using GoogleSheetsToUnity;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using GoogleSheetsToUnity.ThirdPary;
using UnityEditor;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/SubQuestInfo")]
public class SubQuest_Data : ScriptableObject
{
    private static string fileName = "SubQuestFile";
    public SubQuest_Object curSubQuest;

    public void test(int num)
    {
        curSubQuest.thread = num;
        DataSaveText();
    }

    public void LoadSubQuestInfoData()
    {
        //데이터를 로드하는 함수

        string savePath = getPath(fileName);    //저장 파일 경로
        //string savePath = Application.persistentDataPath + fileName + ".json";

        if (!File.Exists(savePath))  //파일이 존재하지 않는다면
        {
            
            Debug.Log("존재하지 않아서 생성 및 저장");
        }
        else    //파일이 존재한다면
        {
            DataLoadText(); //파일 로드

/*            SubQuest_Object subquest_data = CreateInstance<SubQuest_Object>();
            subquest_data.datalist = this.datalist;
            AssetDatabase.CreateAsset(subquest_data, "Assets/Scripts/DataTable/ScritableObjects/subquest.asset");
            AssetDatabase.SaveAssets();
            Debug.Log(AssetDatabase.GetAllAssetPaths());*/
        }
    }

    public void DataSaveText()
    {
        //데이터를 Json으로 저장하는 함수
        string savePath = getPath(fileName);    //저장 파일 경로
        File.WriteAllText(savePath, JsonUtility.ToJson(curSubQuest, true));
    }

    public void DataLoadText()
    {
        //Json 데이터를 불러오는 함수

        string savePath = getPath(fileName);    //저장 파일 경로
        string[] loadJson = File.ReadAllLines(savePath);

        for (int i = 0; i < loadJson.Length; i++)
        {
            string curText1 = loadJson[i];

            if (curText1.Equals("{"))
            {
                string oneItem = "{";

                for (int j = i + 1; j < loadJson.Length; j++)
                {
                    string curText2 = loadJson[j];
                    if (curText2.Equals("}, "))
                    {
                        oneItem += "}";

                        curSubQuest = JsonUtility.FromJson<SubQuest_Object>(oneItem);
                        break;
                    }
                    else
                    {
                        oneItem += curText2;
                    }
                }
            }
        }
    }

    private static string getPath(string fileName)
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Saves/" + fileName + ".json";
#elif UNITY_ANDROID
        return Application.persistentDataPath + "/Saves/" + fileName + ".json";
#elif UNITY_IPHONE
        return Application.persistentDataPath + "/Saves/" + fileName + ".json";
#else
        return Application.persistentDataPath + "/Saves/" + fileName + ".json";
#endif
    }
}
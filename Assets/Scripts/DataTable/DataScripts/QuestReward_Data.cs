using UnityEngine;
using System.Collections;
using GoogleSheetsToUnity;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using GoogleSheetsToUnity.ThirdPary;
using UnityEditor;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/QuestRewardnfo")]
[Serializable]
public class QuestReward_Data : ScriptableObject
{

    [HideInInspector]
    public string associatedSheet = "1W0ML_fkdwZLb9MR4dElftbOhuofJutOvdotkpdRijQQ";
    [HideInInspector]
    public string associatedWorksheet = "questreward";
    private static string fileName = "QuestRewardFile";

    public int startid = 9001;
    public int endid = 9011;

    public List<QuestReward_Object> datalist = new List<QuestReward_Object>();

    public void LoadQuestRewardInfoData()
    {
        //데이터를 로드하는 함수

        string savePath = getPath(fileName);    //저장 파일 경로

        if (!File.Exists(savePath))  //파일이 존재하지 않는다면
        {
            UpdateQuestRewardInfoData();  //데이터 테이블 가져오기

            Debug.Log("존재하지 않아서 생성 및 저장");
        }
        else    //파일이 존재한다면
        {
            DataLoadText(); //파일 로드

/*            QuestReward_Object questreward_data = CreateInstance<QuestReward_Object>();
            questreward_data.datalist = this.datalist;
            AssetDatabase.CreateAsset(questreward_data, "Assets/Scripts/DataTable/ScritableObjects/QuestReward.asset");
            AssetDatabase.SaveAssets();
            Debug.Log(AssetDatabase.GetAllAssetPaths());*/

        }
    }

    public void UpdateQuestRewardInfoData()
    {
        //questresard업데이트 할 때 호출하는 함수

        UpdateStats(UpdateMethod);  //데이터 업데이트
    }

    internal void UpdateStats(GstuSpreadSheet ss)
    {
        datalist.Clear();

        for (int i = startid; i <= endid; i++)
        {
            QuestReward_Object new_questrewardInfo = new QuestReward_Object();

            new_questrewardInfo.id = int.Parse(ss[i.ToString(), "id"].value);
            new_questrewardInfo.name = ss[i.ToString(), "name"].value;
            new_questrewardInfo.rewardType = ss[i.ToString(), "rewardType"].value;
           

            datalist.Add(new_questrewardInfo);
        }
        Debug.Log("questreward 데이터 테이블 업데이트 완료");

        DataSaveText(datalist); //업데이트한 데이터 저장
    }


    void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), callback, mergedCells);
    }

    void UpdateMethod(GstuSpreadSheet ss)
    {
        UpdateStats(ss);
    }

    public void DataSaveText(List<QuestReward_Object> _datalist)
    {
        //데이터를 Json으로 저장하는 함수
        string savePath = getPath(fileName);    //저장 파일 경로

        string saveJson = "";
        for (int i = 0; i < _datalist.Count; i++)
        {
            string saveText = JsonUtility.ToJson(_datalist[i], true) + ", \n";
            saveJson += saveText;
        }

        File.WriteAllText(savePath, saveJson);
    }

    public void DataLoadText()
    {
        //Json 데이터를 불러오는 함수
        datalist.Clear();

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

                        datalist.Add(JsonUtility.FromJson<QuestReward_Object>(oneItem));
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
        return Application.persistentDataPath+ fileName + ".json";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+ fileName + ".json";
#else
        return Application.dataPath +"/"+ fileName + ".json";
#endif
    }
}
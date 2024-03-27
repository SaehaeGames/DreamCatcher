using UnityEngine;
using System.Collections;
using GoogleSheetsToUnity;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using GoogleSheetsToUnity.ThirdPary;
using UnityEditor;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/QuestInfo")]
[Serializable]
public class QuestInfo_Data : ScriptableObject
{
    [HideInInspector]
    public string associatedSheet = "1W0ML_fkdwZLb9MR4dElftbOhuofJutOvdotkpdRijQQ";
    [HideInInspector]
    public string associatedWorksheet = "questinfo";
    private static string fileName = "QuestInfoFile";

    public int startid = 6001;
    public int endid = 6003;

    public List<QuestInfo_Object> datalist = new List<QuestInfo_Object>();

    public void LoadQuestInfoData()
    {
        //데이터를 로드하는 함수

        string savePath = getPath(fileName);    //저장 파일 경로

        if (!File.Exists(savePath))  //파일이 존재하지 않는다면
        {
            UpdateQuestInfoData();  //데이터 테이블 가져오기

            Debug.Log("존재하지 않아서 생성 및 저장");
            //DataLoadText(); //파일 로드
        }
        else    //파일이 존재한다면
        {
            DataLoadText(); //파일 로드

/*            QuestInfo_Data questinfo_data = CreateInstance<QuestInfo_Data>();
            questinfo_data.datalist = this.datalist;
            AssetDatabase.CreateAsset(questinfo_data, "Assets/Scripts/DataTable/ScritableObjects/QuestInfo.asset");
            AssetDatabase.SaveAssets();
            Debug.Log(AssetDatabase.GetAllAssetPaths());*/

        }
    }

    public void UpdateQuestInfoData()
    {
        //questinfo 업데이트 할 때 호출하는 함수

        UpdateStats(UpdateMethod);  //데이터 업데이트
    }

    internal void UpdateStats(GstuSpreadSheet ss)
    {
        datalist.Clear();

        for (int i = startid; i <= endid; i++)
        {
            QuestInfo_Object new_questinfo = new QuestInfo_Object();

            new_questinfo.id = int.Parse(ss[i.ToString(), "id"].value);
            new_questinfo.title = ss[i.ToString(), "title"].value;
            new_questinfo.contents = ss[i.ToString(), "contents"].value;
            new_questinfo.from = ss[i.ToString(), "from"].value;

            datalist.Add(new_questinfo);
        }
        Debug.Log("questinfo 데이터 테이블 업데이트 완료");

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

    public void DataSaveText(List<QuestInfo_Object> _datalist)
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

                        datalist.Add(JsonUtility.FromJson<QuestInfo_Object>(oneItem));
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

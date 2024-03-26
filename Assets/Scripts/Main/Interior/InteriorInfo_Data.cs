using UnityEngine;
using System.Collections;
using GoogleSheetsToUnity;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using GoogleSheetsToUnity.ThirdPary;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/InteriorInfo")]
public class InteriorInfo_Data : ScriptableObject
{
    [HideInInspector]
    public string associatedSheet = "1F3nQXJxatyzvJHbba9H0B832GQ6aQh0cigMeJJxSLx4";
    [HideInInspector]
    public string associatedWorksheet = "interiorinfo";
    private static string fileName = "InteriorInfoFile";

    public int startid = 4001;
    public int endid = 4027;

    public List<InteriorInfo_Object> datalist = new List<InteriorInfo_Object>();

    public void LoadInteriorInfoData()
    {
        //데이터를 로드하는 함수

        string savePath = getPath(fileName);    //저장 파일 경로

        if (!File.Exists(savePath))  //파일이 존재하지 않는다면
        {
            UpdateInteriorInfoData();  //데이터 테이블 가져오기
            Debug.Log("존재하지 않아서 생성 및 저장");
            DataLoadText(); //파일 로드
        }
        else    //파일이 존재한다면
        {
            DataLoadText(); //파일 로드
        }
    }

    public void UpdateInteriorInfoData()
    {
        //storeInfo 업데이트 할 때 호출하는 함수
        UpdateStats(UpdateMethod);  //데이터 업데이트
    }

    internal void UpdateStats(GstuSpreadSheet ss)
    {
        datalist.Clear();

        for (int i = startid; i <= endid; i++)
        {
            InteriorInfo_Object new_interiorinfo = new InteriorInfo_Object();

            new_interiorinfo.id = int.Parse(ss[i.ToString(), "id"].value);
            new_interiorinfo.name = ss[i.ToString(), "name"].value;
            new_interiorinfo.category_1 = ss[i.ToString(), "category_1"].value;
            new_interiorinfo.category_2 = ss[i.ToString(), "category_2"].value;
            new_interiorinfo.gold = int.Parse(ss[i.ToString(), "gold"].value);

            datalist.Add(new_interiorinfo);
        }
        Debug.Log("InteriorInfo 데이터 테이블 업데이트 완료");

        DataSaveText(); //업데이트한 데이터 저장
    }


    void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), callback, mergedCells);
    }

    void UpdateMethod(GstuSpreadSheet ss)
    {
        UpdateStats(ss);
    }

    public void DataSaveText()
    {
        //데이터를 Json으로 저장하는 함수
        string savePath = getPath(fileName);    //저장 파일 경로

        string saveJson = "";
        for (int i = 0; i < datalist.Count; i++)
        {
            string saveText = JsonUtility.ToJson(datalist[i], true) + ", \n";
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

                        datalist.Add(JsonUtility.FromJson<InteriorInfo_Object>(oneItem));
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

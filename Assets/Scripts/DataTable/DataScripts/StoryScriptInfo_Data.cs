using GoogleSheetsToUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

//[CreateAssetMenu(fileName="DataTable", menuName="Scriptable Object Asset/StoryScriptInfo")]
[Serializable]
public class StoryScriptInfo_Data : ScriptableObject
{
    [HideInInspector]
    public string associatedSheet = "10-I4KgO4n3KZMx-NcmLVCEmTT_W6Uj9I_-ynXLTNolg";
    [HideInInspector]
    public string associatedWorksheet = "storyscriptinfo";
    private static string fileName = "StoryScriptInfoFile";

    public int startid = 7000;
    public int endid = 7230;

    public List<StoryScriptInfo_Object> datalist = new List<StoryScriptInfo_Object>();

    public void LoadStoryScriptInfoData()
    {
        string savePath = getPath(fileName);

        if (!File.Exists(savePath)) // 파일이 존재하지 않는다면
        {
            UpdateStoryScriptInfoData(); // 데이터 테이블 가져오기
            Debug.Log("StoryScriptInfo_Data : 존재하지 않아서 생성 및 저장");
            //DataLoadText(); //파일 로드
        }
        else // 파일이 존재 한다면
        {
            DataLoadText(); // 파일 로드
        }
    }

    public void UpdateStoryScriptInfoData()
    {
        UpdateStats(UpdateMethod);
    }

    internal void UpdateStats(GstuSpreadSheet ss)
    {
        datalist.Clear();

        for (int i = startid; i <= endid; i++)
        {
            StoryScriptInfo_Object new_storyscriptinfo = new StoryScriptInfo_Object();

            new_storyscriptinfo.id = int.Parse(ss[i.ToString(), "id"].value);
            new_storyscriptinfo.sceneNum = int.Parse(ss[i.ToString(), "sceneNum"].value);
            new_storyscriptinfo.questNum = int.Parse(ss[i.ToString(), "questNum"].value);
            new_storyscriptinfo.charImage = int.Parse(ss[i.ToString(), "charImage"].value);
            new_storyscriptinfo.faceImage = int.Parse(ss[i.ToString(), "faceImage"].value);
            new_storyscriptinfo.effectImage = int.Parse(ss[i.ToString(), "effectImage"].value);
            new_storyscriptinfo.screenEffect = int.Parse(ss[i.ToString(), "screenEffect"].value);
            new_storyscriptinfo.speaker = ss[i.ToString(), "speaker"].value;
            new_storyscriptinfo.line = ss[i.ToString(), "line"].value;

            datalist.Add(new_storyscriptinfo);
        }
        Debug.Log("storyscriptinfo 데이터 테이블 업데이트 완료");

        DataSaveText(datalist); // 업데이트한 데이터 저장
    }

    void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), callback, mergedCells);
    }

    void UpdateMethod(GstuSpreadSheet ss)
    {
        Debug.Log("storyscriptinfo 데이터 테이블 업데이트 함수");
        UpdateStats(ss);
    }

    public void DataSaveText(List<StoryScriptInfo_Object> _datalist)
    {
        string savePath = getPath(fileName);
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
        datalist.Clear();

        string savePath = getPath(fileName);
        string[] loadJson = File.ReadAllLines(savePath);

        for (int i = 0; i < loadJson.Length; i++)
        {
            string curText1 = loadJson[i];
            if (curText1.Equals("{"))
            {
                string oneItem = "{";

                for (int j = i + 1; i < loadJson.Length; j++)
                {
                    string curText2 = loadJson[j];
                    if (curText2.Equals("}, "))
                    {
                        oneItem += "}";
                        datalist.Add(JsonUtility.FromJson<StoryScriptInfo_Object>(oneItem));
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

using GoogleSheetsToUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="DataTable", menuName ="Scriptable Object Asset/StorySceneInfo")]

[Serializable]
public class StorySceneInfo_Data : ScriptableObject
{
    [HideInInspector]
    public string associateSheet = "14moYHgh4H_et4Dj0CY6UtzB7eaOoR_C7Oi4GL5qeZqE";
    [HideInInspector]
    public string associatedWorksheet = "storysceneinfo";
    private static string fileName = "StorySceneInfoFile";

    public int startid = 8000;
    public int endid = 8024;

    public List<StorySceneInfo_Object> datalist = new List<StorySceneInfo_Object>();

    public void LoadStorySceneInfoData()
    {
        string savePath = getPath(fileName);

        if(!File.Exists(savePath))
        {
            UpdateStorySceneInfoData();
            Debug.Log("존재하지 않아서 생성 및 저장(StorySceneInfo)");
        }
        else
        {
            DataLoadText();
        }
    }

    public void UpdateStorySceneInfoData()
    {
        UpdateStats(UpdateMethod);
    }

    internal void UpdateStats(GstuSpreadSheet ss)
    {
        datalist.Clear();
        for(int i=startid; i<=endid; i++)
        {
            StorySceneInfo_Object new_storysceneinfo=new StorySceneInfo_Object();

            new_storysceneinfo.id = int.Parse(ss[i.ToString(), "id"].value);
            new_storysceneinfo.sceneNum = int.Parse(ss[i.ToString(), "sceneNum"].value);
            new_storysceneinfo.startId = int.Parse(ss[i.ToString(), "startId"].value);
            new_storysceneinfo.endId = int.Parse(ss[i.ToString(), "endId"].value);

            datalist.Add(new_storysceneinfo);
        }
        DataSaveText();
    }

    void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells=false)
    {
        SpreadsheetManager.Read(new GSTU_Search(associateSheet, associatedWorksheet), callback, mergedCells);
    }

    void UpdateMethod(GstuSpreadSheet ss)
    {
        UpdateStats(ss);
    }

    public void DataSaveText()
    {
        string savePath = getPath(fileName);
        string saveJson = "";
        for(int i=0; i<datalist.Count; i++)
        {
            string saveText = JsonUtility.ToJson(datalist[i], true) + ", \n";
            saveJson += saveText;
        }
        File.WriteAllText(savePath, saveJson);
    }

    public void DataLoadText()
    {
        datalist.Clear();

        string savePath = getPath(fileName);
        string[] loadJson = File.ReadAllLines(savePath);

        for(int i=0; i<loadJson.Length; i++)
        {
            string curText1 = loadJson[i];
            if(curText1.Equals("{"))
            {
                string oneItem = "{";
                for(int j=i+1; j<loadJson.Length; j++)
                {
                    string curText2 = loadJson[j];
                    if(curText2.Equals("}, "))
                    {
                        oneItem += "}";
                        datalist.Add(JsonUtility.FromJson<StorySceneInfo_Object>(oneItem));
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
        return Application.persistentDataPath + "/Saves/" + fileName + ".json";
#elif UNITY_ANDROID
        return Application.persistentDataPath + "/Saves/" + fileName + ".json";
#elif UNITY_IPHONE
        return Application.persistentDataPath + "/Saves/" + fileName + ".json";
#else
        return Application.persistentDataPath + "/Saves/" + fileName + ".json";
#endif
    }
}

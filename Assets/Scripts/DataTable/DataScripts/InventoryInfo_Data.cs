using UnityEngine;
using System.Collections;
using GoogleSheetsToUnity;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using GoogleSheetsToUnity.ThirdPary;
using System.IO;
using UnityEditor;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/InventoryInfo")]
public class InventoryInfo_Data : ScriptableObject
{
    [HideInInspector]
    public string associatedSheet = "1gb92n5lB5w_-IDIYROvlRSifqM6RLe5MLckegBVtpr8";
    [HideInInspector]
    public string associatedWorksheet1 = "featherinfo";

    private static string fileName = "InventoryInfoFile";

    public int startid_feather = 3001;
    public int endid_feather = 3016;

    public List<InventoryInfo_feather_Object> datalist = new List<InventoryInfo_feather_Object>();

    public void LoadInventoryInfoData()
    {
        //데이터를 로드하는 함수

        string savePath = getPath(fileName);    //저장 파일 경로

        if (!File.Exists(savePath))  //파일이 존재하지 않는다면
        {
            UpdateInventoryInfoData();  //데이터 테이블 가져오기

            Debug.Log("존재하지 않아서 생성 및 저장");
        }
        else    //파일이 존재한다면
        {
            DataLoadText(); //파일 로드

/*            InventoryInfo_Data inventoryinfo_data = CreateInstance<InventoryInfo_Data>();
            inventoryinfo_data.datalist = this.datalist;
            AssetDatabase.CreateAsset(inventoryinfo_data, "Assets/Scripts/DataTable/ScritableObjects/InventoryInfo.asset");
            AssetDatabase.SaveAssets();
            Debug.Log(AssetDatabase.GetAllAssetPaths());*/

        }
    }

    public void UpdateInventoryInfoData()
    {
        //inventoryinfo 업데이트 할 때 호출하는 함수

        UpdateStats(UpdateMethod);  //데이터 업데이트
    }

    internal void UpdateStats(GstuSpreadSheet ss)
    {
        datalist.Clear();
        //datalist2.Clear();

        for (int i = startid_feather; i <= endid_feather; i++)
        {
            InventoryInfo_feather_Object new_feather = new InventoryInfo_feather_Object();

            new_feather.id = int.Parse(ss[i.ToString(), "id"].value);
            new_feather.name = ss[i.ToString(), "name"].value;
            new_feather.contents = ss[i.ToString(), "contents"].value;
            new_feather.appear = int.Parse(ss[i.ToString(), "appear"].value);
            new_feather.number = int.Parse(ss[i.ToString(), "number"].value);

            datalist.Add(new_feather);
        }
        Debug.Log("inventory_feather 데이터 테이블 업데이트 완료");

        DataSaveText(); //업데이트한 데이터 저장
    }


    void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet1), callback, mergedCells);
    }

    void UpdateMethod(GstuSpreadSheet ss)
    {
        UpdateStats(ss);
    }

    public void DataSaveText()
    {
        //데이터를 Json으로 저장하는 함수
        string savePath = getPath(fileName);    //저장 파일 경로
        //string savePath = Application.persistentDataPath + fileName + ".json";

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

                        datalist.Add(JsonUtility.FromJson<InventoryInfo_feather_Object>(oneItem));
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class QuestJSON : MonoBehaviour
{
    private static string fileName = "QuestDataFile";       //파일 이름
    QuestContainer dataContainer;  //상품 데이터 변수

    public void LoadQuestData()
    {
        //데이터를 로드하는 함수

        string savePath = getPath(fileName);    //저장 파일 경로

        if (!File.Exists(savePath))  //파일이 존재하지 않는다면
        {
            this.dataContainer = new QuestContainer();  //객체 생성
        }
        else    //파일이 존재한다면
        {
            //this.dataContainer = DataLoadText<QuestContainer>(); //파일 로드
            DataLoadText();
        }
    }

    public QuestContainer GetQuestData()
    {
        //데이터를 반환하는 함수

        LoadQuestData();
        return dataContainer;
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

    public void DataLoadText()
    {
        //Json 데이터를 불러오는 함수

        Debug.Log("퀘스트 데이터 로드 시작");
        try
        {
            // 기존 파싱 코드
            /*string savePath = getPath(fileName);    //저장 파일 경로
            string loadJson = File.ReadAllText(savePath);

            T t = JsonUtility.FromJson<T>(loadJson);
            return t;*/


            // 드림캐쳐 객체 따로 파싱하는 코드
            string savePath = getPath(fileName);    //저장 파일 경로
            string[] loadJson = File.ReadAllLines(savePath);    //Text말고 Line으로 읽어옴(한줄한줄 직접 파싱)
            dataContainer = new QuestContainer();   // 퀘스트 리스트를 가지는 객체 생성
            //DreamCatcher curDC = new DreamCatcher(0, null, null, 1, 2, 3, 4);

            for (int i = 2; i < loadJson.Length; i++)   // 퀘스트 데이터 객체부터 파싱 시작
            {
                string curText1 = loadJson[i].Trim();  //현재 라인 공백 제거
                string oneDC = "";  // 퀘스트 데이터 내부의 드림캐쳐 데이터를 저장할 string 객체

                if (curText1.Equals("{"))   //하나의 퀘스트 데이터 내부
                {
                    string oneItem = "{";   // 시작 중괄호 추가
                    Debug.Log("객체 하나 시작 : " + oneItem);

                    for (int j = i + 1; j < loadJson.Length; j++) 
                    {
                        string curText2 = loadJson[j].Trim();   // 현재의 라인 공백 제거
                        if (curText2.Contains("{"))     //드림캐쳐 데이터 내부
                        {
                            // 드림캐쳐 데이터라면
                            for (int k = j + 1; k < loadJson.Length; k++)
                            {
                                string curText3 = loadJson[k].Trim();   // 현재의 라인 공백 제거

                                if (curText3.Equals("}"))   // 드림캐쳐 데이터가 끝났다면
                                {
                                    oneDC += "}";   // 닫는 중괄호 추가
                                    j = k;  // 탐색(?)한 라인 반영
                                    Debug.Log("드림캐쳐 데이터 끝 : " + oneDC); 
                                    break;  // 하나의 드림캐쳐 데이터를 가져오는 반복문 종료
                                }
                                else    // 드림캐쳐 데이터가 끝나지 않았다면
                                {
                                    oneDC += curText3;  // 드림캐쳐 데이터를 저장하는 string 객체에 현재 라인에 있는 string 값 추가 (실 색깔, 깃털 번호 등)
                                }
                            }
                        }

                        if (curText2.Contains("}"))     //퀘스트 데이터 하나가 끝났다면
                        {
                            oneItem += oneDC + "}";     //퀘스트 데이터를 저장하는 string 객체 마지막에 드림캐쳐 데이터를 추가하고, 닫는 중괄호 추가
                            Debug.Log("객체 하나 끝 : " + oneItem);

                            dataContainer.questList.Add(JsonUtility.FromJson<QuestData>(oneItem));  // 퀘스트 데이터 string을 QuestData형으로 변경하여 퀘스트 리스트에 추가
                            oneItem = "";   //퀘스트 데이터값 초기화
                            break;  // 하나의 퀘스트 데이터를 가져오는 반복문 종료
                        }
                        else    // 하나의 퀘스트 데이터가 끝나지 않았다면
                        {
                            oneItem += curText2;    // 퀘스트 데이터를 저장하는 string 객체에 현재 라인에 있는 string 값 추가(id, 클리어 여부 등)
                        }
                    }
                }
            }

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

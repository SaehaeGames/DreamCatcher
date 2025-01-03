using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DreamCatcherInfoLoad : MonoBehaviour
{
    public MyDreamCatcher dreamCatcherData; //MyDreamCatcher 객체 필요
    private static DreamCatcherInfoLoad _instance;
    public static DreamCatcherInfoLoad Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<DreamCatcherInfoLoad>();

            return _instance;
        }
    }

    // 드림캐쳐 이미지 불러오기(dcId/Index를 이용)
    // : dcId/Index를 이용하여 드림캐쳐의 이미지 파일에서 불러와 Sprite로 반환한다.
    public Sprite ImageLoad(int index)
    {
        string fileName;

        if (index>=1000) // 아이디를 입력받았을 때
        {
            // 이미지 파일 이름
            fileName = index + ".png"; // 파일이름(Id)
        }
        else // 인덱스를 입력받았을 때
        {
            // Json파일 로드
            dreamCatcherData = GameManager.instance.dreamCatcherDataManager; //MyDreamCatcher 객체 GameManager에서 가져옴
            GameManager.instance.GetComponent<DreamCatcherDataManager>().DataLoadText<MyDreamCatcher>();
            DreamCatcher selectDreamCatcher = dreamCatcherData.dreamCatcherList[index];

            // 이미지 파일 이름
            fileName = selectDreamCatcher.GetId() + ".png"; // 파일이름(Id)
        }

        // 이미지 위치
        string filePath = getPath(fileName);

        // 이미지 스프라이트화
        Sprite mySprite = IMG2Sprite.instance.LoadNewSprite(filePath);

        return mySprite;
    }

    // 드림 캐쳐 삭제
    // 데이터 삭제
    // : 저장되어 있는 json 데이터를 삭제합니다.
    public void DataDelete(int index)
    {
        // 아이디 json 삭제
        // Json파일 로드
        dreamCatcherData = GameManager.instance.dreamCatcherDataManager; //MyDreamCatcher 객체 GameManager에서 가져옴
        GameManager.instance.GetComponent<DreamCatcherDataManager>().DataLoadText<MyDreamCatcher>();

        if(index>=1000)
        {
            for (int i = 0; i < dreamCatcherData.nDreamCatcher; i++)
            {
                int id = dreamCatcherData.dreamCatcherList[i].GetId();
                if (id == index)
                {
                    dreamCatcherData.dreamCatcherList.RemoveAt(i); // 삭제
                }
            }
        }
        else
        {
            dreamCatcherData.dreamCatcherList.RemoveAt(index); // 삭제
        }

        // 드림캐쳐 개수 업데이트
        dreamCatcherData.nDreamCatcher = dreamCatcherData.nDreamCatcher - 1;
        // 드림캐쳐 json 세이브
        GameManager.instance.GetComponent<DreamCatcherDataManager>().DataSaveText(dreamCatcherData); // json 세이브
    }

    // 이미지 삭제
    // : 드림캐쳐 Id에 해당하는 이미지를 삭제합니다.
    public void ImageDelete(int dcId) 
    {
        // 아이디 이미지 삭제
        string fileName = dcId + ".png"; // 파일이름(Id)
        string filePath = getPath(fileName);
        System.IO.File.Delete(filePath);
    }

    // 경로
    private static string getPath(string fileName)
    {
#if UNITY_EDITOR
        return Application.dataPath + "/DreamCatcherImgs/" + fileName;
#elif UNITY_ANDROID
        return Application.persistentDataPath + "/DreamCatcherImgs/" + fileName;
#elif UNITY_IPHONE
        return Application.persistentDataPath + "/DreamCatcherImgs/" + fileName;
#else
        return Application.dataPath + "/DreamCatcherImgs/" + fileName;
#endif
    }
}

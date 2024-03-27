using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class ResetGame : MonoBehaviour
{
    //게임을 초기화하는 스크립트

    private PlayerDataContainer curPlayerData;   //플레이어 데이터 정보
    private GoodsContainer curGoodsData;   //상품 정보
    private BirdContainer curBirdData;  //먹이둔 새 데이터 정보
    private MyFeatherNumber featherData;
    //List<MyFeatherNumber> featherData;    //인벤토리 깃털 정보
    //private List<InventoryDreamCatcherData> curInventoryDreamCatherData;    //인벤토리 드림캐쳐 데이터 정보

    public void ResettingGame()
    {
        //리셋 버튼 이벤트

        //1. 플레이어 데이터 리셋(꿈구슬, 특제먹이, 골드, 볼륨)
        curPlayerData = new PlayerDataContainer();
        GameManager.instance.loadPlayerData = curPlayerData;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(curPlayerData);   //변경사항 json으로 저장

        //2. 상점 구매 데이터 리셋
        curGoodsData = new GoodsContainer();
        GameManager.instance.loadGoodsData = curGoodsData;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GoodsJSON>().DataSaveText(curGoodsData);   //변경사항 json으로 저장

        //3. 인벤토리 데이터 리셋
        //1) 깃털 개수 리셋
        featherData = new MyFeatherNumber();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<FeatherNumDataManager>().DataSaveText(featherData);
        //GameManager.instance.GetComponent<FeatherNumDataManager>().ResetFeatherJson();  //깃털 개수 리셋
        //GameManager.instance.GetComponent<FeatherNumDataManager>().SaveFeatherJson();   //리셋한 데이터 저장

        //2) 인벤토리 드림캐쳐 개수 리셋
        //GameManager의 loadInventoryDreamCatcherData를 초기화
        System.IO.File.Delete(getPath("Saves", "DreamCatcherListData.json"));
        //Directory.Delete(getPath("DreamCatcherImgs", ""), true);

        //5. 새 저장 데이터 리셋(먹이둔 새 데이터)
        curBirdData = new BirdContainer();
        GameManager.instance.loadBirdData = curBirdData;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<BirdJSON>().DataSaveText(curBirdData);   //변경사항 json으로 저장

        //6. 리셋한 데이터 GameManager에 반영
        GameManager.instance.ResetGameManager();

        //7. 시작 화면으로 이동
        GameObject.FindGameObjectWithTag("BottomBar").GetComponent<SceneChange>().ChangeStartScene();

        //나중에 수정할 것(급하게 한다고..)
        PlayerPrefs.SetInt("RepearFirstLetter", System.Convert.ToInt16(false)); //상단주의 편지 안읽음 처리
        //GameManager.instance.resetTempData();
    }

    private static string getPath(string directory, string fileName)
    {
#if UNITY_EDITOR
        return Application.dataPath + "/"+ directory + "/" + fileName;
#elif UNITY_ANDROID
        return Application.persistentDataPath + "/"+ directory + "/" + fileName;
#elif UNITY_IPHONE
        return Application.persistentDataPath + "/"+ directory + "/" + fileName;
#else
        return Application.dataPath +"/"+ fileName;
#endif
    }

}

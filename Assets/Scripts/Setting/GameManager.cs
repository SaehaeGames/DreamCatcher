using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // 게임 데이터를 관리하는 싱글톤 패턴

    //싱글톤 패턴을 사용하기 위한 전역 변수
    // 테스트
    public static GameManager instance;

    [Header("[Game Data]")]
    public GoodsContainer loadGoodsData;  //상품(보조도구) 레벨 데이터
    public PlayerDataContainer loadPlayerData;  //플레이어 데이터(꿈구슬, 골드, 특제먹이) 개수, 음향 데이터 (여기에 보조도구 레벨 데이터 넣어도 될 듯)
    public BirdContainer loadBirdData;  //먹이둔 새 데이터
    public MyFeatherNumber loadFeatherData;
    public MyDreamCatcher loadDreamCatcherData; //드림캐쳐 저장 데이터
    public InteriorContainer loadInteriorData;  //플레이어 인테리어 저장 데이터
    public QuestContainer loadQuestData;
    public Text testText;

    //데이터 테이블 오브젝트
    public BirdInfo_Data birdinfo_data;
    public DreamInfo_Data dreaminfo_data;
    public StoreInfo_Data storeinfo_data;
    public InventoryInfo_Data inventoryinfo_data;
    public InteriorInfo_Data interiorinfo_data;
    public QuestInfo_Data questinfo_data;
    public StoryScriptInfo_Data storyscriptinfo_data;
    public StorySceneInfo_Data storysceneinfo_data;

    void Awake()
    {
        // 게임 시작과 동시에 싱글톤 구성

        if (instance)     //싱글톤 변수 instance가 이미 있다면
        {
            DestroyImmediate(gameObject);   //삭제
            return;
        }

        instance = this;    //유일한 인스턴스
        DontDestroyOnLoad(gameObject);  //씬이 바뀌어도 계속 유지시킴
    }

    private void Start()
    {
        //UpdateGameDataFromSpreadSheet();

        ResetGameManager();
        LoadGameDataFromJson();
        //LoadGameDataFromJson();
        try
        {
            testText.text = "테스트용 : " + loadQuestData.questList[2].questDreamCatcher.GetFeather(2);
        }
        catch (Exception e)
        {
            testText.text = e.ToString();
        }
    }

    public static GameManager GetGameManager()
    {
        return instance;
    }

    public void LoadGameDataFromJson()
    {
        //데이터 테이블 불러오는 함수

        birdinfo_data.LoadBirdInfoData();
        dreaminfo_data.LoadDreamInfoData();
        storeinfo_data.LoadStoreInfoData();
        inventoryinfo_data.LoadInventoryInfoData();
        interiorinfo_data.LoadInteriorInfoData();
        questinfo_data.LoadQuestInfoData();
        storyscriptinfo_data.LoadStoryScriptInfoData();
        storysceneinfo_data.LoadStorySceneInfoData();
    }

    public void UpdateGameDataFromSpreadSheet()
    {
        //스프레드 시트로부터 게임 데이터를 불러오는 함수
        //데이터 테이블에 변경사항 있을 때 딱 한 번 호출하기!

        birdinfo_data.UpdateBirdInfoData();
        dreaminfo_data.UpdateDreamInfoData();
        storeinfo_data.UpdateStoreInfoData();
        inventoryinfo_data.UpdateInventoryInfoData();
        interiorinfo_data.UpdateInteriorInfoData();
        questinfo_data.UpdateQuestInfoData();
        storyscriptinfo_data.UpdateStoryScriptInfoData();
        storysceneinfo_data.UpdateStorySceneInfoData();
    }


    public void ResetGameManager()
    {
        //초기화 함수

        loadGoodsData = this.gameObject.GetComponent<GoodsJSON>().GetGoodsData();   //상품 데이터 가져오기
        loadPlayerData = this.gameObject.GetComponent<PlayerDataJSON>().GetTopBarData();    //상단바 데이터 가져오기
        loadBirdData = this.gameObject.GetComponent<BirdJSON>().GetBirdData();   //먹이둔 새 데이터 가져오기
        loadFeatherData = this.gameObject.GetComponent<FeatherNumDataManager>().GetFeatherData();
        loadDreamCatcherData = this.gameObject.GetComponent<DreamCatcherDataManager>().GetDreamCatcherData();
        loadInteriorData = this.gameObject.GetComponent<InteriorJSON>().GetInteriorData();  //인테리어 저장 데이터 가져오기
        loadQuestData = this.gameObject.GetComponent<QuestJSON>().GetQuestData();
    }
}
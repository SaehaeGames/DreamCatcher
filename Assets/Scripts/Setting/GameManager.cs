using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 게임 데이터를 관리하는 싱글톤 패턴

    //싱글톤 패턴을 사용하기 위한 전역 변수
    public static GameManager instance;

    [Header("[Game Data]")]
    public JsonManager jsonManager;                 // JsonManager : 세이브&로드 클래스
    public PlayerDataManager playerDataManager = new PlayerDataManager();     //플레이어 데이터(꿈구슬, 골드, 특제먹이) 개수, 음향 데이터
    public List<RackData> rackDataList;             // 횃대에 놓인 먹이 데이터   ** 이건 횃대 레벨업 하면 리스트에 add해서 저장하도록 하기
    public RackDataManager RackDataManager { get; private set; } = new RackDataManager();
    public GoodsDataManager goodsDataManager;       // 상점 아이템 레벨 데이터
    public InteriorDataManager interiorDataManager; //플레이어 인테리어 저장 데이터
    public QuestDataManager questDataManager = new QuestDataManager();       // 퀘스트 데이터 리스트
    public FeatherDataManager featherDataManager = new FeatherDataManager();
    public DreamCatcherInventoryDataManager dreamCatcherInventoryDataManager = new DreamCatcherInventoryDataManager();
    public DreamCatcherDataManager dreamCatcherDataManager;

    //데이터 베이스 오브젝트 (스크립터블 오브젝트 객체)
    [Space]
    public BirdInfo_Data birdinfo_data;
    public DreamInfo_Data dreaminfo_data;
    public StoreInfo_Data storeinfo_data;
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

        dreamCatcherDataManager = new DreamCatcherDataManager(dreamCatcherInventoryDataManager);

        ResetGameManager();
    }

    public static GameManager GetGameManager()
    {
        return instance;
    }  

    public void ResetGameManager()
    {
        //초기화 함수

        jsonManager = new JsonManager();    // JSON 저장 매니저 객체 생성

        // 각 저장 데이터 가져오기
        RackDataManager.Load();
        rackDataList = RackDataManager.dataList;
        goodsDataManager = new GoodsDataManager();
        goodsDataManager.Load();
        interiorDataManager = new InteriorDataManager();
        interiorDataManager.Load();
        questDataManager.Load();
        playerDataManager.Load();
        //playerDataManager.ResetData();
        featherDataManager.Load();
        //featherDataManager.ResetData();
        dreamCatcherDataManager.Load();
        dreamCatcherInventoryDataManager.Load();
    }
}

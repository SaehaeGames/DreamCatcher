using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 게임 데이터를 관리하는 싱글톤 패턴

    //싱글톤 패턴을 사용하기 위한 전역 변수
    public static GameManager instance;
    public bool IsInitialized { get; private set; }

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
    public QuestInfo_Data questinfo_data;
    public StoryScriptInfo_Data storyscriptinfo_data;
    public StorySceneInfo_Data storysceneinfo_data;
    public CharonInfo_Data charoninfo_data;
    public DiaryInfo_Data diaryinfo_data;


    void Awake()
    {
        // 게임 시작과 동시에 싱글톤 구성

        if (instance)     //싱글톤 변수 instance가 이미 있다면
        {
            Destroy(gameObject);
            return;
        }

        instance = this;    //유일한 인스턴스
        DontDestroyOnLoad(gameObject);  //씬이 바뀌어도 계속 유지시킴

        dreamCatcherDataManager = new DreamCatcherDataManager(dreamCatcherInventoryDataManager);
        // 모든 화면의 Start보다 먼저 데이터 로드를 완료합니다.
        ResetGameManager();
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public static GameManager GetGameManager()
    {
        return instance;
    }  


    public void ResetGameManager()
    {
        //초기화 함수

        IsInitialized = false;
        if (jsonManager == null) jsonManager = new JsonManager();

        // 각 저장 데이터 가져오기
        RackDataManager.Load();
        rackDataList = RackDataManager.dataList;
        if (goodsDataManager == null) goodsDataManager = new GoodsDataManager();
        goodsDataManager.Load();
        if (interiorDataManager == null) interiorDataManager = new InteriorDataManager();
        interiorDataManager.Load();
        questDataManager.Load();
        playerDataManager.Load();
        //playerDataManager.ResetData();
        featherDataManager.Load();
        //featherDataManager.ResetData();
        dreamCatcherDataManager.Load();
        dreamCatcherInventoryDataManager.Load();
        // 인테리어는 모든 화면에서 같은 보유·배치 상태를 사용하도록 데이터 로드 뒤에 준비합니다.
        // 이전 세이브에 추가 상품이 없거나 배치 상태가 잘못된 경우 여기서 한 번만 보정·저장합니다.
        if (!interiorDataManager.TryPrepareData(storeinfo_data, goodsDataManager, out string interiorError))
        {
            Debug.LogError("[GameManager] 인테리어 데이터 준비 실패: " + interiorError, this);
            return;
        }
        IsInitialized = true;
    }
}

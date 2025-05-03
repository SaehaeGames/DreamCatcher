using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //인벤토리 데이터를 가져오는 클래스

    [Header("[Inventory Product]")]
    public GameObject ItemPrefab;   // 아이템 프리팹
    public GameObject ItemContent;  //아이템 오브젝트를 넣을 부모 오브젝트
    public Sprite[] ItemImages; //아이템 이미지 배열
    public GameObject dreamCatcherPrefab;   //드림캐쳐 프리팹

    [Space]
    [Header("[Inventory Data]")]
    public int itemMaxCnt;  //인벤토리 최대 용량
    public int itemCurCnt;  //인벤토리 현재 아이템 수
    public List<GameObject> itemList;   //인벤토리 아이템 리스트    
    MyFeatherNumber featherData;    //플레이어 깃털 정보

    public void SettingInventory()
    {
        //인벤토리 열 때마다 세팅하는 함수

        AddInventoryList();
        UpdateInventory();
        //드림캐쳐 가져오는 함수
    }

    public void AddInventoryList()
    {
        //상자가 업그레이드되면 용량만큼 인벤토리 리스트 요소를 추가하는 함수

        //itemMaxCnt = CheckItemMaximum();    //인벤토리 용량 갱신
        itemMaxCnt = 4; //인벤토리 용량 4개로 고정
        int curCnt = itemList.Count;
        int addCnt = itemMaxCnt - curCnt;   //추가해야할 요소 수

        Debug.Log("현재 curCnt : " + curCnt + " 추가할 양 : " + addCnt);

        SetItemObject(ItemPrefab, ItemContent, addCnt);  //인벤토리 요소 생성(상자 용량만큼)
    }

    public int CheckItemMaximum()
    {
        //인벤토리 최대 용량을 반환하는 함수

        int PocketLevel = GameManager.instance.goodsDataManager.GetValidatedGoodsData(Constants.GoodsData_Box).level;  //주머니의 레벨을 가져옴

        StoreInfo_Data _storeinfo_data = GameManager.instance.storeinfo_data;  
        int boxEffect = int.Parse(_storeinfo_data.dataList[PocketLevel + 8].effect);    //상자 효과를 가져옴(12는 상점 데이터의 박스 시작 인덱스)
        //위 이거 말고 그냥 플레이어 데이터 저장할 때 상품 레벨(0,1,2,3)이 아니라 아이템 id로 저장하기.. 그래서 아이템 id 바로 가져와서 그걸로 조회하기?


        int maxCnt = boxEffect;

        return maxCnt;
    }

    public int GetInventoryItemCnt()
    {
        //현재 인벤토리의 요소 개수를 반환하는 함수

        MyFeatherNumber featherData = GameManager.instance.featherDataManager;
        int itemCurCnt = 0;
        int featherCnt = featherData.birdCnt;
        Debug.Log("featherData.Count : " + featherCnt);
        for (int i = 0; i < featherCnt; i++)     //깃털 개수만큼 반복(**나중에 드림캐쳐 개수도 추가해야함)
        {
            if (featherData.featherList[i].feather_number > 0)
            {
                itemCurCnt++;   //인벤토리 요소 카운트 증가
            }
        }

        //드림캐쳐 개수도 추가?

        return itemCurCnt;
    }

    public void SetItemObject(GameObject _prefab, GameObject _parent, int _cnt)
    {
        //인벤토리 아이템 오브젝트를 생성하는 함수

        for (int i = 0; i < _cnt; i++)
        {
            GameObject obj = (GameObject)Instantiate(_prefab, new Vector3(0f, 0f, 0), Quaternion.identity); //아이템 오브젝트 생성
            obj.transform.SetParent(_parent.transform, false);   //아이템 모음 오브젝트의 자식 오브젝트로 설정
            itemList.Add(obj);  //리스트에 저장
        }
    }

    public void UpdateInventory()
    {
        //인벤토리 데이터를 기준으로 인벤토리 업데이트
/*
        InventoryInfo_Data _inventoryinfo_data = GameManager.instance.inventoryinfo_data;
        featherData = GameManager.instance.loadFeatherData;   //깃털 정보를 가져옴

        int birdCnt = 0;    //새 번호를 카운트하는 변수(특별새 깃털은 넘어가기 위해)
        int listCnt = 0;    //인벤토리 리스트의 인덱스
        for (int i = 0; i < _inventoryinfo_data.datalist.Count; i++)     //인벤토리 아이템들의 총 개수만큼 반복
        {
            if (i <= 15 && birdCnt == 3)   //만약 특별새라면
            {
                birdCnt = 0;    //특별새 카운트 번호 초기화
                continue;   //인벤토리에 추가하지 않고 넘어감
            }
            else    //만약 특별새가 아니라면
            {
                birdCnt++;  //새 번호 증가

                //int isAcquireed = _inventoryinfo_data.datalist[i].appear;  //해당 새의 획득 여부 가져옴
                int appearNumber = featherData.featherList[i].feather_number;   //깃털의 수를 가져옴
                if (appearNumber != 0)   //획득 하였다면
                {
                    GameObject Item = itemList[listCnt];  //아이템 리스트 한 요소 가져옴
                    GameObject itemImage = Item.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;   //아이템 이미지
                    GameObject itemContentsText = Item.transform.GetChild(1).gameObject;    //아이템 이름, 내용 텍스트
                    GameObject itemCntText = Item.transform.GetChild(2).gameObject; //아이템 개수 텍스트

                    itemImage.SetActive(true);  //아이템 이미지 활성화
                    itemImage.transform.GetChild(0).gameObject.SetActive(false);  //아이템 이미지 안의 드림캐쳐 모양 비활성화
                    itemContentsText.SetActive(true);  //아이템 이름, 내용 텍스트 활성화
                    itemCntText.SetActive(true);  //아이템 개수 텍스트 활성화

                    itemImage.GetComponent<Image>().sprite = ItemImages[i];    //아이템 이미지 변경   (깃털만 해당)

                    itemContentsText.transform.GetChild(0).gameObject.GetComponent<Text>().text = _inventoryinfo_data.datalist[i].name; //아이템 이름 변경
                    string contents = _inventoryinfo_data.datalist[i].contents; //아이템 내용 변경
                    itemContentsText.transform.GetChild(1).gameObject.GetComponent<Text>().text = contents.Replace("nn", "\n");
                    itemCntText.transform.GetChild(1).gameObject.GetComponent<Text>().text = featherData.featherList[i].feather_number.ToString();   //아이템 획득 개수 변경

                    listCnt++;  //리스트 번호 증가
                }
            }
        }                                                                                                                               //obj.transform.SetParent(_parent.transform, false);   //아이템 모음 오브젝트의 자식 오브젝트로 설정

        //인벤토리 데이터를 다 추가했다면 드림캐쳐 데이터도 추가
        List<DreamCatcher> dreamCatcherdataList= new List<DreamCatcher>();
        MyDreamCatcher dreamCatcherData; //MyDreamCatcher 객체 필요
        dreamCatcherData = GameManager.instance.loadDreamCatcherData; //MyDreamCatcher 객체 GameManager에서 가져옴
        int nDreamCatcher = dreamCatcherData.nDreamCatcher;
        Debug.Log("만든 드림캐쳐 개수 : "+ nDreamCatcher);
        if (nDreamCatcher != 0)    //저장된 드림캐쳐가 있다면
        {
            for (int i = 0; i < nDreamCatcher; i++)   //드림캐쳐 개수만큼 반복
            {
                GameObject Item = itemList[listCnt];  //아이템 리스트 한 요소 가져옴
                GameObject itemImage = Item.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;   //아이템 이미지
                Debug.Log(itemImage);
                GameObject itemContentsText = Item.transform.GetChild(1).gameObject;    //아이템 이름, 내용 텍스트
                GameObject itemCntText = Item.transform.GetChild(2).gameObject; //아이템 개수 텍스트

                itemImage.SetActive(true); //아이템 이미지 안의 드림캐쳐 모양 활성화
                itemContentsText.SetActive(true);  //아이템 이름, 내용 텍스트 활성화
                itemCntText.SetActive(false);  //아이템 개수 텍스트 비활성화

                //GameObject dreamCatcherObj = (GameObject)Instantiate(dreamCatcherPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity); //드림캐쳐 오브젝트 생성
                //dreamCatcherObj.transform.SetParent(itemImage.transform, false);   //아이템 모음 오브젝트의 자식 오브젝트로 설정

                // TODO: 아이템 이미지 드림캐쳐 이미지로 변경
                itemImage.GetComponent<Image>().sprite = DreamCatcherInfoLoad.Instance.ImageLoad(i);
                // TODO: 이전 코드 삭제, 게임 오브젝트가 아닌 이미지 스프라이트 변경
                //GameObject dreamCatcherObj = itemImage.transform.GetChild(0).gameObject;
                //    dreamCatcherObj.GetComponent<MakeDreamCatcher>().LoadJson();
                // TODO: 삭제(스샷으로 대체)
                //dreamCatcherObj.GetComponent<MakeDreamCatcher>().MakeDreamCatcherImg(i);    //드림캐쳐 생성 

                //itemImage.GetComponent<Image>().sprite = ItemImages[16];    //아이템 이미지 변경(드림캐쳐 이미지)
                itemContentsText.transform.GetChild(0).gameObject.GetComponent<Text>().text = "드림캐쳐"; //아이템 이름 변경
                //itemContentsText.transform.GetChild(1).gameObject.GetComponent<Text>().text = "드림캐쳐 내용";    //아이템 설명 변경
                string contents = WriteDreamCatcherContents(dreamCatcherData.dreamCatcherList[i]); //아이템 내용 변경
                itemContentsText.transform.GetChild(1).gameObject.GetComponent<Text>().text = contents;
                //itemContentsText.transform.GetChild(1).gameObject.GetComponent<Text>().text = contents.Replace("nn", "\n");
                //itemCntText.transform.GetChild(1).gameObject.GetComponent<Text>().text = curInventoryDreamCatcher_data.dataList[i].number.ToString();   //아이템 획득 개수 변경

                listCnt++;  //리스트 번호 증가
            }
        }*/
    }

    public string WriteDreamCatcherContents(DreamCatcher obj)
    {
        //드림캐쳐 내용 설명을 생성하는 함수
        string content = "";
        switch (obj.DCcolor)
        {
            case 0:
                content += "흰색";
                break;
            case 1:
                content += "노란색";
                break;
            case 2:
                content += "파란색";
                break;
            case 3:
                content += "빨간색";
                break;
            case 4:
                content += "검정색";
                break;
            default:
                content += "";
                break;
        }

        content += "\n";
        content += GetFeatherContents(obj.DCfeather1, obj.DCfeather2, obj.DCfeather3);

        return content;
    }

    // 깃털 정보 가져오기
    public string GetFeatherContents(int _feather1, int _feather2, int _feather3)
    {
        BirdInfo_Data _birdinfo_data = GameManager.instance.birdinfo_data;
        string featherTxt = "";

        if (_feather1 == _feather2 && _feather1 == _feather3)
        {
            featherTxt = _birdinfo_data.dataList[_feather1].name + " 깃털 X3";
        }
        else if (_feather1 == _feather2 && _feather1 != _feather3)
        {
            featherTxt = _birdinfo_data.dataList[_feather1].name + " 깃털 X2" + "\n" + _birdinfo_data.dataList[_feather3].name + " 깃털";
        }
        else if (_feather1 == _feather3 && _feather1 != _feather2)
        {
            featherTxt = _birdinfo_data.dataList[_feather1].name + " 깃털 X2" + "\n" + _birdinfo_data.dataList[_feather2].name + " 깃털";
        }
        else if (_feather2 == _feather3 && _feather2 != _feather1)
        {
            featherTxt = _birdinfo_data.dataList[_feather2].name + " 깃털 X2" + "\n" + _birdinfo_data.dataList[_feather1].name + " 깃털";
        }
        else
        {
            featherTxt = _birdinfo_data.dataList[_feather1].name + " 깃털" + "\n" + _birdinfo_data.dataList[_feather2].name + " 깃털" + "\n" + _birdinfo_data.dataList[_feather3].name + " 깃털";
        }
        return featherTxt;
    }

    public void AddInventory(int itemNumber)
    {
        //증가할 아이템 번호로 해당 인덱스의 깃털을 개수를 추가하는 함수
        featherData = GameManager.instance.featherDataManager;   //깃털 정보를 가져옴

        int featherCnt = featherData.featherList[itemNumber].feather_number;    //현재 깃털 가져옴
        featherData.featherList[itemNumber].feather_number = featherCnt + 1;   //깃털 개수 증가
        if(featherData.featherList[itemNumber].appear==0)
        {
            featherData.featherList[itemNumber].appear = 1;
        }
        Debug.Log("인벤토리에 추가됨");

        GameManager.instance.GetComponent<FeatherNumDataManager>().DataSaveText(featherData);
    }

    public void DeleteInventory(int itemNumber, int cnt)
    {
        //인벤토리 아이템을 삭제하는(사용&판매하는) 함수
        featherData = GameManager.instance.featherDataManager;   //깃털 정보를 가져옴

        int featherCnt = featherData.featherList[itemNumber].feather_number;    //현재 깃털 가져옴
        featherData.featherList[itemNumber].feather_number = (featherCnt - cnt);   //깃털 개수 감소
        Debug.Log("인벤토리에서 삭제됨");

        GameManager.instance.GetComponent<FeatherNumDataManager>().DataSaveText(featherData);
    }


    //Json파일 저장
    public void SaveJson()
    {
        string savePath = getDreamCatcherPath();
        List<DreamCatcher> madeData = new List<DreamCatcher>();
        string jdata = JsonHelper.ToJson(madeData, true);
        Debug.Log("data: " + madeData);
        Debug.Log("jdata: " + jdata);
        File.WriteAllText(savePath, jdata);
        Debug.Log("**********Json파일 저장************");
    }

    public List<DreamCatcher> LoadDreamCatcherCnt()
    {
        //드림캐쳐 Json파일 불러오기

        string savePath = getDreamCatcherPath();


        if (File.Exists(savePath))     //만약 파일이 존재하면
        {
            string jdata = File.ReadAllText(savePath);
            List<DreamCatcher> madeData = JsonHelper.FromJson<DreamCatcher>(jdata); // 드림캐쳐 데이터들 불러옴
            Debug.Log("파일을 불러온 후 data: " + madeData);
            Debug.Log("**********Json파일 불러옴*************");
            return madeData;
        }
        else
        {
            List<DreamCatcher> madeData = new List<DreamCatcher>();
            SaveJson();
            Debug.Log("**********Json파일 없음*************");
            return madeData;
        }
    }

    //드림캐쳐 파일위치
    private static string getDreamCatcherPath()
    {
        string fileName = "DreamCatcherData";
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

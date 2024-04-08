using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteriorCategory : MonoBehaviour
{
    // 인테리어 카테고리 스크립트

    public GameObject Panel_Interior; // 인테리어 패널
    public GameObject[] Button_InteriorCategory;   //인테리어 패널 버튼
    public GameObject[] Button_InteriorItem;    //인테리어 아이템 버튼

    public GameObject[] horLines;   //인테리어 UI의 카테고리 세로선 배열

    int[] currentAdjusting = new int[40];  //현재 적용중인 아이템 여부

    private InteriorContainer curInteriorData;  //현재 플레이어 인테리어 데이터 정보
    public void Start()
    {
        UpdateCatrgoryPanel(0); //카테고리 번호에 맞게 노출되는 패널 업데이트
        DefineButtonNumber();   //카테고리 버튼 고유 번호 지정
        LoadSaveData(); // 저장 데이터 가져옴
        
    }

    public void UpdateCatrgoryPanel(int panelIdx)
    {
        //인테리어 카테고리 버튼 업데이트 함수

        OpenIndexPanel(panelIdx);
        SetHorLine(panelIdx);
    }

    public void SetHorLine(int curCateNum)
    {
        //현재 실행중인 탭에 따라 세로선 배치하는 함수
        //나중엔 이차원 배열로 한 번에 적용되게 해야하나?

        switch (curCateNum)
        {
            case 0:
                horLines[0].SetActive(false);
                horLines[1].SetActive(true);
                horLines[2].SetActive(true);
                break;
            case 1:
                horLines[0].SetActive(true);
                horLines[1].SetActive(false);
                horLines[2].SetActive(true);
                break;
            case 2:
                horLines[0].SetActive(true);
                horLines[1].SetActive(true);
                horLines[2].SetActive(false);
                break;
            default:
                horLines[0].SetActive(false);
                horLines[1].SetActive(true);
                horLines[2].SetActive(true);
                break;
        }
    }

    public void OpenIndexPanel(int idx)
    {
        //해당하는 인덱스의 인테리어 패널을 활성화하는 함수

        int panelCnt = Panel_Interior.gameObject.transform.childCount;   //패널 오브젝트 개수

        for (int i = 0; i < panelCnt; i++)  //패널 오브젝트 개수만큼 반복
        {
            if (i == idx)
            {
                Panel_Interior.gameObject.transform.GetChild(i).gameObject.SetActive(true);    //해당하는 인덱스의 패널 활성화
            }
            else
            {
                Panel_Interior.gameObject.transform.GetChild(i).gameObject.SetActive(false);    //패널 비활성화
            }
        }
    }

    public void DefineButtonNumber()
    {
        // 인테리어 버튼들에 에 고유 번호를 부여하는 함수

        // 패널 버튼에 고유 번호 부여
        int categoryBtnCnt = Button_InteriorCategory.Length;    //인테리어 카테고리 버튼 개수
        for (int i = 0; i < categoryBtnCnt; i++)
        {
            Button_InteriorCategory[i].gameObject.GetComponent<InteriorButton>().SetButtonNumber(i);  //버튼 고유 번호 지정
            Button_InteriorCategory[i].gameObject.GetComponent<InteriorButton>().SettingInteriorFunction();   //버튼 이벤트 지정
        }

        int imgCnt = 0;
        for (int j = 0; j < 40; j++)
        {
            //버튼 이벤트 지정
            Button_InteriorItem[j].gameObject.GetComponent<InteriorButton>().SetButtonNumber(j);

            // 버튼 이벤트 호출하여 설정
            //Button_InteriorItem[j].gameObject.onClick.AddListener();
            //Button_InteriorItem[j].gameObject.GetComponent<InteriorButton>().SettingInteriorItemFunction();


            if (j < 8)  // 꽃병, 상자라면   //gameManager.instance.storeInfo로 가져와야하나
            {
                Button_InteriorItem[j].gameObject.GetComponent<InteriorButton>().SetButtonImgNumber(imgCnt++);  //버튼 고유 이미지 지정

                if (imgCnt > 3)
                {
                    imgCnt = 0; //4가 되면 초기화
                }
            }
            else if (j >= 8 && j <= 12)     //실이라면
            {
                Button_InteriorItem[j].gameObject.GetComponent<InteriorButton>().SetButtonImgNumber(imgCnt++);  //버튼 고유 번호 지정

                if (imgCnt > 4)
                {
                    imgCnt = 0; //5가 되면 초기화
                }
            }
            else
            {
                // 인테리어 아이템이면 (기본 -> 바다 -> 별 순서)

                Button_InteriorItem[j].gameObject.GetComponent<InteriorButton>().SetButtonImgNumber(imgCnt++);  //버튼 고유 번호 지정
                
                if (imgCnt > 2)
                {
                    imgCnt = 0; //3이 되면 초기화
                }
            }
        }
    }

    public string CheckItemCategory2(int itemIdx)
    {
        // 데이터 테이블에서 아이템의 카테고리2를 가져오는 함수

        int itemId = curInteriorData.itemList[itemIdx].itemId;
        string itemCategory = "";

        for (int j = 0; j < 26; j++)
        {
            if (itemId == GameManager.instance.interiorinfo_data.dataList[j].id)
            {
                itemCategory = GameManager.instance.interiorinfo_data.dataList[j].theme.ToString();
            }
        }

        return itemCategory;
    }

    public void UpdateSameItemAdjusting(int itemIdx, int imgIdx)
    {
        // 같은 아이템류 중복 적용중 정리하는 함수
        // *이부분 나중에 아이템 카테고리 가져와서 판단해서 수정하는 방법으로 코드 정리하기
        int startIdx = 0;
        int endIdx = 0;

        if (itemIdx < 8)    // 화분 또는 상자라면 (아이템 종류가 4개)
        {
            if (itemIdx >= 4)   // 인벤토리라면
            {
                startIdx = 4;   // 반복 시작 숫는 4
            }
            endIdx = startIdx + 4;  // 4번 반복
        }
        else if (itemIdx >= 8 && itemIdx < 13) // 실이라면 (아이템 종류가 5개)
        {
            startIdx = 8;
            endIdx = startIdx + 5;  // 5번 반복
        }
        else    // 인베리어 아이템이라면
        {
            string itemCategory = CheckItemCategory2(itemIdx);  // 아이템의 카테고리를 가져옴
            if (itemCategory == "sea")
            {
                startIdx = itemIdx - 1;
            }
            else if (itemCategory == "star")
            {
                startIdx = itemIdx - 2;
            }

            endIdx = startIdx + 3;  // 3번 반복
        }

        for (int i = startIdx; i < endIdx; i++)
        {
            if (itemIdx == i)
            {
                continue;
            }
            else
            {
                currentAdjusting[i] = 0;
                curInteriorData.itemList[i].itemAdjusting = 0;    // 적용X 반영
            }

        }
        if (imgIdx == 0)
        {
            currentAdjusting[startIdx] = 1;
            curInteriorData.itemList[startIdx].itemAdjusting = 1;    // 적용O 반영
        }
    }

    public void UpdateButtonAdjusting(int itemIdx, int imgIdx)
    {
        // 현재 적용중인 아이템에 적용중 표시하는 함수

        for (int j = 0; j < currentAdjusting.Length; j++)
        {
            //적용중인 아이템만 적용중 표시
            int curAdjusting = currentAdjusting[j]; //현재 아이템의 레벨값

            if (curAdjusting == 1)
            {
                Button_InteriorItem[j].gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                Button_InteriorItem[j].gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
    
    public void LoadSaveData()
    {
        // 인테리어 저장 데이터를 가져오는 함수

        curInteriorData = GameManager.instance.loadInteriorData;    //플레이어의 인테리어 정보       

        for (int i = 0; i < currentAdjusting.Length; i++)
        {
            currentAdjusting[i] = curInteriorData.itemList[i].itemAdjusting;
            
            if (currentAdjusting[i] == 1)
            {
                int imgNum = Button_InteriorItem[i].GetComponent<InteriorButton>().imageNumber;
                UpdateInteriorImage(i, imgNum);
                UpdateButtonAdjusting(i, imgNum);
            }
        }
    }

    public void UpdateInteriorImage(int itemIdx, int imgIdx)
    {
        // 가구 이미지를 업데이트하는 함수

        int objectNum = ChangeItemIdxToObjectNum(itemIdx);  // 오브젝트 번호를 가져옴

        //기본 아이템 이미지 변경       
        GameObject itemObj = this.GetComponent<MainProducts>().goodsContents[objectNum].gameObject;   //적용할 아이템 오브젝트를 가져옴
        itemObj.gameObject.GetComponent<Image>().sprite = this.GetComponent<MainProducts>().goodsImages[objectNum].imageList[imgIdx];   //오브젝트 이미지 변경

        if (objectNum == 4)
        {
            //만약 벽지라면

            for (int j = 5; j < 8; j++)     // 벽지와 세트인 오브젝트들도 적용
            {
                this.GetComponent<MainProducts>().goodsContents[j].gameObject.GetComponent<Image>().sprite = this.GetComponent<MainProducts>().goodsImages[j].imageList[imgIdx];
            }
        }
    }

    public int ChangeItemIdxToObjectNum(int itemIdx)
    {
        // 아이템 인덱스 정보로 오브젝트 정보를 반환하는 함수
        int objectNum = 0;  //오브젝트 번호
        if (itemIdx >= 0 && itemIdx < 4)
        {
            objectNum = 1;  //꽃병
        }
        else if (itemIdx >= 4 && itemIdx < 8)
        {
            objectNum = 2;   //인벤토리
        }
        else if (itemIdx >= 8 && itemIdx < 13)
        {
            objectNum = 3;   //실
        }
        else if (itemIdx >= 13 && itemIdx < 16)
        {
            objectNum = 4;   //벽지
        }
        else if (itemIdx >= 16 && itemIdx < 19)
        {
            objectNum = 8;   //가랜드
        }
        else if (itemIdx >= 19 && itemIdx < 22)
        {
            objectNum = 9;   //창틀
        }
        else if (itemIdx >= 22 && itemIdx < 25)
        {
            objectNum = 10;   //패드
        }
        else if (itemIdx >= 25 && itemIdx < 28)
        {
            objectNum = 11;   //깃펜
        }
        else if (itemIdx >= 28 && itemIdx < 31)
        {
            objectNum = 12;   //스타드롭
        }
        else if (itemIdx >= 31 && itemIdx < 34)
        {
            objectNum = 13;   //수정구슬
        }
        else if (itemIdx >= 34 && itemIdx < 37)
        {
            objectNum = 14;   //망원경
        }
        else if (itemIdx >= 37 && itemIdx < 40)
        {
            objectNum = 15;   //오르골
        }

        return objectNum;
    }

    public void SelectInteriorItem(int itemIdx, int imgIdx)
    {
        // 인테리어 아이템 버튼을 클릭하는 함수

        curInteriorData = GameManager.instance.loadInteriorData;
        int adjustingImg = imgIdx;

        if (currentAdjusting[itemIdx] == 1)     // 만약 적용 중인 아이템을 한 번 더 클릭하여 적용 해제했다면
        {
            currentAdjusting[itemIdx] = 0;  //적용 해제
            curInteriorData.itemList[itemIdx].itemAdjusting = 0;    //적용 해제 반영

            adjustingImg = 0;   // 적용할 이미지 == 기본 이미지
            if (itemIdx >= 0 && itemIdx < 4)
            {
                // 화분의 경우
                currentAdjusting[0] = 1;    // 기본 화분 적용
                curInteriorData.itemList[0].itemAdjusting = 1;  // 기본 화분 적용

                //Button_InteriorItem[0].gameObject.transform.GetChild(1).gameObject.SetActive(true); // 적용중 버튼 활성화
            }
            else if (itemIdx >= 4 && itemIdx < 8)
            {
                // 상자의 경우
                currentAdjusting[4] = 1;    // 기본 상자 적용
                curInteriorData.itemList[4].itemAdjusting = 1;  // 기본 상자 적용

                //Button_InteriorItem[4].gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            else if (itemIdx >= 8 && itemIdx < 13)
            {
                // 실의 경우
                currentAdjusting[8] = 1;    // 기본 실 적용
                curInteriorData.itemList[8].itemAdjusting = 1;  // 기본 실 적용

                //Button_InteriorItem[8].gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                // 그 외 아이템의 경우
                string itemCategory = CheckItemCategory2(itemIdx);

                if (itemCategory == "sea")
                {
                    currentAdjusting[itemIdx - 1] = 0;  //적용
                    curInteriorData.itemList[itemIdx - 1].itemAdjusting = 0;    //적용 반영
                }
                else if (itemCategory == "star")
                {
                    currentAdjusting[itemIdx - 2] = 0;  //적용
                    curInteriorData.itemList[itemIdx - 2].itemAdjusting = 0;    //적용 반영
                }
            }
        }
        else    // 적용중이 아닌 아이템을 선택했다면
        {
            currentAdjusting[itemIdx] = 1;   // 적용중으로 변경
            curInteriorData.itemList[itemIdx].itemAdjusting = 1;    // 적용중 반영
        }

        UpdateSameItemAdjusting(itemIdx, adjustingImg); // 같은 종류의 아이템 적용시 해제
        UpdateInteriorImage(itemIdx, adjustingImg);   // 이미지 변경
        UpdateButtonAdjusting(itemIdx, adjustingImg);    // 적용중 이미지 표시

        // 저장
        GameManager.instance.loadInteriorData = curInteriorData;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<InteriorJSON>().DataSaveText(curInteriorData);
    }

    public void SettingItemHide(int itemIdx)
    {
        //인덱스의 버튼을 검정 이미지로 가리는 함수
        
    }
}

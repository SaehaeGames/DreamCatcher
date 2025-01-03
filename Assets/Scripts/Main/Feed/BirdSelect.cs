using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdSelect : MonoBehaviour
{
    // 새 종류와 이미지를 관리하는 클래스

    [Space]
    [Header("[Bird Image]")]
    public Sprite[] birdImage; // 새 몸통 이미지 배열
    public Sprite[] birdFeatherImage;  // 새 깃털 이미지 배열

    public void ChangeBirdImage(GameObject rackBird, int birdNumber)
    {
        // 새 몸통과 깃털의 이미지를 바꾸는 함수

        Image rackBirdImage = rackBird.GetComponent<Image>();
        rackBirdImage.sprite = birdImage[birdNumber];   // 새 몸통 이미지 변경

        Image featherImage = rackBird.transform.GetChild(0).GetComponent<Image>();
        featherImage.sprite = birdFeatherImage[birdNumber];  // 새 깃털 이미지 변경
    }

    public int SelectBirdType(FeedType feed)
    {
        // 먹이를 통해 랜덤으로 새 종류를 정하는 함수

        BirdInfo_Data birdinfo_data = GameManager.instance.birdinfo_data;   // 새 도감 데이터를 가져옴
        MyFeatherNumber featherData = GameManager.instance.featherDataManager; // 깃털 데이터를 가져옴

        int categoryCnt = SettingCategoryCnt((int)feed); //카테고리(새 종류, 먹이) 시작 번호를 설정함
        bool specialBirdAppear = CheckSpecialBirdAppear(categoryCnt);    //특별새가 등장하였는지 여부 (** 수정 필요)
        int selectedBirdNum = 0;
        //만약 특별 새 나오는 조건을 만족한다면 특별 새 등장(해당 먹이의 특별 새가 한 번도 등장하지 않았다면)
        if (!specialBirdAppear)
        {

            //해당 먹이의 새 3마리 모두 깃털 개수가 1 이상이라면
            List<int> specialBirdCheck = new List<int>();   //특별 새 특정 조건 달성을 확인할 리스트
            for (int i = 0; i < 3; i++)
            {
                int appearNumber = featherData.featherList[categoryCnt + i].appear; //등장 횟수를 가져옴
                specialBirdCheck.Add(appearNumber);    //등장 횟수를 저장함
            }

            if (!specialBirdCheck.Contains(0))
            {
                //등장 횟수가 0인 새가 없다면(특별 새는 등장 안 했는데 그 외의 새는 모두 등장했다면)

                specialBirdAppear = true;   //특별 새 등장
                selectedBirdNum = categoryCnt + 3;  //특별 새로 설정
                featherData.featherList[selectedBirdNum].appear = 1; //도감에 특별새 등장 횟수 입력
                //birdinfo_data.DataSaveText(birdinfo_data.datalist); //도감 저장

                return selectedBirdNum; //특별 새 반환
            }
        }

        //특별 새가 나올 차례가 아니라면(이미 해당 먹이의 특별 새가 등장했거나, 다른 세 마리의 새가 다 등장하지 않았다면)
        List<int> birdRandom = new List<int>(); //새 등장 확률을 추첨할 리스트
        for (int i = 0; i < 3; i++)
        {
            int appearCnt = birdinfo_data.dataList[categoryCnt + i].probability;   //도감에 저장된 해당 먹이의 i번째 새의 등장 확률을 가져옴
            Debug.Log("확률 : " + appearCnt);
            for (int j = 0; j < appearCnt; j++)
            {
                birdRandom.Add(i);  //리스트에 i번 새 등장확률만큼 입력
            }
        }

        int randomBird = Random.Range(0, 100);   //랜덤으로 새를 뽑기 위해 난수 생성(0~100 사이의 수 하나)
        Debug.Log("이거 뽑음 : " + birdRandom[randomBird] + ", " + randomBird);
        selectedBirdNum = birdRandom[randomBird] + categoryCnt;     //선택된 새 번호

        return selectedBirdNum;  //선정된 랜덤 새 번호 반환
    }

    public int SettingCategoryCnt(int feedNumber)
    {
        //카테고리 구분 번호를 정하는 함수
        int cnt;

        switch (feedNumber)
        {
            case 0:     // 비둘기 콩
                cnt = 0;
                break;
            case 1:     // 베리
                cnt = 4;
                break;
            case 2:     // 지렁이
                cnt = 8;
                break;
            case 3:     // 고기
                cnt = 12;
                break;
            default:
                cnt = 0;
                break;
        }

        return cnt;
    }

    public bool CheckSpecialBirdAppear(int categoryCnt)
    {
        //해당 먹이의 특별새가 이미 등장하였는지 여부를 반환하는 함수

        BirdInfo_Data birdinfoData = GameManager.instance.birdinfo_data;
        int[] specialBirdIndices = { 3, 7, 11, 15 };

        // 이 부분을 특별새 깃털이 있는지.. 여부로 판단하기
        // 아니면 특별새 획득 여부만 저장하는 json 파일 따로 만들기
/*        foreach (int index in specialBirdIndices)
        {
            if (birdinfoData.dataList[index].appear == 1)
                return true;
        }*/

        return false;
    }
}

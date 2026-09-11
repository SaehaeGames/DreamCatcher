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

        if (rackBird == null || birdNumber < 0 || birdNumber >= birdImage.Length)
        {
            Debug.LogError($"[BirdSelect] 잘못된 새 이미지 요청입니다. birdNumber: {birdNumber}");
            return;
        }

        Image rackBirdImage = rackBird.GetComponent<Image>();
        rackBirdImage.sprite = birdImage[birdNumber];   // 새 몸통 이미지 변경

        Image featherImage = rackBird.transform.GetChild(0).GetComponent<Image>();
        Sprite featherSprite = birdNumber < birdFeatherImage.Length
            ? birdFeatherImage[birdNumber]
            : null;

        featherImage.sprite = featherSprite;
        featherImage.gameObject.SetActive(featherSprite != null);
    }

    public int SelectBirdType(FeedType feed)
    {
        // 먹이를 통해 랜덤으로 새 종류를 정하는 함수

        BirdInfo_Data birdinfo_data = GameManager.instance.birdinfo_data;   // 새 도감 데이터를 가져옴
        FeatherDataManager featherData = GameManager.instance.featherDataManager; // 깃털 데이터를 가져옴

        List<int> normalBirdIndices = new List<int>();
        int specialBirdIndex = -1;

        for (int i = 0; i < birdinfo_data.dataList.Count; i++)
        {
            BirdInfo_Object bird = birdinfo_data.dataList[i];
            if (bird.feed != feed) continue;

            if (bird.isSpecial)
                specialBirdIndex = i;
            else
                normalBirdIndices.Add(i);
        }

        bool collectedAllNormalBirds = normalBirdIndices.Count > 0;
        foreach (int birdIndex in normalBirdIndices)
        {
            if (!featherData.IsFeatherAppeared(birdIndex))
            {
                collectedAllNormalBirds = false;
                break;
            }
        }

        if (specialBirdIndex >= 0
            && collectedAllNormalBirds
            && !featherData.IsFeatherAppeared(specialBirdIndex))
        {
            featherData.UnlockFeather(specialBirdIndex);
            return specialBirdIndex;
        }

        // 특별 새가 나올 차례가 아니라면 일반 새의 probability로 추첨
        List<int> birdRandom = new List<int>();
        foreach (int birdIndex in normalBirdIndices)
        {
            int probability = birdinfo_data.dataList[birdIndex].probability;
            for (int i = 0; i < probability; i++)
            {
                birdRandom.Add(birdIndex);
            }
        }

        if (birdRandom.Count == 0)
        {
            Debug.LogError("birdRandom 리스트가 비어있습니다. BirdInfo 확률이 0인지 확인하세요.");
            return SelectTutorialBirdType(feed);
        }

        int randomBird = Random.Range(0, birdRandom.Count);   //랜덤으로 새를 뽑기 위해 난수 생성
        Debug.Log("이거 뽑음 : " + birdRandom[randomBird] + ", " + randomBird);
        return birdRandom[randomBird];
    }

    public int SelectTutorialBirdType(FeedType feed)
    {
        BirdInfo_Data birdinfoData = GameManager.instance.birdinfo_data;
        for (int i = 0; i < birdinfoData.dataList.Count; i++)
        {
            BirdInfo_Object bird = birdinfoData.dataList[i];
            if (bird.feed == feed && !bird.isSpecial)
            {
                return i;
            }
        }

        Debug.LogError($"[BirdSelect] {feed}의 일반 새를 찾을 수 없습니다.");
        return 0;
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
        if (categoryCnt < 0 || categoryCnt >= birdinfoData.dataList.Count)
        {
            return false;
        }

        FeedType feed = birdinfoData.dataList[categoryCnt].feed;
        for (int i = 0; i < birdinfoData.dataList.Count; i++)
        {
            BirdInfo_Object bird = birdinfoData.dataList[i];
            if (bird.feed == feed && bird.isSpecial)
            {
                return GameManager.instance.featherDataManager.IsFeatherAppeared(i);
            }
        }

        return false;
    }
}

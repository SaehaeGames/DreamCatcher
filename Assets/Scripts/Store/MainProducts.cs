using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainProducts : MonoBehaviour
{
    [Header("[Good Objects]")]
    public Image[] goodsContents;   //상품 오브젝트 배열
    // 효과 있는 아이템 (횃대, 꽃병, 주머니, 실)
    // 인테리어 아이템 ((벽지, 책상),  가랜드, (창틀1, 창틀2, 창틀 인테리어), 패드, 깃펜, 오브젝트1, 수정구슬, 망원경, 오브젝트2)
    public SpriteArray[] goodsImages; //상품 이미지 배열

    private GoodsDataManager goodsDataManager;   //플레이어의 저장된 상품 레벨 정보

    private void Start()
    {
        goodsDataManager = GameManager.instance.goodsDataManager;  //저장 데이터 가져오기
        ResetMainProducts(); //상품 정보 업데이트
    }

    public void ResetMainProducts()
    {
        //상품을 이미지를 불러오는 함수

        
        // 근데 그럼 StoreInfo의 데이터리스트도 바꿔야하나? 아이템id, name, category, contents, effect, gold -> 종류별로 다 해서 16개
        // interiorInfo는 id, name, category, theme, contents, gold -> 종류별로 다 해서 27개 (기본, 바다, 별 순서)
        // 이거 두 개 합칠 수 있나? 다른 필드는 인테리어의 theme, 상점의 effect 밖에 없음. null 가능 필드로 합칠 수 있을 것 같은데?

        // 인테리어 아이템 찾을 때 순서 -> 카테고리가 Wallpaper인 것 추출 -> theme가 Default/Sea/Star인 것 추출/해당 인덱스의 id 추출 -> 다른 함수에서 해당 id인 아이템의 아이템 정보와 골드 가져와서 사용

       


        for (int i = 0; i < goodsContents.Length; i++)
        {
            //Debug.Log(i + "의 이미지를 " + GetGoodsLevelToIndex(i) + " 로 설정");
            SetGoodsImageToIndex(i, GetGoodsLevelToIndex(i));
        }

    }

    private int GetGoodsLevelToIndex(int index)
    {
        // 상품의 레벨을 인덱스로 가져오는 함수
        return goodsDataManager.dataList[index].level;
    }

    private void SetGoodsImageToIndex(int index, int level)
    {
        // 상품 이미지를 인덱스로 설정하는 함수

        goodsContents[index].sprite = goodsImages[index].imageList[level];
    }
}

[System.Serializable]
public class SpriteArray
{
    public Sprite[] imageList; // 상품 이미지 배열
}

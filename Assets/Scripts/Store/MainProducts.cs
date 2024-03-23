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

    private GoodsContainer curGoodsData;   //플레이어의 저장된 상품 레벨 정보

    private void Start()
    {
        ResetMainProducts(); //상품 정보 업데이트
    }

    public void ResetMainProducts()
    {
        //상품을 이미지를 불러오는 함수

        curGoodsData = GameManager.instance.loadGoodsData;  //저장 데이터 가져오기

        for (int i = 0; i < curGoodsData.goodsCount; i++)
        {
            int goodsLevel = curGoodsData.goodsList[i].goodsLevel;  //상품 레벨
            //나중에는 레벨을 바꾸지 말고 보이는 이미지 번호를 따로 저장하기.. 지금은 레벨도 바뀌면서 스탯도 바뀌는데 나중에 이거 막기ㅜㅜ
            goodsContents[i].gameObject.GetComponent<Image>().sprite = goodsImages[i].imageList[goodsLevel]; //상품 이미지 불러옴
        }
    }
}

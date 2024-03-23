using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CollectionDreamArrow : MonoBehaviour
{
    public Sprite circleRed;
    public Sprite circleGray;
    public Image[] circles;
    public int num = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 페이지 표시 원 모양 초기화 (num==0)
        circles[num].sprite = circleRed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 오른쪽 화살표 클릭
    public void ArrowRight()
    {
        if (num >= 0 && num < 5)
        {
            GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음

            // 페이지 표시 원 모양 변경
            circles[num].sprite = circleGray;
            num++;
            circles[num].sprite = circleRed;

            //스크롤 뷰 이동
            gameObject.transform.localPosition = new Vector2(-1214.498f * num, 0);
        }
    }

    // 왼쪽 화살표 클릭
    public void ArrowLeft()
    {
        if (num > 0 && num <= 5)
        {
            GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음

            // 페이지 표시 원 모양 변경
            circles[num].sprite = circleGray;
            num--;
            circles[num].sprite = circleRed;

            // 스크롤 뷰 이동
            gameObject.transform.localPosition = new Vector2(-1214.498f * num, 0);
        }
    }
}

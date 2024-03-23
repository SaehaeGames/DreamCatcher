using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum category { Development, Wallpaper, SpecialProduct};    //카테고리 열거형
enum interior { Star, Sea };    //인테리어 카테고리 열거형

public class CategorySelect : MonoBehaviour
{
    //카테고리 선택 부분을 담당하는 클래스

    [Header("[Current Store]")]
    [SerializeField] private int currentMainCategory; //현재 선택된 상점 화면(보이는 화면)
    public GameObject[] CategoryProducts;   //해당 카테고리 상품 화면
    [SerializeField] private int currentInteriorCategory; //현재 선택된 상점 화면(보이는 화면)
    public GameObject[] InteriorProducts;

    [Space]
    [Header("[Category Background]")]
    public GameObject[] UnselectedBackGround;   //비활성화된 메인 카테고리 배경
    public GameObject[] UnselectedInteriorBackGround;   //비활성화된 인테리어 카테고리 배경

    void Start()
    {
        currentMainCategory = (int)category.Development;   //기본은 보조도구 화면
        CategoryProducts[currentMainCategory].SetActive(true);     //해당 카테고리 상품 화면 활성화
    }

    public int GetSelectedCategory()
    {
        //선택된 화면을 반환하는 함수

        return currentMainCategory;
    }

    public void SetStarInteriorSelect()
    {
        //별 인테리어 화면을 선택하는 함수

        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
        //SetInActiveBeforeCategory(currentInteriorCategory);     //이전 카테고리 비활성화


        InteriorProducts[currentInteriorCategory].SetActive(false);     //이전 카테고리 상품 화면 비활성화
        UnselectedInteriorBackGround[currentInteriorCategory].SetActive(true); // 이전 카테고리 비활성화 배경 활성화

        currentInteriorCategory = (int)interior.Star;

        //SetActiveCurrentCategory(currentInteriorCategory);      //현재 카테고리 활성화
        InteriorProducts[currentInteriorCategory].SetActive(true);     //현재 카테고리 상품 화면 활성화
        UnselectedInteriorBackGround[currentInteriorCategory].SetActive(false); // 현재 카테고리 비활성화 배경 비활성화
        Debug.Log(currentInteriorCategory + "현재 카테고리 번호");
    }

    public void SetSeaInteriorSelect()
    {
        //바다 인테리어 화면을 선택하는 함수

        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
        //SetInActiveBeforeCategory(currentInteriorCategory);     //이전 카테고리 비활성화
        InteriorProducts[currentInteriorCategory].SetActive(false);     //이전 카테고리 상품 화면 비활성화
        UnselectedInteriorBackGround[currentInteriorCategory].SetActive(true); // 이전 카테고리 비활성화 배경 활성화

        Debug.Log(currentInteriorCategory + "이전 카테고리 번호");
        currentInteriorCategory = (int)interior.Sea;
        //SetActiveCurrentCategory(currentInteriorCategory);      //현재 카테고리 활성화

        InteriorProducts[currentInteriorCategory].SetActive(true);     //현재 카테고리 상품 화면 활성화
        UnselectedInteriorBackGround[currentInteriorCategory].SetActive(false); // 현재 카테고리 비활성화 배경 비활성화

        Debug.Log(currentInteriorCategory + "현재 카테고리 번호");
    }

    public void SetDevelopmentSelect()
    {
        //보조도구 화면을 선택하는 함수

        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
        SetInActiveBeforeCategory(currentMainCategory);     //이전 카테고리 비활성화
        currentMainCategory = (int)category.Development;
        SetActiveCurrentCategory(currentMainCategory);      //현재 카테고리 활성화
    }

    public void SetWallpaperSelect()
    {
        //벽지 화면을 선택하는 함수

        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
        SetInActiveBeforeCategory(currentMainCategory);     //이전 카테고리 비활성화
        currentMainCategory = (int)category.Wallpaper;
        currentInteriorCategory = (int)interior.Star;   //기본은 인테리어 별 화면
        SetActiveCurrentCategory(currentMainCategory);      //현재 카테고리 활성화
    }

    public void SetSpecialProductSelect()
    {
        //특별상품 화면을 선택하는 함수

        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 이동 효과음
        SetInActiveBeforeCategory(currentMainCategory);
        currentMainCategory = (int)category.SpecialProduct;
        SetActiveCurrentCategory(currentMainCategory);      //현재 카테고리 활성화
    }

    private void SetActiveCurrentCategory(int currentCategory)
    {
        //현재 카테고리를 활성화 하는 함수

        CategoryProducts[currentCategory].SetActive(true);     //현재 카테고리 상품 화면 활성화
        UnselectedBackGround[currentCategory].SetActive(false); // 현재 카테고리 비활성화 배경 비활성화
    }

    private void SetInActiveBeforeCategory(int beforeCategory)
    {
        //이전 카테고리를 비활성화 하는 함수

        CategoryProducts[beforeCategory].SetActive(false);     //이전 카테고리 상품 화면 비활성화
        UnselectedBackGround[beforeCategory].SetActive(true); // 이전 카테고리 비활성화 배경 활성화
    }
}

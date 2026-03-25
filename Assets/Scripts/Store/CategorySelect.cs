using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategorySelect : MonoBehaviour
{
    //카테고리 선택 부분을 담당하는 클래스

    [Header("[Current Store]")]
    [SerializeField] private StoreType currentMainCategory; //현재 선택된 상점 화면(보이는 화면)
    [SerializeField] private ItemTheme currentInteriorCategory; //현재 선택된 인테리어 상점 화면(보이는 화면)

    public GameObject[] categoryProducts = new GameObject[3]; // Development, Interior, SpecialProduct
    public GameObject[] unselectedBackground = new GameObject[3];

    public GameObject[] interiorProducts = new GameObject[3]; // Default, Sea, Star
    public GameObject[] unselectedInteriorBackground = new GameObject[3];
    void Start()
    {
        currentMainCategory = (int)StoreType.Development;   //기본은 보조도구 화면


        ActivateCategory(categoryProducts, unselectedBackground, currentMainCategory);

        //CategoryProducts[currentMainCategory].SetActive(true);     //해당 카테고리 상품 화면 활성화
    }

    public StoreType GetSelectedCategory() => currentMainCategory;

    // 메인에서 카테고리 선택
    public void SetDevelopmentSelect() => SetMainCategory(StoreType.Development);
    public void SetWallpaperSelect()
    {
        SetMainCategory(StoreType.Interior);
        // Sea를 비선택 상태로 만들고 Star를 선택 상태로 활성화
        // currentInteriorCategory를 Sea로 설정 → DeactivateCategory(Sea) 후 ActivateCategory(Star) 순서로 실행됨
        currentInteriorCategory = ItemTheme.Sea;
        SetStarInteriorSelect();
    }
    public void SetSpecialProductSelect() => SetMainCategory(StoreType.SpecialProduct);

    // 인테리어 카테고리 선택
    public void SetStarInteriorSelect() => SetInteriorCategory(ItemTheme.Star);
    public void SetSeaInteriorSelect() => SetInteriorCategory(ItemTheme.Sea);

    // 통합 메서드
    private void SetMainCategory(StoreType category)
    {
        PlayOpenSceneEffect();
        DeactivateCategory(categoryProducts, unselectedBackground, currentMainCategory);
        currentMainCategory = category;
        ActivateCategory(categoryProducts, unselectedBackground, currentMainCategory);
    }

    private void SetInteriorCategory(ItemTheme theme)
    {
        PlayOpenSceneEffect();
        DeactivateCategory(interiorProducts, unselectedInteriorBackground, currentInteriorCategory);
        currentInteriorCategory = theme;
        ActivateCategory(interiorProducts, unselectedInteriorBackground, currentInteriorCategory);
        this.GetComponent<StoreData>().UpdateStoreInteriorData(currentInteriorCategory);
    }

    private void ActivateCategory<T>(GameObject[] products, GameObject[] backgrounds, T category) where T : Enum
    {
        int categoryIndex = Convert.ToInt32(category);
        products[categoryIndex].SetActive(true);
        backgrounds[categoryIndex].SetActive(false);
    }

    private void DeactivateCategory<T>(GameObject[] products, GameObject[] backgrounds, T category) where T : Enum
    {
        int categoryIndex = Convert.ToInt32(category);
        products[categoryIndex].SetActive(false);
        backgrounds[categoryIndex].SetActive(true);
    }

    private void PlayOpenSceneEffect()
    {
        var audioManager = GameObject.FindGameObjectWithTag("AudioManager");
        if (audioManager != null)
        {
            var effectChange = audioManager.GetComponent<EffectChange>();
            effectChange?.PlayEffect_OpenScene();
        }
        else
        {
            Debug.LogWarning("AudioManager 게임 오브젝트를 찾을 수 없음");
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class CategorySelect : MonoBehaviour
{
    [Header("[Current Store]")]
    [SerializeField] private StoreType currentMainCategory;       // 현재 선택된 메인 카테고리 (Development / Interior / SpecialProduct)
    [SerializeField] private ItemTheme currentInteriorCategory;   // 현재 선택된 인테리어 서브 카테고리 (Sea / Star)

    public GameObject[] categoryProducts = new GameObject[3];             // 메인 카테고리별 컨텐츠 패널 (Development, Interior, SpecialProduct 순)
    public GameObject[] unselectedBackground = new GameObject[3];         // 메인 카테고리 버튼의 비선택 배경 이미지 (선택 시 숨김)

    public GameObject[] interiorProducts = new GameObject[3];             // 인테리어 서브 카테고리 컨텐츠 패널 (Default, Sea, Star 순 — enum 인덱스 기준)
    public GameObject[] unselectedInteriorBackground = new GameObject[3]; // 인테리어 서브 카테고리 버튼의 비선택 배경 이미지

    private EffectChange effectChange;

    /// <summary>
    /// 씬 시작 시 보조도구(Development) 카테고리를 기본 선택 상태로 초기화
    /// AudioManager에서 EffectChange 컴포넌트를 가져와 캐싱
    /// </summary>
    void Start()
    {
        currentMainCategory = StoreType.Development;
        ActivateCategory(categoryProducts, unselectedBackground, currentMainCategory);

        var audioManager = GameObject.FindGameObjectWithTag(Constants.Tag_AudioManager);
        effectChange = audioManager?.GetComponent<EffectChange>();
    }

    /// <summary>현재 선택된 메인 카테고리를 반환</summary>
    public StoreType GetSelectedCategory() => currentMainCategory;

    /// <summary>보조도구 탭 버튼 클릭 시 호출 — Development 카테고리로 전환</summary>
    public void SetDevelopmentSelect() => SetMainCategory(StoreType.Development);

    /// <summary>
    /// 인테리어 탭 버튼 클릭 시 호출 — Interior 카테고리로 전환하고 Star 서브탭을 기본 선택
    /// DeactivateCategory가 currentInteriorCategory 값을 기준으로 동작하므로,
    /// Sea로 먼저 설정한 뒤 SetStarInteriorSelect()를 호출해야 Sea → Star 전환이 정상 작동
    /// </summary>
    public void SetWallpaperSelect()
    {
        SetMainCategory(StoreType.Interior);
        currentInteriorCategory = ItemTheme.Sea;
        SetStarInteriorSelect();
    }

    /// <summary>특수 상품 탭 버튼 클릭 시 호출 — SpecialProduct 카테고리로 전환</summary>
    public void SetSpecialProductSelect() => SetMainCategory(StoreType.SpecialProduct);

    /// <summary>Star 서브탭 버튼 클릭 시 호출 — Star 인테리어 카테고리로 전환</summary>
    public void SetStarInteriorSelect() => SetInteriorCategory(ItemTheme.Star);

    /// <summary>Sea 서브탭 버튼 클릭 시 호출 — Sea 인테리어 카테고리로 전환</summary>
    public void SetSeaInteriorSelect() => SetInteriorCategory(ItemTheme.Sea);

    /// <summary>
    /// 메인 카테고리를 전환
    /// 기존 카테고리의 UI를 비활성화하고 새 카테고리의 UI를 활성화
    /// </summary>
    private void SetMainCategory(StoreType category)
    {
        PlayOpenSceneEffect();
        DeactivateCategory(categoryProducts, unselectedBackground, currentMainCategory);
        currentMainCategory = category;
        ActivateCategory(categoryProducts, unselectedBackground, currentMainCategory);
    }

    /// <summary>
    /// 인테리어 서브 카테고리를 전환
    /// 기존 서브탭 UI를 비활성화하고 새 서브탭 UI를 활성화한 뒤,
    /// StoreData에 테마 변경을 알려 상품 목록 UI를 갱신
    /// </summary>
    private void SetInteriorCategory(ItemTheme theme)
    {
        PlayOpenSceneEffect();
        DeactivateCategory(interiorProducts, unselectedInteriorBackground, currentInteriorCategory);
        currentInteriorCategory = theme;
        ActivateCategory(interiorProducts, unselectedInteriorBackground, currentInteriorCategory);
        GetComponent<StoreData>().UpdateStoreInteriorData(currentInteriorCategory);
    }

    /// <summary>
    /// enum 값을 배열 인덱스로 변환해 해당 카테고리 UI를 활성화
    /// products[i]는 컨텐츠 패널 → SetActive(true), backgrounds[i]는 버튼 비선택 배경 → SetActive(false).
    /// </summary>
    private void ActivateCategory<T>(GameObject[] products, GameObject[] backgrounds, T category) where T : Enum
    {
        int i = Convert.ToInt32(category);
        products[i].SetActive(true);
        backgrounds[i].SetActive(false);
    }

    /// <summary>
    /// enum 값을 배열 인덱스로 변환해 해당 카테고리 UI를 비활성화
    /// products[i]는 컨텐츠 패널 → SetActive(false), backgrounds[i]는 버튼 비선택 배경 → SetActive(true).
    /// </summary>
    private void DeactivateCategory<T>(GameObject[] products, GameObject[] backgrounds, T category) where T : Enum
    {
        int i = Convert.ToInt32(category);
        products[i].SetActive(false);
        backgrounds[i].SetActive(true);
    }

    private void PlayOpenSceneEffect()
    {
        effectChange?.PlayEffect_OpenScene();
    }
}

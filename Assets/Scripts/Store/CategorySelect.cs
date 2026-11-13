using UnityEngine;

/// <summary>실제 선택값을 기준으로 모든 탭의 활성 상태를 일관되게 갱신합니다.</summary>
public class CategorySelect : MonoBehaviour
{
    [Header("[Current Store]")]
    [SerializeField] private StoreType currentMainCategory;
    [SerializeField] private ItemTheme currentInteriorCategory;
    public GameObject[] categoryProducts = new GameObject[3];
    public GameObject[] unselectedBackground = new GameObject[3];
    public GameObject[] interiorProducts = new GameObject[3];
    public GameObject[] unselectedInteriorBackground = new GameObject[3];
    [SerializeField] private EffectChange effectChange;

    private StoreData storeData;

    private void Awake()
    {
        // 담당 객체와 효과음 참조는 탭을 누를 때마다 탐색하지 않고 시작할 때 보관합니다.
        storeData = GetComponent<StoreData>();
        if (effectChange == null)
        {
            GameObject audioObject = GameObject.FindGameObjectWithTag(Constants.Tag_AudioManager);
            if (audioObject != null) effectChange = audioObject.GetComponent<EffectChange>();
        }
    }

    private void Start()
    {
        // 1. 기존 씬의 활성 상태와 무관하게 처음 선택할 두 카테고리를 정합니다.
        currentMainCategory = StoreType.Development;
        currentInteriorCategory = ItemTheme.Star;
        // 2. 이 컴포넌트는 패널만 준비하며, 상품 데이터 초기화는 StoreData가 담당합니다.
        RefreshPanels(categoryProducts, unselectedBackground, (int)currentMainCategory);
        RefreshPanels(interiorProducts, unselectedInteriorBackground, (int)currentInteriorCategory);
    }

    // 다음 함수 이름은 씬의 버튼 이벤트가 사용하므로 유지합니다.
    public void SetDevelopmentSelect() => SetMainCategory(StoreType.Development);
    public void SetSpecialProductSelect() => SetMainCategory(StoreType.SpecialProduct);

    public void SetWallpaperSelect()
    {
        // 인테리어 탭 진입 시 기본 테마를 선택하되 한 클릭에서 효과음은 한 번만 재생합니다.
        SetMainCategory(StoreType.Interior);
        SetInteriorCategory(ItemTheme.Star, false);
    }

    public void SetStarInteriorSelect() => SetInteriorCategory(ItemTheme.Star);
    public void SetSeaInteriorSelect() => SetInteriorCategory(ItemTheme.Sea);

    /// <summary>주 카테고리가 달라졌을 때만 패널과 선택 배경을 바꿉니다.</summary>
    private void SetMainCategory(StoreType category)
    {
        if (category == currentMainCategory) return;
        currentMainCategory = category;
        RefreshPanels(categoryProducts, unselectedBackground, (int)category);
        PlayOpenSceneEffect();
    }

    /// <summary>단순한 두 테마 분기로 표시 패널과 상품 보유 상태를 함께 갱신합니다.</summary>
    private void SetInteriorCategory(ItemTheme theme, bool playSound = true)
    {
        if (theme != ItemTheme.Sea && theme != ItemTheme.Star) return;
        bool changed = currentInteriorCategory != theme;
        currentInteriorCategory = theme;
        RefreshPanels(interiorProducts, unselectedInteriorBackground, (int)theme);
        if (storeData != null) storeData.UpdateStoreInteriorData(theme);
        if (changed && playSound) PlayOpenSceneEffect();
    }

    /// <summary>선택 탭만 켜고 나머지는 끕니다. 사용하지 않는 Default 슬롯의 null은 허용합니다.</summary>
    private static void RefreshPanels(GameObject[] products, GameObject[] backgrounds, int selected)
    {
        if (products != null)
            for (int i = 0; i < products.Length; i++)
                if (products[i] != null) products[i].SetActive(i == selected);
        if (backgrounds != null)
            for (int i = 0; i < backgrounds.Length; i++)
                if (backgrounds[i] != null) backgrounds[i].SetActive(i != selected);
    }

    private void PlayOpenSceneEffect()
    {
        if (effectChange != null) effectChange.PlayEffect_OpenScene();
    }
}

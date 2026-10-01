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
        storeData = GetComponent<StoreData>();
        if (effectChange == null)
        {
            GameObject audioObject = GameObject.FindGameObjectWithTag(Constants.Tag_AudioManager);
            if (audioObject != null) effectChange = audioObject.GetComponent<EffectChange>();
        }
    }

    private void Start()
    {
        // 기존 씬의 활성 상태와 무관하게 모든 탭을 초기화합니다.
        currentMainCategory = StoreType.Development;
        currentInteriorCategory = ItemTheme.Star;
        RefreshPanels(categoryProducts, unselectedBackground, (int)currentMainCategory);
        RefreshPanels(interiorProducts, unselectedInteriorBackground, (int)currentInteriorCategory);
        if (storeData != null) storeData.Initialize();
    }

    public StoreType GetSelectedCategory() => currentMainCategory;
    public void SetDevelopmentSelect() => SetMainCategory(StoreType.Development);
    public void SetSpecialProductSelect() => SetMainCategory(StoreType.SpecialProduct);

    public void SetWallpaperSelect()
    {
        SetMainCategory(StoreType.Interior);
        SetInteriorCategory(ItemTheme.Star, false);
    }

    public void SetStarInteriorSelect() => SetInteriorCategory(ItemTheme.Star);
    public void SetSeaInteriorSelect() => SetInteriorCategory(ItemTheme.Sea);

    private void SetMainCategory(StoreType category)
    {
        if (category == currentMainCategory) return;
        currentMainCategory = category;
        RefreshPanels(categoryProducts, unselectedBackground, (int)category);
        PlayOpenSceneEffect();
    }

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

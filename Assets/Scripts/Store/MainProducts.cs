using UnityEngine;
using UnityEngine.UI;

/// <summary>이미 로드된 보조도구 데이터를 사용하여 Main 화면의 상품 이미지를 표시합니다.</summary>
public class MainProducts : MonoBehaviour
{
    [Header("[Good Objects]")]
    public GameObject[] goodsContents;
    public SpriteArray[] goodsImages;

    [Tooltip("표시 대상별 GoodsData ID입니다. 저장 리스트의 순서가 바뀌어도 같은 대상을 조회합니다.")]
    [SerializeField] private string[] goodsIds =
    {
        "JS_3000", "JS_3001", "JS_3002", "JS_3003", "JS_3004",
        "JS_3005", "JS_3006", "JS_3007", "JS_3008", "JS_3009",
        "JS_3010", "JS_3011", "JS_3012", "JS_3013", "JS_3014",
        "JS_3015", "JS_3016"
    };

    private Image[] productImages;
    private GoodsDataManager goodsDataManager;

    private void Start()
    {
        // 1. 공통 초기화에서 준비한 매니저를 사용하여 데이터 객체가 화면별로 교체되지 않게 합니다.
        goodsDataManager = GameManager.instance != null ? GameManager.instance.goodsDataManager : null;
        if (goodsDataManager == null)
        {
            Debug.LogError("[MainProducts] 보조도구 데이터가 초기화되지 않았습니다.", this);
            return;
        }
        // 2. 이미지 컴포넌트는 한 번 캐싱하고, 이후 갱신은 저장 데이터 조회와 이미지 변경만 수행합니다.
        productImages = new Image[goodsContents?.Length ?? 0];
        for (int i = 0; i < productImages.Length; i++)
            if (goodsContents[i] != null) productImages[i] = goodsContents[i].GetComponent<Image>();
        ResetMainProducts();
    }

    /// <summary>데이터를 다시 로드하거나 매니저를 교체하지 않고 지정 ID의 이미지만 갱신합니다.</summary>
    public void ResetMainProducts()
    {
        // 1. 기존 Inspector 배열을 유지하면서 모든 참조가 존재하는 공통 범위만 처리합니다.
        if (goodsDataManager == null || productImages == null || goodsIds == null || goodsImages == null) return;
        int count = Mathf.Min(productImages.Length, Mathf.Min(goodsIds.Length, goodsImages.Length));
        for (int i = 0; i < count; i++)
        {
            // 2. 비어 있는 표시 슬롯을 건너뛰고 ID로 해당 대상의 현재 레벨을 조회합니다.
            Image image = productImages[i];
            Sprite[] sprites = goodsImages[i]?.imageList;
            string id = goodsIds[i];
            if (image == null || sprites == null || string.IsNullOrEmpty(id)) continue;
            GoodsData item = goodsDataManager.GetGoodsDataByID(id);
            if (item == null || item.level < 0 || item.level >= sprites.Length)
            {
                Debug.LogWarning("[MainProducts] 표시 대상 ID 또는 레벨이 올바르지 않습니다: " + id, this);
                continue;
            }

            // 3. 연결되지 않은 레벨 이미지는 기존 화면을 유지하여 정상 이미지를 지우지 않습니다.
            Sprite sprite = sprites[item.level];
            if (sprite != null && image.sprite != sprite) image.sprite = sprite;
        }
    }

    private void OnValidate()
    {
        if (goodsContents != null && goodsIds != null && goodsContents.Length != goodsIds.Length)
            Debug.LogWarning("[MainProducts] 표시 대상과 데이터 ID 배열 길이를 맞춰 주세요.", this);
        if (goodsContents != null && goodsImages != null && goodsContents.Length != goodsImages.Length)
            Debug.LogWarning("[MainProducts] 표시 대상과 레벨 이미지 배열 길이를 맞춰 주세요.", this);
    }
}

/// <summary>보조도구 하나의 레벨별 이미지를 묶습니다. 인테리어 코드도 사용하는 기존 직렬화 구조입니다.</summary>
[System.Serializable]
public class SpriteArray
{
    public Sprite[] imageList;
}

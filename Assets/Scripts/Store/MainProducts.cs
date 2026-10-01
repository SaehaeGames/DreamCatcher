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
        goodsDataManager = GameManager.instance != null ? GameManager.instance.goodsDataManager : null;
        if (goodsDataManager == null)
        {
            Debug.LogError("[MainProducts] 보조도구 데이터가 초기화되지 않았습니다.", this);
            return;
        }
        productImages = new Image[goodsContents?.Length ?? 0];
        for (int i = 0; i < productImages.Length; i++)
            if (goodsContents[i] != null) productImages[i] = goodsContents[i].GetComponent<Image>();
        ResetMainProducts();
    }

    /// <summary>데이터를 다시 로드하거나 매니저를 교체하지 않고 지정 ID의 이미지만 갱신합니다.</summary>
    public void ResetMainProducts()
    {
        if (goodsDataManager == null || productImages == null) return;
        for (int i = 0; i < productImages.Length; i++)
        {
            if (goodsIds == null || i >= goodsIds.Length || productImages[i] == null
                || goodsImages == null || i >= goodsImages.Length || goodsImages[i]?.imageList == null) continue;
            GoodsData item = goodsDataManager.GetGoodsDataByID(goodsIds[i]);
            Sprite[] sprites = goodsImages[i].imageList;
            if (item == null || item.level < 0 || item.level >= sprites.Length)
            {
                Debug.LogWarning("[MainProducts] 표시 대상 ID 또는 레벨이 올바르지 않습니다: " + goodsIds[i], this);
                continue;
            }
            if (sprites[item.level] != null) productImages[i].sprite = sprites[item.level];
        }
    }

    private void OnValidate()
    {
        if (goodsContents != null && goodsIds != null && goodsContents.Length != goodsIds.Length)
            Debug.LogWarning("[MainProducts] 표시 대상과 데이터 ID 배열 길이를 맞춰 주세요.", this);
    }
}

[System.Serializable]
public class SpriteArray
{
    public Sprite[] imageList;
}

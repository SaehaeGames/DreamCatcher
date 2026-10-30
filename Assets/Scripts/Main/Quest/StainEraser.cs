using System;
using UnityEngine;
using UnityEngine.UI;

public class StainEraser : MonoBehaviour
{
    [Header("얼룩")]
    [SerializeField] private Image stainImage;

    [Header("지우개 설정")]
    [SerializeField, Min(1)]
    private int brushRadiusPixels = 60;

    [SerializeField, Range(0f, 1f)]
    private float eraseThreshold = 0.7f;

    [Header("커서 관리")]
    [SerializeField] private QuestUIController questUIController;

    // 얼룩이 설정 비율만큼 지워졌을 때 호출
    public event Action OnErased;

    private Texture2D eraseTexture;
    private Sprite runtimeSprite;
    private Color32[] pixels;

    private int originalPixelCount;
    private int remainingPixelCount;

    private bool isErased;
    private bool hasPreviousPosition;
    private Vector2 previousPosition;

    private bool canErase = false;

    public float ErasedRatio { get; private set; }

    private void Awake()
    {
        questUIController = this.transform.parent.parent.GetComponent<QuestUIController>();
        
        if (stainImage == null)
            stainImage = GetComponent<Image>();

        if (stainImage == null || stainImage.sprite == null)
        {
            Debug.LogError("StainEraser: 얼룩 Image 또는 Sprite가 없습니다.", this);
            enabled = false;
            return;
        }

        // 스프라이트가 아틀라스에 들어가 있어도
        // 실제 스프라이트 영역만 복사한다.
        Sprite sourceSprite = stainImage.sprite;
        Texture2D sourceTexture = sourceSprite.texture;
        Rect textureRect = sourceSprite.textureRect;

        int width = Mathf.RoundToInt(textureRect.width);
        int height = Mathf.RoundToInt(textureRect.height);

        eraseTexture = new Texture2D(
            width,
            height,
            TextureFormat.RGBA32,
            false
        );

        eraseTexture.filterMode = FilterMode.Bilinear;
        eraseTexture.wrapMode = TextureWrapMode.Clamp;

        Color32[] sourcePixels = sourceTexture.GetPixels32();

        int startX = Mathf.RoundToInt(textureRect.x);
        int startY = Mathf.RoundToInt(textureRect.y);

        pixels = new Color32[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int sourceIndex = (startY + y) * sourceTexture.width
                               + (startX + x);

                int targetIndex = y * width + x;

                pixels[targetIndex] = sourcePixels[sourceIndex];
            }
        }

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0)
                originalPixelCount++;
        }

        remainingPixelCount = originalPixelCount;

        eraseTexture.SetPixels32(pixels);
        eraseTexture.Apply(false, false);

        runtimeSprite = Sprite.Create(
            eraseTexture,
            new Rect(0, 0, width, height),
            sourceSprite.pivot / sourceSprite.rect.size,
            sourceSprite.pixelsPerUnit
        );

        stainImage.sprite = runtimeSprite;
    }

    public void EraseAtScreenPosition(
        Vector2 screenPosition,
        Camera eventCamera)
    {
        if (!canErase || isErased || !gameObject.activeInHierarchy)
            return;

        RectTransform rectTransform = stainImage.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            screenPosition,
            eventCamera,
            out Vector2 localPosition))
        {
            return;
        }

        Rect rect = rectTransform.rect;

        float u = (localPosition.x - rect.xMin) / rect.width;
        float v = (localPosition.y - rect.yMin) / rect.height;

        if (u < 0f || u > 1f || v < 0f || v > 1f)
        {
            hasPreviousPosition = false;
            return;
        }

        Vector2 currentPosition = new Vector2(
            u * (eraseTexture.width - 1),
            v * (eraseTexture.height - 1)
        );

        if (hasPreviousPosition)
        {
            float distance = Vector2.Distance(
                previousPosition,
                currentPosition
            );

            // 브러시 반경보다 촘촘하게 점을 찍어 빈틈을 방지
            float spacing = Mathf.Max(1f, brushRadiusPixels * 0.5f);

            int steps = Mathf.Max(
                1,
                Mathf.CeilToInt(distance / spacing)
            );

            for (int i = 1; i <= steps; i++)
            {
                float t = (float)i / steps;

                Vector2 point = Vector2.Lerp(
                    previousPosition,
                    currentPosition,
                    t
                );

                EraseCircle(
                    Mathf.RoundToInt(point.x),
                    Mathf.RoundToInt(point.y)
                );
            }
        }
        else
        {
            EraseCircle(
                Mathf.RoundToInt(currentPosition.x),
                Mathf.RoundToInt(currentPosition.y)
            );
        }

        previousPosition = currentPosition;
        hasPreviousPosition = true;

        eraseTexture.SetPixels32(pixels);
        eraseTexture.Apply(false, false);

        CheckProgress();
    }

    private void EraseCircle(int centerX, int centerY)
    {
        int radiusSquared = brushRadiusPixels * brushRadiusPixels;

        for (int y = -brushRadiusPixels; y <= brushRadiusPixels; y++)
        {
            for (int x = -brushRadiusPixels; x <= brushRadiusPixels; x++)
            {
                if (x * x + y * y > radiusSquared)
                    continue;

                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= eraseTexture.width ||
                    py < 0 || py >= eraseTexture.height)
                    continue;

                int index = py * eraseTexture.width + px;

                // 이미 투명한 픽셀은 다시 계산하지 않는다.
                if (pixels[index].a == 0)
                    continue;

                pixels[index].a = 0;
                remainingPixelCount--;
            }
        }
    }

    private void CheckProgress()
    {
        if (originalPixelCount <= 0)
            return;

        ErasedRatio =
            1f - (float)remainingPixelCount / originalPixelCount;

        if (ErasedRatio >= eraseThreshold)
        {
            isErased = true;
            hasPreviousPosition = false;

            stainImage.gameObject.SetActive(false);
            OnErased?.Invoke();
        }
    }

    public void SetCanErase(bool value)
    {
        canErase = value;

        // 지우개를 놓으면 다음 드래그는 새 입력으로 처리
        if (!value)
            hasPreviousPosition = false;
    }

    private void OnDestroy()
    {
        if (runtimeSprite != null)
            Destroy(runtimeSprite);

        if (eraseTexture != null)
            Destroy(eraseTexture);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryItemType
{
    None,
    Feather,
    DreamCatcher
}

public class InventoryItemSlot : MonoBehaviour
{
    [Header("UI")]
    public Image ItemImage;
    public Text itemNameText;
    public Text descriptionText;
    public Text countText;
    public GameObject selectedIcon;
    public GameObject ItemImageInteractableMask;

    private DreamCatcherInventoryData dreamCatcherInventoryData;
    private FeatherData featherData;

    private InventoryItemType type;
    private bool selected;
    private InventoryManager inventoryManager;

    private Texture2D loadedTexture;
    private Sprite loadedSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSlotFeather(InventoryManager _inventoryManager, FeatherData _featherData, Sprite itemSprite, string itemName, string description, int count)
    {
        inventoryManager= _inventoryManager;
        featherData = _featherData;
        type = InventoryItemType.Feather;

        gameObject.SetActive(true);
        ItemImage.gameObject.SetActive(true);
        countText.gameObject.SetActive(true);

        ItemImage.sprite = itemSprite;
        itemNameText.text = itemName;
        descriptionText.text = description;
        countText.text = "X " + count.ToString();

        selected = false;
        selectedIcon.SetActive(selected);
    }

    public void SetSlotDreamCatcher(InventoryManager _inventoryManager, DreamCatcherInventoryData _dreamCatcherInventoryData, string itemName, string description, int count) 
    {
        inventoryManager= _inventoryManager;
        dreamCatcherInventoryData= _dreamCatcherInventoryData;
        type = InventoryItemType.DreamCatcher;

        gameObject.SetActive(true);
        ItemImage.gameObject.SetActive(true);
        countText.gameObject.SetActive(true);

        LoadDreamCatcherThumbnail();

        itemNameText.text = "드림 캐쳐";
        descriptionText.text = description;
        countText.text = "X " + count.ToString();

        selected = false;
        selectedIcon.SetActive(selected);
    }

    private void LoadDreamCatcherThumbnail()
    {
        string dreamCatcherId =
        dreamCatcherInventoryData.GetDCids()[0];

        string path =
            DreamCatcherThumbnailRenderer.GetThumbnailPathStatic(
                dreamCatcherId);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"썸네일 없음 : {path}");

            ItemImage.gameObject.SetActive(false);
            return;
        }

        byte[] bytes = File.ReadAllBytes(path);

        loadedTexture = new Texture2D(2, 2);
        loadedTexture.LoadImage(bytes);

        loadedSprite = Sprite.Create(
            loadedTexture,
            new Rect(
                0,
                0,
                loadedTexture.width,
                loadedTexture.height),
            new Vector2(0.5f, 0.5f));

        ItemImage.sprite = loadedSprite;
    }

    public DreamCatcherInventoryData GetSlotDreamCatcherInventoryData()
    {
        if (type != InventoryItemType.DreamCatcher)
        {
            Debug.LogError($"[InventoryItemSlot] Invalid type: {type}");
            return null;
        }

        return dreamCatcherInventoryData;
    }

    public FeatherData GetSlotFeatherData()
    {
        if(type != InventoryItemType.Feather)
        {
            Debug.LogError($"[InventoryItemSlot] Invalid type: {type}");
            return null;
        }

        return featherData;
    }

    public void SetInventoryItemType(InventoryItemType _type)
    {
        this.type = _type;
    }

    public InventoryItemType GetInventoryItemType()
    {
        return type;
    }

    public void SetSelected(bool value)
    {
        selected = value;
        selectedIcon.SetActive(selected);
    }

    public void SelectInventoryItem()
    {
        inventoryManager.SelectSlot(this);
    }

    public void SetInteractable(bool interactable)
    {
        this.GetComponent<Button>().interactable = interactable;
        ItemImageInteractableMask.SetActive(!interactable);
    }
}

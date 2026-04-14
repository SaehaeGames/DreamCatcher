using System;
using System.Collections;
using System.Collections.Generic;
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

    private DreamCatcherInventoryData dreamCatcherInventoryData;
    private FeatherData featherData;

    private InventoryItemType type;
    private bool selected;
    public InventoryManager inventoryManager;

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

    public void SetSlotDreamCatcher(InventoryManager _inventoryManager, DreamCatcherInventoryData _dreamCatcherInventoryData, Sprite itemSprite, string itemName, string description, int count) 
    {
        inventoryManager= _inventoryManager;
        dreamCatcherInventoryData= _dreamCatcherInventoryData;
        type = InventoryItemType.DreamCatcher;

        gameObject.SetActive(true);
        ItemImage.gameObject.SetActive(true);
        countText.gameObject.SetActive(true);

        if(itemSprite == null)
        {
            ItemImage.gameObject.SetActive(false);
        }
        else
        {
            ItemImage.sprite = itemSprite;
        }
        itemNameText.text = "드림 캐쳐";
        descriptionText.text = description;
        countText.text = "X " + count.ToString();

        selected = false;
        selectedIcon.SetActive(selected);
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
}

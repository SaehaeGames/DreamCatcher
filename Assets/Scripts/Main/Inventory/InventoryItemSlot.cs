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

    private InventoryItemType type;
    private bool selected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSlotFeather(Sprite itemSprite, string itemName, string description, int count)
    {
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

    public void SetSlotDreamCatcher(Sprite itemSprite, string itemName, string description, int count) 
    {
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

    public void SetInventoryItemType(InventoryItemType _type)
    {
        this.type = _type;
    }

    public void SelectInventoryItem()
    {
        selected = !selected;
        selectedIcon.SetActive(selected);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryMode
{
    None,
    Normal,
    Shipment
}
public class InventoryUIController : MonoBehaviour
{
    [Header("[UI Object]")]
    public GameObject disassembleButton;
    public GameObject shipmentButton;

    private InventoryManager inventoryManager;
    private InventoryMode currentMode;

    private void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

    public void OpenNormalInventory()
    {
        inventoryManager.OpenInventory();
        SetMode(InventoryMode.Normal);
    }

    public void OpenShipmentInventory()
    {
        inventoryManager.OpenInventory();
        SetMode(InventoryMode.Shipment);
    }

    public void SetMode(in InventoryMode mode)
    {
        currentMode = mode;
        RefreshUI();
    }

    public void RefreshUI()
    {
        var slot = inventoryManager.GetSelectedSlot();
        UpdateDisassembleButton(slot);
        UpdateSlotVisuals();
        UpdateShipmentButton();
    }

    public void UpdateDisassembleButton(InventoryItemSlot slot)
    {
        if (slot == null)
        {
            disassembleButton.SetActive(false);
            return;
        }

        switch (currentMode)
        {
            case InventoryMode.Normal:
                disassembleButton.SetActive(
                    slot.GetInventoryItemType() == InventoryItemType.DreamCatcher
                );
                break;

            case InventoryMode.Shipment:
                disassembleButton.SetActive(false);
                break;
        }
    }

    public void UpdateShipmentButton()
    {
        switch (currentMode)
        {
            case InventoryMode.Normal:
                shipmentButton.SetActive(false);
                break;

            case InventoryMode.Shipment:
                shipmentButton.SetActive(true);
                break;
        }
    }

    void UpdateSlotVisuals()
    {
        foreach (var slot in inventoryManager.itemList)
        {
            var slotComp = slot.GetComponent<InventoryItemSlot>();

            if (!slot.activeSelf) continue;

            bool interactable = true;

            switch (currentMode)
            {
                case InventoryMode.Shipment:
                    if (slotComp.GetInventoryItemType() == InventoryItemType.Feather)
                    {
                        interactable = false; // 회색 처리
                    }
                    break;
            }

            slotComp.SetInteractable(interactable);
        }
    }
}

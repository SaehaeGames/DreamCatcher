using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryMode
{
    None,
    Normal,
    Delivery
}
public class InventoryUIController : MonoBehaviour
{
    [Header("[UI Object]")]
    public GameObject disassembleButton;
    public GameObject shipmentButton;

    [Space]
    [Header("[Managers]")]
    public GameObject managers;
    private InventoryManager inventoryManager;
    private DeliveryManager deliveryManager;

    [Space]
    [Header("[UIControllers]")]
    public DeliveryUIController deliveryUIController;

    private InventoryMode currentMode;

    private void Awake()
    {
        inventoryManager = managers.GetComponent<InventoryManager>();
        deliveryManager = managers.GetComponent<DeliveryManager>();
    }

    private void Start()
    {
        
    }

    public void OpenNormalInventory()
    {
        inventoryManager.OpenInventory();
        SetMode(InventoryMode.Normal);
    }

    public void OpenShipmentInventory()
    {
        inventoryManager.OpenInventory();
        SetMode(InventoryMode.Delivery);
    }

    public void DeliveryDreamCatcher()
    {
        InventoryItemSlot selectedSlot = inventoryManager.GetSelectedSlot();

        if (deliveryManager.HasEnoughDreamCatchers(selectedSlot.GetSlotDreamCatcherInventoryData()))
        {
            deliveryUIController.OpenCheckWin(selectedSlot.GetSlotDreamCatcherInventoryData());
            Debug.Log("DeliveryDreamCatcher-opencheckwin");
        }
        else
        {
            deliveryUIController.OpenObjectNotEnoughWin();
            Debug.Log("DeliveryDreamCatcher-openobjectnotenoughwin");
        }

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

            case InventoryMode.Delivery:
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

            case InventoryMode.Delivery:
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
                case InventoryMode.Delivery:
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

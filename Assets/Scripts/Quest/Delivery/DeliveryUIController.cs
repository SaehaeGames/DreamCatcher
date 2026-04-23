using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUIController : MonoBehaviour
{
    [Header("[UI Object(CheckWin)]")]
    public Text description;
    public Image dreamcatcherImg;

    [Header("[Managers]")]
    public GameObject managers;
    private DeliveryManager deliveryManager;

    private DreamCatcherInventoryData selectedDreamCatcherInventoryData;

    private void Awake()
    {
        deliveryManager = managers.GetComponent<DeliveryManager>();
    }

    public void OpenCheckWin(DreamCatcherInventoryData dreamCatcherInventoryData)
    {
        // 창 활성화
        this.gameObject.SetActive(true);
        this.transform.GetChild(0).gameObject.SetActive(true);

        // 선택 드림캐쳐 표시
        description.text = dreamCatcherInventoryData.GetDescription();
        dreamcatcherImg.sprite = null; // 나중에 수정

        // 선택한 드림캐쳐 저장
        deliveryManager.SetSelectedDreamCatcherInventoryData(dreamCatcherInventoryData);
    }

    public void OpenObjectNotEnoughWin()
    {
        this.gameObject.SetActive(true);
        this.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void OnClickDeliveryButton()
    {
        deliveryManager.TryDeliveryDreamCatcher();
    }
}

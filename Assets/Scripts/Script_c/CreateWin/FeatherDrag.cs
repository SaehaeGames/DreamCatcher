using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FeatherDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public int featherNum;
    private Vector3 mousePos;
    private Transform startParent;
    public Sprite[] beadsImg;
    private int itemcnt = 0;

    // 매니저
    private DCCheckManager DCManager;
    //public FeatherNumDataManager FNDManager;
    MyFeatherNumber featherData;

    // 제작시작여부 판단
    private MakingUIManager makingUiManager;

    private void OnEnable()
    {
        DCManager = GameObject.FindWithTag("CreateManager").gameObject.GetComponent<DCCheckManager>();
        featherData = GameManager.instance.loadFeatherData;
        makingUiManager = GameObject.FindGameObjectWithTag("CreateManager").GetComponent<MakingUIManager>();
        //FNDManager = GameObject.FindWithTag("GameManager").gameObject.GetComponent<FeatherNumDataManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //transform.SetAsFirstSibling();
        FeatherCntReset();
    }

    public void FeatherCntReset()
    {
        //InventoryInfo_Data _inventoryinfo_data = GameManager.instance.inventoryinfo_data;   //플레이어의 인벤토리 정보를 가져옴    
        itemcnt = featherData.featherList[this.featherNum].feather_number;   //깃털의 수를 가져옴
        //Debug.Log("featherNum("+this.featherNum+") : " + itemcnt + "-itemcnt(FeatherCntReset)");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 깃털 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        startParent = transform.parent;
        transform.SetParent(GameObject.FindGameObjectWithTag("UI Canvas").transform);
    }

    // 깃털 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 5f;
        gameObject.transform.position = mousePos;
    }

    // 깃털 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(startParent);
        transform.localPosition = Vector3.zero;
        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("Deco"))
        {
            itemcnt--;
            if (itemcnt < 1)
            {
                this.transform.parent.gameObject.SetActive(false);
            }
            else if (itemcnt == 1)
            {
                startParent.transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (itemcnt > 1)
            {
                startParent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "X" + itemcnt;
            }
            // 제작 시작 알림
            makingUiManager.StartMakingFeather();

            // 깃털 데이터 저장
            DCManager.UpdateFeather(eventData.pointerCurrentRaycast.gameObject.GetComponent<FeatherDrop>().featherNum, this.featherNum);
            // 해당장식 깃털 변경
            GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_MakingOrFeather();   //제작, 선긋기 효과음
            eventData.pointerCurrentRaycast.gameObject.transform.GetChild(3).GetComponent<Image>().sprite = this.gameObject.GetComponent<Image>().sprite;
            eventData.pointerCurrentRaycast.gameObject.transform.GetChild(3).gameObject.SetActive(true);
            eventData.pointerCurrentRaycast.gameObject.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = beadsImg[MakingUIManager.colorNumber];
            eventData.pointerCurrentRaycast.gameObject.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = beadsImg[MakingUIManager.colorNumber];

            Debug.Log("FeatherCntReset item : " + itemcnt);
        }
    }
}

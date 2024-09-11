using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BeadPoint : MonoBehaviour, IPointerDownHandler
{
    // 매니저
    private DCCheckManager DCManager;

    // 비즈 번호
    public int beadNum;

    // Start is called before the first frame update
    void Start()
    {
        DCManager = GameObject.FindWithTag("CreateManager").gameObject.GetComponent<DCCheckManager>();
        this.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    }

    // 비즈를 눌렀을 때
    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_MakingOrFeather();   //제작, 선긋기 효과음
        this.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        // 비즈 위치 비교
        DCManager.UpdateBead(beadNum);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenEffect : MonoBehaviour
{
    // 패널이 열릴 때, 닫힐 때 수행할 클래스(각 열리는 패널에 넣기)


    private void OnEnable()
    {
        //창 열리는 효과음 재생

        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 열리는 효과음
    }

    private void OnDisable()
    {
        //창 닫히는 효과음 재생

        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<EffectChange>().PlayEffect_OpenScene(); //창 닫히는 효과음
    }
}

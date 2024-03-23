using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectChange : MonoBehaviour
{
    //이펙트 오디오 소소의 오디오 클립을 바꾸는 클래스


    public AudioSource EFAudioSource;     //이펙트 오디오 소스
    public AudioClip[] EffectClips;     //이펙트 오디오 클립 배열

    void Start()
    {
        EFAudioSource = this.gameObject.GetComponent<AudioManager>().EFSound;   //이펙트 오디오 소스를 가져옴
    }

    public void PlayEffect_OpenScene()
    {
        //씬 변경 효과음을 실행시키는 함수(이후 가상함수나 다른 효율적인 방법 있나 고민)
        //씬, 창을 열고 이동하는 효과음
        EFAudioSource.PlayOneShot(EffectClips[1], 1f);
    }

    public void PlayEffect_ClearOrFinish()
    {
        //클리어, 완성, 완료 효과음
        EFAudioSource.PlayOneShot(EffectClips[2], 1f);
    }

    public void PlayEffect_MakingOrFeather()
    {
        //선긋기, 깃털 제작, 깃털 수확 효과음
        EFAudioSource.PlayOneShot(EffectClips[4], 1f);
    }

    public void PlayEffect_SelectFeed()
    {
        //먹이 두기 효과음
        EFAudioSource.PlayOneShot(EffectClips[5], 1f);
    }

    public void PlayEffect_BirdArrived()
    {
        //새 도착 효과음
        EFAudioSource.PlayOneShot(EffectClips[6], 1f);
    }
}

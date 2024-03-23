using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //오디오를 재생하는 스크립트

    //싱글톤 패턴을 사용하기 위한 전역 변수
    public static AudioManager instance;

    //타이틀 씬 배경음악 오디오를 가져올 변수
    public AudioSource BGSound;
    public AudioSource EFSound;

    void Awake()
    {
        // 게임 시작과 동시에 싱글톤 구성

        if (instance)     //싱글톤 변수 instance가 이미 있다면
        {
            DestroyImmediate(gameObject);   //삭제
            return;
        }

        instance = this;    //유일한 인스턴스
        DontDestroyOnLoad(gameObject);  //씬이 바뀌어도 계속 유지시킴
    }

    public void SetBGVolume(float num)
    {
        //배경 음악의 크기를 바꾸는 함수
        BGSound.volume = num;   
    }

    public void SetEffectVolume(float num)
    {
        //효과음의 크기를 바꾸는 함수
        EFSound.volume = num;
    }

    public void SetBGMute(bool onoff)
    {
        BGSound.mute = onoff;
    }

    public void SetEffectMute(bool onoff)
    {
        EFSound.mute = onoff;
    }
}

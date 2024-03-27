using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    //음향을 저장하고 불러오는 스크립트(음향을 조절하는 오브젝트에 넣는 스크립트)

    //배경음악, 효과음 조절 슬라이더 오브젝트를 가져올 변수
    public Slider BGSlider; //배경음악 슬라이더
    public Slider EFSlider; //효과음 슬라이더

    //오디오를 끄고 켤 토글
    public Toggle BGToggle;    //배경음악 토글
    public Toggle EffectToggle;    //효과음 토글

    //오디오 크기 값을 저장할 변수
    float BGVol;
    float EFVol;

    //토글 값을 저장할 변수
    bool BGMute;
    bool EffectMute;

    public void Start()
    {
        ResetAudio();
        Debug.Log("오디오 실행");
    }

    public void ResetAudio()
    {
        //json에 저장된 값을 가져옴
        PlayerDataContainer curPlayerData = GameManager.instance.loadPlayerData;

        BGVol = curPlayerData.dataList[3].dataNumber;
        EFVol = curPlayerData.dataList[4].dataNumber;
        BGMute = curPlayerData.dataList[5].dataNumber == 1 ? true : false;
        EffectMute = curPlayerData.dataList[6].dataNumber == 1 ? true : false;

        //저장된 값을 슬라이더, 토글에 반영함
        BGSlider.value = BGVol;
        EFSlider.value = EFVol;
        BGToggle.isOn = BGMute;
        EffectToggle.isOn = EffectMute;

        //슬라이더, 토글의 값을 타이틀씬 배경음악 오디오에 반영함
        AudioManager.instance.SetBGVolume(BGVol);
        AudioManager.instance.SetEffectVolume(EFVol);
        AudioManager.instance.SetBGMute(BGMute);
        AudioManager.instance.SetEffectMute(EffectMute);
    }

    public void BGSoundSlider()
    {
        //배경음악 슬라이더 값에 따라 배경 음악 크기가 변하는 함수

        BGVol = BGSlider.value;
        AudioManager.instance.SetBGVolume(BGVol);

        //SaveAudioSetting();
    }

    public void EFSoundSlider()
    {
        //효과음 슬라이더 값에 따라 효과음 크기가 변하는 함수

        EFVol = EFSlider.value;
        AudioManager.instance.SetEffectVolume(EFVol);

        //SaveAudioSetting();
    }

    public void BGSoundToggle()
    {
        //배경음악 토글 체크 여부에 따라 음소거 되는 함수

        BGMute = BGToggle.isOn;
        AudioManager.instance.SetBGMute(BGMute);

        SaveAudioSetting();
    }

    //효과음 음소거 함수
    public void EFSoundToggle()
    {
        //효과음 토글 체크 여부에 따라 음소거 되는 함수

        EffectMute = EffectToggle.isOn;
        AudioManager.instance.SetEffectMute(EffectMute);

        SaveAudioSetting();
    }

    public void SaveAudioSetting()
    {
        //오디오 설정을 저장하는 함수

        PlayerDataContainer saveData = GameManager.instance.loadPlayerData;
        saveData.dataList[3].dataNumber = BGVol;
        saveData.dataList[4].dataNumber = EFVol;
        saveData.dataList[5].dataNumber = BGMute == true ? 1 : 0;
        saveData.dataList[6].dataNumber = EffectMute == true ? 1 : 0;

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerDataJSON>().DataSaveText(saveData);   //변경사항 json으로 저장
        Debug.Log("오디오 저장됨");
    }
}

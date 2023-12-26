using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundCtrl : MonoBehaviour
{
    public AudioMixer Mixer;
    public Slider audioSlider;

    void Start()
    {
        MasterAudioControl();
         SFXAudioControl();
    }
    
    
    public void MasterAudioControl()
    {
        float sound = audioSlider.value;
        if (sound == -40f) Mixer.SetFloat("Master", -80);
        else Mixer.SetFloat("Master", sound);
        Debug.Log($"{sound}");
    }

    public void SFXAudioControl()
    {
        float sound = audioSlider.value;
        if (sound == -40f) Mixer.SetFloat("SFX", -80);
        else Mixer.SetFloat("SFX", sound);

    }

}

/*1. 오디오 슬라이더의 값을 갖고 와야지 함수가 실행이 된다.
2. 게임 시작할 때는 오디오 슬라이더의 값이 가져와지지 않는다.
3. 오디오 슬라이더의 값을 갖고 오기 위해서는 함수를 실행을 시켜야한다.(마우스클릭)
4. 게임 시작할 때 오디오 슬라이더의 값을 갖고 오기 위해서는 함수를 자동 실행 시켜야한다. 한번
5. 4번을 진행하기 위해서는 마우스 클릭하지 않고 함수를 실행하는 방법을 알아야한다. 가 돼 내생각으론-feat.중호오빠-*/

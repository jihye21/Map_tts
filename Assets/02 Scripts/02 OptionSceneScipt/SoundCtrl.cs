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

/*1. ����� �����̴��� ���� ���� �;��� �Լ��� ������ �ȴ�.
2. ���� ������ ���� ����� �����̴��� ���� ���������� �ʴ´�.
3. ����� �����̴��� ���� ���� ���� ���ؼ��� �Լ��� ������ ���Ѿ��Ѵ�.(���콺Ŭ��)
4. ���� ������ �� ����� �����̴��� ���� ���� ���� ���ؼ��� �Լ��� �ڵ� ���� ���Ѿ��Ѵ�. �ѹ�
5. 4���� �����ϱ� ���ؼ��� ���콺 Ŭ������ �ʰ� �Լ��� �����ϴ� ����� �˾ƾ��Ѵ�. �� �� ����������-feat.��ȣ����-*/

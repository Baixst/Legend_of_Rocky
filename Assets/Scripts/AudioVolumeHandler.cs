using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumeHandler : MonoBehaviour
{
    public AudioMixer mixer;
    public string audioMixerParameter;

    public void SetVolumeLevel(float sliderValue)
    {
        mixer.SetFloat(audioMixerParameter, Mathf.Log10 (sliderValue) * 20);
    }
}

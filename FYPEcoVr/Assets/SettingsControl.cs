using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class SettingsControl : MonoBehaviour
{
    public AudioSource musicSource;
    //public AudioListener audioListener;
    public AudioMixer audioMixer;

    //public GameObject musicSliderObj;
    //public GameObject globalSliderObj;
    public Slider musicSlider;
    public Slider globalSlider;
    // Start is called before the first frame update
    void Start()
    {
        musicSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        //musicSlider = musicSliderObj.GetComponent<Slider>();
        //globalSlider = globalSliderObj.GetComponent<Slider>();
        UpdateQuality(3);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateVolume()
    {
        print("update slider "+musicSource.volume+":"+musicSlider.value);
        musicSource.volume = musicSlider.value;
        audioMixer.SetFloat("MasterVolume",globalSlider.value);

    }
    
    public void UpdateQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

    }
}

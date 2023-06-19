using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider fxSlider;
    [SerializeField] private Slider ambientSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle muteToggle;


    public void SetMasterVolume()
    {
        audioMixer.SetFloat("masterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("masterVolume", masterSlider.value);
        muteToggle.isOn = false;
    }

    public void SetFXVolume()
    {
        audioMixer.SetFloat("fxVolume", fxSlider.value);
        PlayerPrefs.SetFloat("fxVolume", fxSlider.value);
        muteToggle.isOn = false;
    }

    public void SetAmbientVolume()
    {
        audioMixer.SetFloat("ambientVolume", ambientSlider.value);
        PlayerPrefs.SetFloat("ambientVolume", ambientSlider.value);
        muteToggle.isOn = false;
    }

    public void SetMusicVolume()
    {
        audioMixer.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        muteToggle.isOn = false;
    }

    public void MuteButton()
    {
        if (muteToggle.isOn)
        {
            audioMixer.SetFloat("masterVolume", -60);
            PlayerPrefs.SetInt("isMuted", 1);
        }

        else
        {
            audioMixer.SetFloat("masterVolume", masterSlider.value);
            PlayerPrefs.SetInt("isMuted", 0);
        }
    }

    private void Start()
    {
        LoadVolume();

        SetFXVolume();
        SetAmbientVolume();
        SetMusicVolume();

        if (muteToggle.isOn == false)
            SetMasterVolume();
        else
            audioMixer.SetFloat("masterVolume", -60);
    }

    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume", masterSlider.value);
        fxSlider.value = PlayerPrefs.GetFloat("fxVolume", fxSlider.value);
        ambientSlider.value = PlayerPrefs.GetFloat("ambientVolume", ambientSlider.value);
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", musicSlider.value);

        if (PlayerPrefs.GetInt("isMuted") == 0)
            muteToggle.isOn = false;
        else
            muteToggle.isOn = true;
    }
}

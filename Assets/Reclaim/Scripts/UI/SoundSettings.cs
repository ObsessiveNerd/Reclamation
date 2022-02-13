using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : EscapeableMono
{
    public TextMeshProUGUI MasterVolumePercentText;
    public TextMeshProUGUI SoundEffectsVolumePercentText;
    
    public Slider MasterVolumeSlider;
    public Slider SoundEffectsVolumeSlider;
    
    public AudioSource MusicVolume;
    public AudioSource SoundEffectsVolume;

    public GameObject UISource;

    void Start()
    {
        MasterVolumeSlider.onValueChanged.AddListener(SetVolume);
        SoundEffectsVolumeSlider.onValueChanged.AddListener(SetSoundEffectsVolume);

        if (MetaData.Data != null)
        { 
            MasterVolumeSlider.value = MetaData.Data.Volume;
            SoundEffectsVolumeSlider.value = MetaData.Data.SoundEffectsVolume;
        }
    }

    protected override void OnEnable() { }
    protected override void OnDisable() { }

    public void Open()
    {
        UISource.SetActive(true);
        UIManager.Push(this);
        transform.SetAsLastSibling();
    }

    public override void OnEscape()
    {
        UISource.SetActive(false);
        UIManager.ForcePop(this);
        base.OnEscape();
    }

    void SetVolume(float value)
    {
        //if (MasterVolume != null)
        {
            MasterVolumePercentText.text = (int)(value * 100) + "%";
            MusicVolume.volume = value;
            MetaData.Data.Volume = value;
            MetaData.Data.Save();
        }
    }

     void SetSoundEffectsVolume(float value)
    {
        //if (MasterVolume != null)
        {
            SoundEffectsVolumePercentText.text = (int)(value * 100) + "%";
            SoundEffectsVolume.volume = value;
            MetaData.Data.SoundEffectsVolume = value;
            MetaData.Data.Save();
        }
    }
}

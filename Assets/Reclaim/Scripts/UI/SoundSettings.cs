using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : EscapeableMono
{
    public TextMeshProUGUI VolumePercentText;
    public Slider VolumeSlider;
    public AudioSource MasterVolume;
    public GameObject UISource;

    void Start()
    {
        VolumeSlider.onValueChanged.AddListener(SetVolume);

        if (MetaData.Data != null)
            VolumeSlider.value = MetaData.Data.Volume;
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
            VolumePercentText.text = (int)(value * 100) + "%";
            MasterVolume.volume = value;
            MetaData.Data.Volume = value;
            MetaData.Data.Save();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplaySettings : EscapeableMono
{
    public TMP_Dropdown WindowType;
    public TMP_Dropdown Resolution;
    public GameObject UISource;

    // Start is called before the first frame update
    void Start()
    {
        WindowType.options.Clear();
        var screenModeNames = Enum.GetNames(typeof(FullScreenMode));
        foreach(var screenModeName in screenModeNames)
            WindowType.options.Add(new TMP_Dropdown.OptionData(screenModeName));
        WindowType.onValueChanged.AddListener(ChangeScreenMode);

        Resolution.options.Clear();
        foreach (var resolution in Screen.resolutions)
            Resolution.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
        Resolution.onValueChanged.AddListener(ChangeResolution);

        //WindowType.value = (int)MetaData.component.ScreenMode;
        //Resolution.value = MetaData.component.GameResolutionIndex;
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

    public void ChangeScreenMode(int newIndex)
    {
        Screen.fullScreenMode = (FullScreenMode)newIndex;
        //MetaData.component.ScreenMode = (FullScreenMode)newIndex;
        //MetaData.component.Save();
    }

    public void ChangeResolution(int newIndex)
    {
        Resolution reso = Screen.resolutions[newIndex];
        Screen.SetResolution(reso.width, reso.height, Screen.fullScreenMode);
        //MetaData.component.GameResolutionIndex = newIndex;
        //MetaData.component.Save();
    }
}

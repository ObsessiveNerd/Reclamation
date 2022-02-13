using System;
using System.IO;
using UnityEngine;

[Serializable]
public class MetaData
{
    public static string MetaDataPath
    {
        get { return $"{Application.streamingAssetsPath}/game.config"; }
    }

    public static MetaData m_MetaData;
    public static MetaData Data
    {
        get
        {
            if (m_MetaData == null)
            {
                if (File.Exists(MetaDataPath))
                    m_MetaData = JsonUtility.FromJson<MetaData>(File.ReadAllText(MetaDataPath));
                else
                    m_MetaData = new MetaData(Array.IndexOf(Screen.resolutions, Screen.currentResolution), FullScreenMode.ExclusiveFullScreen);
            }
            return m_MetaData;
        }
    }

    public int GameResolutionIndex;
    public FullScreenMode ScreenMode;
    public float Volume;
    public float SoundEffectsVolume;

    public MetaData(int resIndex, FullScreenMode screenMode, float volume = 0.6f, float soundEffectsVolume = 0.6f)
    {
        GameResolutionIndex = resIndex;
        ScreenMode = screenMode;
        Volume = volume;
        SoundEffectsVolume = soundEffectsVolume;
    }

    public void Save()
    {
        string data = JsonUtility.ToJson(this);
        File.WriteAllText(MetaDataPath, data);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicService : GameService
{
    AudioSource m_BackgroundMusicSource;
    AudioSource m_SoundEffectMusicSource;
    SoundSettings m_Settings;

    public void PlaySoundClip(GameObject soundOrigin, string clipPath)
    {
        if (m_SoundEffectMusicSource == null)
            m_SoundEffectMusicSource = GameObject.Find("SoundEffectsMusicSource").GetComponent<AudioSource>();

        var clip = Resources.Load<AudioClip>(clipPath);
        if (clip != null)
        {
            if (m_Settings == null)
                m_Settings = GameObject.FindObjectOfType<SoundSettings>();

            if (m_ActivePlayer == null || m_ActivePlayer.Value == null)
                return;

            if (!m_EntityToPointMap.ContainsKey(m_ActivePlayer.Value.ID))
                return;

            Point activePlayerPoint = m_EntityToPointMap[m_ActivePlayer.Value.ID];
            Point sourcePoint = Point.InvalidPoint;

            if (!m_EntityToPointMap.ContainsKey(soundOrigin.ID))
            {
                if (m_EntityToPreviousPointMap.ContainsKey(soundOrigin.ID))
                    sourcePoint = m_EntityToPreviousPointMap[soundOrigin.ID];
                Debug.LogError($"{soundOrigin.Name} is not something on the map so we don't know the audio source.");
                return;
            }
            else
               sourcePoint = m_EntityToPointMap[soundOrigin.ID];

            float distance = Point.Distance(activePlayerPoint, sourcePoint);
            float volume = 1.0f;
            if (distance > 5)
                volume = 0f;
            else
                volume = 1f - (distance / 5f);

            m_SoundEffectMusicSource.volume = Mathf.Min(m_Settings.SoundEffectsVolumeSlider.value, volume);

            m_SoundEffectMusicSource.PlayOneShot(clip);
        }
    }

    public void PlayBackgroundMusic(string clipPath)
    {
        if (m_BackgroundMusicSource == null)
            m_BackgroundMusicSource = GameObject.Find("BackgroundMusicSource").GetComponent<AudioSource>();

        var clip = Resources.Load<AudioClip>(clipPath);
        if (clip != null)
        { 
            m_BackgroundMusicSource.clip = clip;
            m_BackgroundMusicSource.Play();
        }
    }
}

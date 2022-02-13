using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicService : GameService
{
    AudioSource m_BackgroundMusicSource;
    AudioSource m_SoundEffectMusicSource;

    public void PlaySoundClip(string clipPath)
    {
        if (m_SoundEffectMusicSource == null)
            m_SoundEffectMusicSource = GameObject.Find("SoundEffectsMusicSource").GetComponent<AudioSource>();

        var clip = Resources.Load<AudioClip>(clipPath);
        if (clip != null)
            m_SoundEffectMusicSource.PlayOneShot(clip);
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

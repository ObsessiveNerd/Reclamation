using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : EntityComponentBehavior
{
    public AudioClip SoundClip;
    public string Key;

    //public override void WakeUp(IComponent data = null)
    //{
    //    RegisteredEvents.Add(GameEventId.Playsound, PlaySound);
    //}

    void PlaySound(GameEvent gameEvent)
    {
        string key = gameEvent.GetValue<string>(EventParameter.Key);
        //if (key == Key)
            //Services.Music.PlaySoundClip(gameEvent.GetValue<GameObject>(EventParameter.SoundSource), SoundClip);
    }
}
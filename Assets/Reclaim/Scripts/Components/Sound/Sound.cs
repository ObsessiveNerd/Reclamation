using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundKey
{
    public static string WeaponAttack = nameof(WeaponAttack);
    public static string Cast = nameof(Cast);
    public static string AttackHit = nameof(AttackHit);
    public static string AttackMiss = nameof(AttackMiss);
    public static string Activate = nameof(Activate);
}

[Serializable]
public class SoundPath
{
    [SerializeField]
    public string Path;

    public SoundPath(string path)
    {
        Path = path;
    }

    public override string ToString()
    {
        return Path;
    }
}

public class Sound : EntityComponent
{
    public SoundPath SoundPath;
    public string Key;

    public Sound(string path, string key)
    {
        SoundPath = new SoundPath(path);
        Key = key;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.Playsound);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Playsound)
        {
            string key = gameEvent.GetValue<string>(EventParameters.Key);
            if(key == Key)
                Services.Music.PlaySoundClip(SoundPath.Path);
        }
    }
}

public class DTO_Sound : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string key = "";
        string path = "";

        string[] kvpairs = data.Split(',');
        foreach(var kvp in kvpairs)
        {
            string passedKey = kvp.Split('=')[0];
            string passedValue = kvp.Split('=')[1];

            switch(passedKey)
            {
                case "Key":
                    key = passedValue;
                    break;
                case "SoundPath":
                    path = passedValue;
                    break;
            }
        }

        Component = new Sound(path, key);
    }

    public string CreateSerializableData(IComponent component)
    {
        Sound s = (Sound)component;
        return $"{nameof(Sound)}: {nameof(s.Key)}={s.Key}, {nameof(s.SoundPath)}={s.SoundPath.Path}";
    }
}
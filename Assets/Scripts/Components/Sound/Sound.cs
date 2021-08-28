using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundKey
{
    public static string WeaponAttack = nameof(WeaponAttack);
    public static string CastSpell = nameof(CastSpell);
}

public class Sound : Component
{
    public string SoundPath;
    public string Key;

    public Sound(string path, string key)
    {
        SoundPath = path;
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
            {
                var clip = Resources.Load<AudioClip>(SoundPath);
                GameObject.FindObjectOfType<AudioSource>()?.PlayOneShot(clip);
            }
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
        return $"{nameof(Sound)}: {nameof(s.Key)}={s.Key}, {nameof(s.SoundPath)}={s.SoundPath}";
    }
}
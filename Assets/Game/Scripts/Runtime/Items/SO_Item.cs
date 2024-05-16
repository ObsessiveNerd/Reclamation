using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedItem
{
    public string Name;
    public string FlavorText;
    [HideInInspector]
    public string SpriteName;
    [HideInInspector]
    public string IconName;
    public Color SpriteColor;
    public int MaxStack;
}

public abstract class SO_Item : ScriptableObject
{
    public Sprite Icon;
    public Sprite Sprite;
    public virtual string GetDescription() { return ""; }

    public virtual string Serialize()
    {
        return string.Empty;
    }

    public virtual void Deserialize(string json) { }

    public virtual SerializedItem GetSerializedItem() { return null; }
}

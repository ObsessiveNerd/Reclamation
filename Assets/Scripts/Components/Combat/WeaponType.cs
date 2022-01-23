﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    None,
    Melee,
    Ranged,
    RangedSpell,
    Finesse
    
}

public enum SpellType
{
    None,
    StrSpell,
    AgiSpell,
    ConSpell,
    WisSpell,
    IntSpell,
    ChaSpell
}

public class WeaponType : Component
{
    public AttackType Type;

    public WeaponType(AttackType type)
    {
        Type = type;
        RegisteredEvents.Add(GameEventId.GetWeapon);
        RegisteredEvents.Add(GameEventId.GetWeaponType);
        RegisteredEvents.Add(GameEventId.GetWeaponTypes);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetWeaponType)
            gameEvent.Paramters[EventParameters.WeaponType] = Type;
        else if (gameEvent.ID == GameEventId.GetWeaponTypes)
            gameEvent.GetValue<List<AttackType>>(EventParameters.WeaponTypeList).Add(Type);
        else if (gameEvent.ID == GameEventId.GetWeapon)
            gameEvent.GetValue<List<string>>(EventParameters.Weapon).Add(Self.ID);
    }
}

public class DTO_WeaponType : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data;
        if (data.Contains("="))
            value = data.Split('=')[1];

        AttackType tw = (AttackType)Enum.Parse(typeof(AttackType), value);
        Component = new WeaponType(tw);
    }

    public string CreateSerializableData(IComponent component)
    {
        WeaponType wt = (WeaponType)component;
        return $"{nameof(WeaponType)}:{nameof(wt.Type)}={wt.Type}";
    }
}
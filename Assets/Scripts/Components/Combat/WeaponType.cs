using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeWeapon
{
    None,
    Melee,
    Ranged,
    Wand,
    MagicStaff,
    Finesse,
    StrSpell,
    AgiSpell,
    ConSpell,
    WisSpell,
    IntSpell,
    ChaSpell
}

public class WeaponType : Component
{
    TypeWeapon m_TypeWeapon;

    public WeaponType(TypeWeapon type)
    {
        m_TypeWeapon = type;
        RegisteredEvents.Add(GameEventId.GetWeaponType);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.WeaponType] = m_TypeWeapon;
    }
}

public class DTO_WeaponType : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        TypeWeapon tw = (TypeWeapon)Enum.Parse(typeof(TypeWeapon), data);
        Component = new WeaponType(tw);
    }
}
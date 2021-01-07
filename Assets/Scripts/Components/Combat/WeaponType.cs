using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeWeapon
{
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

    public WeaponType(IEntity self, TypeWeapon type)
    {
        Init(self);
        m_TypeWeapon = type;

        RegisteredEvents.Add(GameEventId.GetWeaponType);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.WeaponType] = m_TypeWeapon;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    None,
    Melee,
    Ranged,
    RangedSpell,
    Finesse,
    Spell
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

public class WeaponType : EntityComponent
{
    public AttackType Type;

    public void Start()
    {
        //RegisteredEvents.Add(GameEventId.GetWeapon);
        //RegisteredEvents.Add(GameEventId.GetWeaponType);
    }

    //public override void HandleEvent(GameEvent gameEvent)
    //{
    //    if (gameEvent.ID == GameEventId.GetWeaponType)
    //        gameEvent.GetValue<List<AttackType>>(EventParameter.WeaponType).Add(Type);
    //    else if (gameEvent.ID == GameEventId.GetWeapon)
    //        gameEvent.GetValue<List<string>>(EventParameter.Weapon).Add(Self.ID);
    //}
}
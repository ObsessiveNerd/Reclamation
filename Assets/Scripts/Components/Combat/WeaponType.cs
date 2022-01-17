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
    public TypeWeapon TypeWeapon;

    public WeaponType(TypeWeapon type)
    {
        TypeWeapon = type;
        RegisteredEvents.Add(GameEventId.GetWeapon);
        RegisteredEvents.Add(GameEventId.GetWeaponType);
        RegisteredEvents.Add(GameEventId.GetWeaponTypes);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetWeaponType)
            gameEvent.Paramters[EventParameters.WeaponType] = TypeWeapon;
        else if (gameEvent.ID == GameEventId.GetWeaponTypes)
            gameEvent.GetValue<List<TypeWeapon>>(EventParameters.WeaponTypeList).Add(TypeWeapon);
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

        TypeWeapon tw = (TypeWeapon)Enum.Parse(typeof(TypeWeapon), value);
        Component = new WeaponType(tw);
    }

    public string CreateSerializableData(IComponent component)
    {
        WeaponType wt = (WeaponType)component;
        return $"{nameof(WeaponType)}:{wt.TypeWeapon}";
    }
}
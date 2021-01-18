using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : Component
{
    public DamageType DamageType;
    public Dice Dice;

    public DealDamage(DamageType damageType, Dice dice)
    {
        DamageType = damageType;
        Dice = dice;

        RegisteredEvents.Add(GameEventId.AmAttacking);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        ((List<Damage>)gameEvent.Paramters[EventParameters.DamageList]).Add(new Damage(Dice.Roll(), DamageType));
    }
}

public class DTO_DealDamage : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string damageDice = null;
        DamageType type = 0;

        string[] parameters = data.Split(',');
        foreach(string parameter in parameters)
        {
            string[] value = parameter.Split('=');
            switch(value[0])
            {
                case "DamageType":
                    type = (DamageType)Enum.Parse(typeof(DamageType), value[1]);
                    break;
                case "Damage":
                    damageDice = value[1];
                    break;
            }
        }
        Component = new DealDamage(type, new Dice(damageDice));
    }

    public string CreateSerializableData(IComponent component)
    {
        DealDamage dd = (DealDamage)component;
        return $"{nameof(DealDamage)}: DamageType={dd.DamageType}, Damage={dd.Dice.GetNotation()}";
    }
}

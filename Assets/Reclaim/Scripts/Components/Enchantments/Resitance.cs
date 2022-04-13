using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resitance : EntityComponent
{
    public DamageType Type;
    public Resitance(DamageType type)
    {
        Type = type;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetResistances);
        RegisteredEvents.Add(GameEventId.TakeDamage);
        RegisteredEvents.Add(GameEventId.GetInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetResistances)
            gameEvent.GetValue<List<DamageType>>(EventParameters.Resistances).Add(Type);
        else if(gameEvent.ID == GameEventId.TakeDamage)
        {
            foreach(var damage in gameEvent.GetValue<List<Damage>>(EventParameters.DamageList))
            {
                if (damage.DamageType == Type)
                    damage.DamageAmount = damage.DamageAmount / 2;
            }
        }
        else if(gameEvent.ID == GameEventId.GetInfo)
        {
            var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameters.Info);
            dictionary.Add($"{nameof(Resitance)}{Guid.NewGuid()}", $"{Type.ToString()} Resistance");
        }
    }
}

public class DTO_Resitance : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string damageType = data.Split('=')[1];
        DamageType t = (DamageType)Enum.Parse(typeof(DamageType), damageType);
        Component = new Resitance(t);
    }

    public string CreateSerializableData(IComponent component)
    {
        Resitance res = (Resitance)component;
        return $"{nameof(Resitance)}: {nameof(res.Type)}={res.Type}";
    }
}

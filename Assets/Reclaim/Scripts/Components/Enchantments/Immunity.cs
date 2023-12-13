using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Immunity : EntityComponent
{
    public DamageType Type;
    public Immunity(DamageType type)
    {
        Type = type;
    }

    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetImmunity);
        RegisteredEvents.Add(GameEventId.TakeDamage);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetImmunity)
            gameEvent.GetValue<List<DamageType>>(EventParameter.Immunity).Add(Type);
        else if(gameEvent.ID == GameEventId.TakeDamage)
        {
            foreach(var damage in gameEvent.GetValue<List<Damage>>(EventParameter.DamageList))
            {
                if (damage.DamageType == Type)
                    damage.DamageAmount = 0;
            }
        }
    }
}

public class DTO_Immunity : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string damageType = data.Split('=')[1];
        DamageType t = (DamageType)Enum.Parse(typeof(DamageType), damageType);
        Component = new Immunity(t);
    }

    public string CreateSerializableData(IComponent component)
    {
        Immunity im = (Immunity)component;
        return $"{nameof(Immunity)}: {nameof(im.Type)}={im.Type}";
    }
}

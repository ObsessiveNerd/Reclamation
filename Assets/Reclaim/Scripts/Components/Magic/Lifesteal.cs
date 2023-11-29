using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifesteal : EntityComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.DealtDamage);
        RegisteredEvents.Add(GameEventId.GetInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.DealtDamage)
        {
            IEntity damageSource = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.DamageSource));
            Damage damage = gameEvent.GetValue<Damage>(EventParameter.Damage);

            GameEvent healForDamage = GameEventPool.Get(GameEventId.RestoreHealth)
                                        .With(EventParameter.Healing, damage.DamageAmount);

            damageSource.FireEvent(healForDamage).Release();
        }

        else if (gameEvent.ID == GameEventId.GetInfo)
        {
            var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
            dictionary.Add($"{nameof(Lifesteal)}{Guid.NewGuid()}", "Lifesteal: Damage dealt by this returns health to the attacker.");
        }
    }
}

public class DTO_Lifesteal : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Lifesteal();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Lifesteal);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : EntityComponent
{
    public Dice HealAmount;

    public Heal(Dice healAmount)
    {
        HealAmount = healAmount;
    }

    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.ApplyEffectToTarget);
        RegisteredEvents.Add(GameEventId.CastSpellEffect);
        RegisteredEvents.Add(GameEventId.GetInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.ApplyEffectToTarget)
        {
            GameObject target = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
            GameEvent e = GameEventPool.Get(GameEventId.RestoreHealth)
                                .With(EventParameter.Healing, HealAmount.Roll());
            target.FireEvent(e).Release();
        }
        else if (gameEvent.ID == GameEventId.CastSpellEffect)
        {
            GameObject target = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Target));
            GameEvent e = GameEventPool.Get(GameEventId.RestoreHealth)
                                .With(EventParameter.Healing, HealAmount.Roll());
            target.FireEvent(e).Release();
        }
        else if (gameEvent.ID == GameEventId.GetInfo)
        {
            var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
            dictionary.Add($"{nameof(Heal)}{Guid.NewGuid()}", $"Heals for: {HealAmount}");
        }
    }
}

public class DTO_Heal : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Dice dice = new Dice(data.Split('=')[1]);
        Component = new Heal(dice);
    }

    public string CreateSerializableData(IComponent component)
    {
        Heal h = (Heal)component;
        return $"{nameof(Heal)}: {nameof(h.HealAmount)}={h.HealAmount}";
    }
}

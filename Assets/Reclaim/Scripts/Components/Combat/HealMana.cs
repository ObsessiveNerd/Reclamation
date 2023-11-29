using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealMana : EntityComponent
{
    public Dice HealAmount;

    public HealMana(Dice healAmount)
    {
        HealAmount = healAmount;
    }

    public override void Init(IEntity self)
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
            IEntity target = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
            GameEvent e = GameEventPool.Get(GameEventId.RestoreMana)
                                .With(EventParameter.Mana, HealAmount.Roll());
            target.FireEvent(e).Release();
        }
        else if (gameEvent.ID == GameEventId.CastSpellEffect)
        {
            IEntity target = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Target));
            GameEvent e = GameEventPool.Get(GameEventId.RestoreMana)
                                .With(EventParameter.Healing, HealAmount.Roll());
            target.FireEvent(e).Release();
        }
        else if (gameEvent.ID == GameEventId.GetInfo)
        {
            var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
            dictionary.Add($"{nameof(HealMana)}{Guid.NewGuid()}", $"Heals for: {HealAmount}");
        }
    }
}

public class DTO_HealMana : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Dice dice = new Dice(data.Split('=')[1]);
        Component = new HealMana(dice);
    }

    public string CreateSerializableData(IComponent component)
    {
        HealMana h = (HealMana)component;
        return $"{nameof(HealMana)}: {nameof(h.HealAmount)}={h.HealAmount}";
    }
}

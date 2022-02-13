﻿using System;
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

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.ApplyEffectToTarget);
        RegisteredEvents.Add(GameEventId.GetInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.ApplyEffectToTarget)
        {
            IEntity target = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            GameEvent e = GameEventPool.Get(GameEventId.RestoreHealth)
                                .With(EventParameters.Healing, HealAmount.Roll());
            target.FireEvent(e).Release();
        }
        else if (gameEvent.ID == GameEventId.GetInfo)
        {
            var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameters.Info);
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

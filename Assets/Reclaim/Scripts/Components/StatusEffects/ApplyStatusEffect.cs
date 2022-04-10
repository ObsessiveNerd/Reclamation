using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusEffect : EntityComponent
{
    public StatusEffects StatusEffectType;
    public int DestroyAfterTurns;

    public ApplyStatusEffect(StatusEffects se, int destroyAfterTurns)
    {
        StatusEffectType = se;
        DestroyAfterTurns = destroyAfterTurns;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.ApplyEffectToTarget);
        RegisteredEvents.Add(GameEventId.CastSpellEffect);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.ApplyEffectToTarget)
        {
            var target = gameEvent.GetValue<string>(EventParameters.Entity);
            ApplyEffectToTarget(Services.EntityMapService.GetEntity(target));
        }
        else if(gameEvent.ID == GameEventId.CastSpellEffect)
        {
            var target = gameEvent.GetValue<string>(EventParameters.Target);
            ApplyEffectToTarget(Services.EntityMapService.GetEntity(target));
        }
    }

    void ApplyEffectToTarget(IEntity target)
    {
        target.AddComponent(new StatusEffect(StatusEffectType, 0, DestroyAfterTurns));
    }
}

public class DTO_ApplyStatusEffect : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        StatusEffects se = StatusEffects.None;
        int destroyAfterTurns = 0;

        string[] kvp = data.Split(',');
        foreach(var kvpair in kvp)
        {
            string key = kvpair.Split('=')[0];
            string value = kvpair.Split('=')[1];

            switch (key)
            {
                case "StatusEffectType":
                    se = (StatusEffects)Enum.Parse(typeof(StatusEffects), value);
                    break;
                case "DestroyAfterTurns":
                    destroyAfterTurns = int.Parse(value);
                    break;
            }
        }
        Component = new ApplyStatusEffect(se, destroyAfterTurns);
    }

    public string CreateSerializableData(IComponent component)
    {
        ApplyStatusEffect ase = (ApplyStatusEffect)component;
        return $"{nameof(ApplyStatusEffect)}: {nameof(ase.StatusEffectType)}={ase.StatusEffectType}, {nameof(ase.DestroyAfterTurns)}={ase.DestroyAfterTurns}";
    }
}

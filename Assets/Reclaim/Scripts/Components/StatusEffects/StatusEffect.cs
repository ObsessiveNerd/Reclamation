using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffects
{
    None = 0,
    Rage,
    Poison
}

public class StatusEffect : EntityComponent
{
    public int CurrentTurnCount;
    public int DestroyAfterTurnCount;

    public StatusEffects StatusEffectType;

    IComponent m_StatusComponent;

    public StatusEffect(StatusEffects type, int currentTurn, int destroyAfterTurns)
    {
        StatusEffectType = type;
        CurrentTurnCount = currentTurn;
        DestroyAfterTurnCount = destroyAfterTurns;
    }

    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.EndTurn);

        switch(StatusEffectType)
        {
            case StatusEffects.Rage:
                m_StatusComponent = new Rage();
                break;
            case StatusEffects.Poison:
                m_StatusComponent = new Poison();
                break;
        }

        Self.AddComponent(m_StatusComponent);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.EndTurn)
        {
            CurrentTurnCount++;

            if (CurrentTurnCount > DestroyAfterTurnCount)
            {
                Self.RemoveComponent(m_StatusComponent);
                Self.RemoveComponent(this);
            }
        }
    }
}

public class DTO_StatusEffect : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        StatusEffects effectType = StatusEffects.None;
        int currentturn = 0;
        int destroyafterturns = 0;

        string[] kvpairs = data.Split(',');
        foreach(var keyvaluepair in kvpairs)
        {
            string key = keyvaluepair.Split('=')[0];
            string value = keyvaluepair.Split('=')[1];

            switch (key)
            {
                case "StatusEffect":
                    effectType = (StatusEffects)Enum.Parse(typeof(StatusEffects), value);
                    break;
                case "DestroyAfterTurnCount":
                    destroyafterturns = int.Parse(value);
                    break;
                case "CurrentTurnCount":
                    currentturn = int.Parse(value);
                    break;
            }
        }
        Component = new StatusEffect(effectType, currentturn, destroyafterturns);
    }

    public string CreateSerializableData(IComponent component)
    {
        StatusEffect bat = (StatusEffect)component;
        return $"{nameof(StatusEffect)}: {nameof(bat.StatusEffectType)}={bat.StatusEffectType}, {nameof(bat.DestroyAfterTurnCount)}={bat.DestroyAfterTurnCount}, {nameof(bat.CurrentTurnCount)}={bat.CurrentTurnCount}";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAfterTurns : EntityComponent
{
    public int CurrentTurnCount;
    public int DestroyAfterTurnCount;

    public Stat StatType;
    public int Amount;

    public BuffAfterTurns(Stat type, int amount, int currentTurn, int destroyAfterTurns)
    {
        StatType = type;
        Amount = amount;
        CurrentTurnCount = currentTurn;
        DestroyAfterTurnCount = destroyAfterTurns;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.EndTurn);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.EndTurn)
        {
            CurrentTurnCount++;

            if (CurrentTurnCount >= DestroyAfterTurnCount)
            {
                GameEvent buff = GameEventPool.Get(GameEventId.BoostStat)
                                .With(EventParameter.StatType, StatType)
                                .With(EventParameter.Cost, false)
                                .With(EventParameter.StatBoostAmount, Amount);

                Self.FireEvent(buff);
                Self.RemoveComponent(this);
            }
        }
    }
}

public class DTO_BuffAfterTurns : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Stat stattype = Stat.Str;
        int amount = 0;
        int currentturn = 0;
        int destroyafterturns = 0;

        string[] kvpairs = data.Split(',');
        foreach(var keyvaluepair in kvpairs)
        {
            string key = keyvaluepair.Split('=')[0];
            string value = keyvaluepair.Split('=')[1];

            switch (key)
            {
                case "StatType":
                    stattype = (Stat)Enum.Parse(typeof(Stat), value);
                    break;
                case "Amount":
                    amount = int.Parse(value);
                    break;
                case "DestroyAfterTurnCount":
                    destroyafterturns = int.Parse(value);
                    break;
                case "CurrentTurnCount":
                    currentturn = int.Parse(value);
                    break;
            }
        }
        Component = new BuffAfterTurns(stattype, amount, currentturn, destroyafterturns);
    }

    public string CreateSerializableData(IComponent component)
    {
        BuffAfterTurns bat = (BuffAfterTurns)component;
        return $"{nameof(BuffAfterTurns)}: {nameof(bat.StatType)}={bat.StatType}, {nameof(bat.Amount)}={bat.Amount}, {nameof(bat.DestroyAfterTurnCount)}={bat.DestroyAfterTurnCount}, {nameof(bat.CurrentTurnCount)}={bat.CurrentTurnCount}";
    }
}

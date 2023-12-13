using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuff : EntityComponent
{
    public Stat StatToBuff;
    public int Amount;
    public int RemoveAfterTurns;

    public StatBuff(Stat stat, int amount, int removeAfterTurns)
    {
        StatToBuff = stat;
        Amount = amount;
        RemoveAfterTurns = removeAfterTurns;
    }

    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.CastSpellEffect);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.CastSpellEffect)
        {
            GameObject target = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameter.Target));
            GameEvent getStat = GameEventPool.Get(GameEventId.GetStatRaw)
                                .With(EventParameter.StatType, StatToBuff)
                                .With(EventParameter.Value, Stat.Str);

            int value = target.FireEvent(getStat).GetValue<int>(EventParameter.Value);
            float percent = Amount / 100f;
            int amountToBuff = (int)(value * percent);

            GameEvent boostStat = GameEventPool.Get(GameEventId.BoostStat)
                                    .With(EventParameter.StatType, StatToBuff)
                                    .With(EventParameter.Cost, false)
                                    .With(EventParameter.StatBoostAmount, amountToBuff);

            target.FireEvent(boostStat);
            boostStat.Release();

            if(RemoveAfterTurns > 0)
                target.AddComponent(new BuffAfterTurns(StatToBuff, -1 * amountToBuff, 0, RemoveAfterTurns));
                                
        }
    }
}

public class DTO_StatBuff : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Stat stat = Stat.Str;
        int amount = 0;
        int removeAfterTurns = 0;

        string[] kvp = data.Split(',');
        foreach(var kvpair in kvp)
        {
            string key = kvpair.Split('=')[0];
            string value = kvpair.Split('=')[1];

            switch(key)
            {
                case "StatToBuff":
                    stat = (Stat)Enum.Parse(typeof(Stat), value);
                    break;
                case "Amount":
                    amount = int.Parse(value);
                    break;
                case "RemoveAfterTurns":
                    removeAfterTurns = int.Parse(value);
                    break;
            }
        }
        Component = new StatBuff(stat, amount, removeAfterTurns);
    }

    public string CreateSerializableData(IComponent component)
    {
        StatBuff sb = (StatBuff)component;
        return $"{nameof(StatBuff)}: {nameof(sb.StatToBuff)}={sb.StatToBuff}, {nameof(sb.Amount)}={sb.Amount}, {nameof(sb.RemoveAfterTurns)}={sb.RemoveAfterTurns}";
    }
}

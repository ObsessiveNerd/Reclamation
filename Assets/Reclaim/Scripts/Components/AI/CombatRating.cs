using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRating : EntityComponent
{
    public override void Init(GameObject self)
    {
        base.Init(self);
        //RegisteredEvents.Add(GameEventId.GetCombatRating);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetCombatRating)
        {
            GameEvent getPrimaryStat = GameEventPool.Get(GameEventId.GetPrimaryStatType)
                                            .With(EventParameter.Value, null);
            Stat primaryType = FireEvent(Self, getPrimaryStat).GetValue<Stat>(EventParameter.Value);
            getPrimaryStat.Release();

            int primaryStatMod = GetStatMod(primaryType);
            int conStatMod = GetStatMod(Stat.Con);

            gameEvent.Paramters[EventParameter.Value] = ((int)gameEvent.Paramters[EventParameter.Value] + primaryStatMod + conStatMod);
        }
    }

    int GetStatMod(Stat statType)
    {
        GameEvent getPrimaryStatModifier = GameEventPool.Get(GameEventId.GetStat)
                                                    .With(EventParameter.StatType, statType)
                                                    .With(EventParameter.Value, 0);
        var res = FireEvent(Self, getPrimaryStatModifier).GetValue<int>(EventParameter.Value);
        getPrimaryStatModifier.Release();
        return res;
    }
}

public class DTO_CombatRating : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new CombatRating();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(CombatRating);
    }
}
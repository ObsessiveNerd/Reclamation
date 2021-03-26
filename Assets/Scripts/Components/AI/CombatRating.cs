using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRating : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetCombatRating);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetCombatRating)
        {
            EventBuilder getPrimaryStat = new EventBuilder(GameEventId.GetPrimaryStatType)
                                            .With(EventParameters.Value, null);
            Stat primaryType = FireEvent(Self, getPrimaryStat.CreateEvent()).GetValue<Stat>(EventParameters.Value);

            int primaryStatMod = GetStatMod(primaryType);
            int conStatMod = GetStatMod(Stat.Con);

            gameEvent.Paramters[EventParameters.Value] = ((int)gameEvent.Paramters[EventParameters.Value] + primaryStatMod + conStatMod);
        }
    }

    int GetStatMod(Stat statType)
    {
        EventBuilder getPrimaryStatModifier = new EventBuilder(GameEventId.GetStat)
                                                    .With(EventParameters.StatType, statType)
                                                    .With(EventParameters.Value, 0);
        return FireEvent(Self, getPrimaryStatModifier.CreateEvent()).GetValue<int>(EventParameters.Value);
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
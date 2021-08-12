using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Component
{
    public int HealAmount;

    public Heal(int healAmount)
    {
        HealAmount = healAmount;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.ApplyEffectToTarget);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.ApplyEffectToTarget)
        {
            IEntity target = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            EventBuilder e = new EventBuilder(GameEventId.RestoreHealth)
                                .With(EventParameters.Healing, HealAmount);
            target.FireEvent(e.CreateEvent());
        }
    }
}

public class DTO_Heal : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        var value = int.Parse(data.Split('=')[1]);
        Component = new Heal(value);
    }

    public string CreateSerializableData(IComponent component)
    {
        Heal h = (Heal)component;
        return $"{nameof(Heal)}: {nameof(h.HealAmount)}={h.HealAmount}";
    }
}

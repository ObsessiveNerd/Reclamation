using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnDeath : Component
{
    public string EventId;
    public TriggerOnDeath(string eventId)
    {
        EventId = eventId;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.Died);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Died)
        {
            EventBuilder eb = new EventBuilder(EventId)
                                .With(EventParameters.Entity, Self.ID);
            FireEvent(World.Instance.Self, eb.CreateEvent());
        }
    }
}

public class DTO_TriggerOnDeath : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new TriggerOnDeath(data);
    }

    public string CreateSerializableData(IComponent component)
    {
        TriggerOnDeath tod = (TriggerOnDeath)component;
        return $"{nameof(TriggerOnDeath)}: {tod.EventId}";
    }
}

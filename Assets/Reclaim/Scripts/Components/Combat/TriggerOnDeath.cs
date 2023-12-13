using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnDeath : EntityComponent
{
    public string EventId;
    public TriggerOnDeath(string eventId)
    {
        EventId = eventId;
    }

    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.Died);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Died)
        {
            throw new Exception("Need to fix win condition");

            //GameEvent eb = GameEventPool.Get(EventId)
            //                    .With(EventParameters.Entity, Self.ID);
            //FireEvent(World.Services.Self, eb).Release();
        }
    }
}

public class DTO_TriggerOnDeath : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data.Split('=')[1];
        Component = new TriggerOnDeath(value);
    }

    public string CreateSerializableData(IComponent component)
    {
        TriggerOnDeath tod = (TriggerOnDeath)component;
        return $"{nameof(TriggerOnDeath)}: {nameof(tod.EventId)}={tod.EventId}";
    }
}

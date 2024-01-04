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
        
        RegisteredEvents.Add(GameEventId.Died, Died);
    }

    void Died(GameEvent gameEvent)
    {

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IComponent
{
    Dictionary<GameEventId, Action<GameEvent>> RegisteredEvents { get; }
    GameEvent FireEvent(GameObject target, GameEvent gameEvent, bool logEvent = false);
    void HandleEvent(GameEvent gameEvent);
}

public class EntityComponent : NetworkBehaviour, IComponent
{
    private Dictionary<GameEventId, Action<GameEvent>> m_RegisteredEvents;
    public Dictionary<GameEventId, Action<GameEvent>> RegisteredEvents
    {
        get
        {
            if (m_RegisteredEvents == null)
                m_RegisteredEvents = new Dictionary<GameEventId, Action<GameEvent>>();
            return m_RegisteredEvents;
        }
    }

    public virtual void WakeUp() { }

    public void HandleEvent(GameEvent gameEvent) 
    {
        if (RegisteredEvents.ContainsKey(gameEvent.ID))
            RegisteredEvents[gameEvent.ID](gameEvent);
    }

    public GameEvent FireEvent(GameObject target, GameEvent gameEvent, bool logEvent = false)
    {
        return target.FireEvent(gameEvent, logEvent);
    }
}


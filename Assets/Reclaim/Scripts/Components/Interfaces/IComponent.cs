using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IComponent
{
    void WakeUp(IComponentData data = null);
    Dictionary<GameEventId, Action<GameEvent>> RegisteredEvents { get; }
    GameEvent FireEvent(GameObject target, GameEvent gameEvent, bool logEvent = false);
    void HandleEvent(GameEvent gameEvent);
}

public interface IComponentData
{
    Dictionary<GameEventId, Action<GameEvent>> RegisteredEvents { get; }
    //GameEvent FireEvent(GameObject target, GameEvent gameEvent, bool logEvent = false);
    void HandleEvent(GameEvent gameEvent);
    void WakeUp();
}

[Serializable]
public class ComponentData : IComponentData
{
    public EntityData Entity;

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
}

//public class EntityComponentBehavior : NetworkBehaviour, IComponentBehavior
//{
//    public IComponent ComponentData;
//}

public class EntityComponent : NetworkBehaviour, IComponent
{
    public virtual IComponentData GetData() { return null; }

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

    public virtual void WakeUp(IComponentData data = null) { }

    public void HandleEvent(GameEvent gameEvent) 
    {
        if (RegisteredEvents.ContainsKey(gameEvent.ID))
            RegisteredEvents[gameEvent.ID](gameEvent);

        IComponentData data = GetData();
        if(data != null)
        {
            if (data.RegisteredEvents.ContainsKey(gameEvent.ID))
                data.RegisteredEvents[gameEvent.ID](gameEvent);
        }
    }

    public GameEvent FireEvent(GameObject target, GameEvent gameEvent, bool logEvent = false)
    {
        return target.FireEvent(gameEvent, logEvent);
    }
}


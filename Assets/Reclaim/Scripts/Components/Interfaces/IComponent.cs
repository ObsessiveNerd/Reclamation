using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IComponentBehavior
{
    void HandleEvent(GameEvent gameEvent);
}

public interface IComponent
{
    Entity Entity { get; set; }
    Dictionary<GameEventId, Action<GameEvent>> RegisteredEvents { get; }
    //GameEvent FireEvent(GameObject target, GameEvent gameEvent, bool logEvent = false);
    void HandleEvent(GameEvent gameEvent);
    void WakeUp();
}

[Serializable]
public class EntityComponent : IComponent
{
    public Entity Entity { get; set; }

    public virtual void WakeUp() { }

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
    
    public void HandleEvent(GameEvent gameEvent) 
    {
        if (RegisteredEvents.ContainsKey(gameEvent.ID))
            RegisteredEvents[gameEvent.ID](gameEvent);
    }
}

[RequireComponent(typeof(EntityBehavior))]
public class EntityComponentBehavior : NetworkBehaviour, IComponentBehavior
{
    public virtual IComponent GetData() { return null; }

    //private Dictionary<GameEventId, Action<GameEvent>> m_RegisteredEvents;
    //public Dictionary<GameEventId, Action<GameEvent>> RegisteredEvents
    //{
    //    get
    //    {
    //        if (m_RegisteredEvents == null)
    //            m_RegisteredEvents = new Dictionary<GameEventId, Action<GameEvent>>();
    //        return m_RegisteredEvents;
    //    }
    //}

    public void HandleEvent(GameEvent gameEvent) 
    {
        //if (RegisteredEvents.ContainsKey(gameEvent.ID))
        //    RegisteredEvents[gameEvent.ID](gameEvent);

        IComponent data = GetData();
        if(data != null)
        {
            if (data.RegisteredEvents.ContainsKey(gameEvent.ID))
                data.RegisteredEvents[gameEvent.ID](gameEvent);
        }
    }

    //public GameEvent FireEvent(GameObject target, GameEvent gameEvent, bool logEvent = false)
    //{
    //    return target.FireEvent(gameEvent, logEvent);
    //}
}


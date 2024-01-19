using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Netcode;
using UnityEngine;

public interface IComponentBehavior
{
    EntityComponent GetComponent();
    void SetComponent(IComponent component);
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
public class ComponentBehavior<T> : NetworkBehaviour, IComponentBehavior where T : EntityComponent, new()
{
    [SerializeField]
    T m_Component;

    public T component 
    { 
        get
        {
            if (m_Component == null)
                m_Component = new T();
            return m_Component;
        }
    }

    public void SetComponent(IComponent comp)
    {
        m_Component = comp as T;
    }

    public EntityComponent GetComponent()
    {
        return component;
    }

    public void HandleEvent(GameEvent gameEvent) 
    {
        if(component != null)
        {
            if (component.RegisteredEvents.ContainsKey(gameEvent.ID))
                component.RegisteredEvents[gameEvent.ID](gameEvent);
        }
    }
}


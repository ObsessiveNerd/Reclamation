using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour, IEntity
{
    List<IComponent> m_Components = new List<IComponent>();
    List<IComponent> m_AddQueue = new List<IComponent>();
    List<IComponent> m_RemoveQueue = new List<IComponent>();

    public GameEvent FireEvent(IEntity target, GameEvent gameEvent)
    {
        target.HandleEvent(gameEvent);
        return gameEvent;
    }

    public void HandleEvent(GameEvent gameEvent)
    {
        foreach (IComponent component in m_Components)
        {
            if (component.RespondsTo(gameEvent))
                component.HandleEvent(gameEvent);
        }
    }

    public void AddComponent(IComponent component)
    {
        m_AddQueue.Add(component);
    }

    public void RemoveComponent(IComponent component)
    {
        m_RemoveQueue.Add(component);
    }

    public void RemoveComponent(Type component)
    {
        IComponent comp = m_Components.Find(c => c.GetType() == component);
        m_RemoveQueue.Add(comp);
    }

    public void CleanupComponents()
    {
        foreach (IComponent component in m_RemoveQueue)
            m_Components.Remove(component);
        m_Components.AddRange(m_AddQueue);
        m_RemoveQueue.Clear();
        m_AddQueue.Clear();
    }
}

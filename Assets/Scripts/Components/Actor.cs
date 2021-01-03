using System;
using System.Collections.Generic;

public class Actor : IEntity
{
    PriorityQueue<IComponent> m_Components;
    List<IComponent> m_AddQueue = new List<IComponent>();
    List<IComponent> m_RemoveQueue = new List<IComponent>();

    public string Name { get; internal set; }
    public string ID { get; internal set; }

    public Actor(string name)
    {
        Name = name;
        //TODO: ID = GetID();
        m_Components = new PriorityQueue<IComponent>(new ComponentComparer());
    }

    public GameEvent FireEvent(IEntity target, GameEvent gameEvent)
    {
        target.HandleEvent(gameEvent);
        return gameEvent;
    }

    public void HandleEvent(GameEvent gameEvent)
    {
        foreach (IComponent component in m_Components.Values)
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
        IComponent comp = m_Components.Values.Find(c => c.GetType() == component);
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

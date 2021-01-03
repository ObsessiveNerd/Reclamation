using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComponent
{
    void Init(IEntity self);
    IEntity Self { get; }
    List<string> RegisteredEvents { get; }
    bool RespondsTo(GameEvent gameEvent);
    GameEvent FireEvent(IEntity target, GameEvent gameEvent);
    void HandleEvent(GameEvent gameEvent);
}

public class Component : IComponent
{
    public void Init(IEntity self)
    {
        m_Self = self;
    }

    IEntity m_Self;
    public IEntity Self { get { return m_Self; } }

    private List<string> m_RegisteredEvents = new List<string>();
    public virtual List<string> RegisteredEvents
    {
        get { return m_RegisteredEvents; }
    }

    public GameEvent FireEvent(IEntity target, GameEvent gameEvent)
    {
        target.FireEvent(target, gameEvent);
        return gameEvent;
    }

    public bool RespondsTo(GameEvent gameEvent)
    {
        if (RegisteredEvents.Contains(gameEvent.ID))
            return true;
        return false;
    }

    public virtual void HandleEvent(GameEvent gameEvent) { }
}

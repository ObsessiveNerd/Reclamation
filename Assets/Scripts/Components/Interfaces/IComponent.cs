using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComponent
{
    void Init(IEntity self);
    IEntity Self { get; }
    List<string> RegisteredEvents { get; }
    bool RespondsTo(GameEvent gameEvent);
    GameEvent FireEvent(IEntity target, GameEvent gameEvent, bool logEvent = false);
    void HandleEvent(GameEvent gameEvent);
    int Priority { get; }
}

public class Component : IComponent
{
    public virtual void Init(IEntity self)
    {
        m_Self = self;
    }

    public Component() { }

    //Priority right now is from 1 to 10
    public virtual int Priority { get { return 5; } }

    IEntity m_Self;
    public IEntity Self { get { return m_Self; } }

    private List<string> m_RegisteredEvents = new List<string>();
    public virtual List<string> RegisteredEvents
    {
        get { return m_RegisteredEvents; }
    }

    public GameEvent FireEvent(IEntity target, GameEvent gameEvent, bool logEvent = false)
    {
        if(target != null)
            target.FireEvent(target, gameEvent, logEvent);

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

public class ComponentComparer : IComparer<IComponent>
{
    public int Compare(IComponent x, IComponent y)
    {
        if (x.Priority < y.Priority)
            return -1;
        if (x.Priority == y.Priority)
            return 0;
        return 1;
    }
}


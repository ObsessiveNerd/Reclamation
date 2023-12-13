using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComponent
{
    HashSet<GameEventId> RegisteredEvents { get; }
    bool RespondsTo(GameEvent gameEvent);
    GameEvent FireEvent(GameObject target, GameEvent gameEvent, bool logEvent = false);
    void HandleEvent(GameEvent gameEvent);
}

public class EntityComponent : MonoBehaviour, IComponent
{
    private HashSet<GameEventId> m_RegisteredEvents = new HashSet<GameEventId>();
    public virtual HashSet<GameEventId> RegisteredEvents
    {
        get { return m_RegisteredEvents; }
    }

    public GameEvent FireEvent(GameObject target, GameEvent gameEvent, bool logEvent = false)
    {
        if(target != null)
        {
            foreach(var comp in target.GetComponents<EntityComponent>())
                comp.HandleEvent(gameEvent);
        }
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


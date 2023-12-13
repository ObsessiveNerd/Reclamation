using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractsWithWorld : EntityComponent
{
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.InteractWithTarget, InteractWithTarget);
    }

    void InteractWithTarget(GameEvent gameEvent)
    {
        GameObject target = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Target]);
        Demeanor demeanor = Factions.GetDemeanorForTarget(gameEvent, target);

        switch (demeanor)
        {
            case Demeanor.Friendly:
            case Demeanor.None:
            case Demeanor.Neutral:
                FireEvent(target, GameEventPool.Get(GameEventId.Interact).With(EventParameter.Entity, gameObject), true).Release();
                break;
            case Demeanor.Hostile:
                FireEvent(gameObject, GameEventPool.Get(GameEventId.PerformAttack)
                        .With(EventParameter.Target, target)
                        .With(EventParameter.Melee, true), true).Release();
                break;
        }
    }
}
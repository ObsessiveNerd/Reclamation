using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractsWithWorld : Component
{
    public InteractsWithWorld()
    {
        RegisteredEvents.Add(GameEventId.InteractWithTarget);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        IEntity target = (IEntity)gameEvent.Paramters[EventParameters.Target];
        Demeanor demeanor = Factions.GetDemeanorForTarget(Self, target);

        switch(demeanor)
        {
            case Demeanor.Friendly:
            case Demeanor.None:
            case Demeanor.Neutral:
                FireEvent(target, new GameEvent(GameEventId.Interact));
                break;
            case Demeanor.Hostile:
                FireEvent(Self, new GameEvent(GameEventId.PerformAttack, new KeyValuePair<string, object>(EventParameters.Target, target),
                                                                         new KeyValuePair<string, object>(EventParameters.WeaponType, TypeWeapon.Melee & TypeWeapon.Finesse)));
                break;
        }
    }
}

public class DTO_InteractsWithWorld : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new InteractsWithWorld();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(InteractsWithWorld);
    }
}
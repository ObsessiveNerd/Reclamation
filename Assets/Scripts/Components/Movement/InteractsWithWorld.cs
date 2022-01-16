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
        if (gameEvent.ID == GameEventId.InteractWithTarget)
        {
            IEntity target = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Target]);
            Demeanor demeanor = Factions.GetDemeanorForTarget(Self, target);

            switch (demeanor)
            {
                case Demeanor.Friendly:
                case Demeanor.None:
                case Demeanor.Neutral:
                    FireEvent(target, new GameEvent(GameEventId.Interact, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID)));
                    break;
                case Demeanor.Hostile:
                    FireEvent(Self, new GameEvent(GameEventId.PerformAttack, new KeyValuePair<string, object>(EventParameters.Target, target.ID),
                                                                             new KeyValuePair<string, object>(EventParameters.WeaponType, TypeWeapon.Melee & TypeWeapon.Finesse)));
                    break;
            }
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
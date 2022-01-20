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
                    FireEvent(target, GameEventPool.Get(GameEventId.Interact).With(EventParameters.Entity, Self.ID)).Release();
                    break;
                case Demeanor.Hostile:
                    FireEvent(Self, GameEventPool.Get(GameEventId.PerformAttack)
                            .With(EventParameters.Target, target.ID)
                            .With(EventParameters.WeaponType, TypeWeapon.Melee & TypeWeapon.Finesse)).Release();
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
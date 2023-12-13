using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfTargetingSpell : EntityComponent
{
    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.AmAttacking);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.AmAttacking)
            gameEvent.Paramters[EventParameter.Target] = gameEvent.Paramters[EventParameter.Entity];
    }
}

public class DTO_SelfTargetingSpell : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new SelfTargetingSpell();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(SelfTargetingSpell);
    }
}

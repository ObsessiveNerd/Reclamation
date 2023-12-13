using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWhileEquiped : EntityComponent
{
    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.ItemEquipped);
        RegisteredEvents.Add(GameEventId.ItemUnequipped);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.ItemEquipped)
        {
            var activate = GameEventPool.Get(GameEventId.ActivateObject)
                            .With(EventParameter.Owner, gameEvent.Paramters[EventParameter.Owner]);
            FireEvent(Self, activate, true).Release();
        }

        if (gameEvent.ID == GameEventId.ItemUnequipped)
        {
            var deactivate = GameEventPool.Get(GameEventId.DeactivateObject)
                                .With(EventParameter.Owner, gameEvent.Paramters[EventParameter.Owner]);
            FireEvent(Self, deactivate, true).Release();
        }
    }
}

public class DTO_ActiveWhileEquiped : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new ActiveWhileEquiped();
    }

    public string CreateSerializableData(IComponent component)
    {
        return $"{nameof(ActiveWhileEquiped)}";
    }
}
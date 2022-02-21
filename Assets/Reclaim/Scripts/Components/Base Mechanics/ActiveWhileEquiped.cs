using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWhileEquiped : EntityComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.ItemEquipped);
        RegisteredEvents.Add(GameEventId.ItemUnequipped);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.ItemEquipped)
            FireEvent(Self, GameEventPool.Get(GameEventId.ActivateObject), true).Release();

        if (gameEvent.ID == GameEventId.ItemUnequipped)
            FireEvent(Self, GameEventPool.Get(GameEventId.DeactivateObject), true).Release();
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
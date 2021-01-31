using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFOVHandler : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.FOVRecalculated);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.FOVRecalculated)
            FireEvent(World.Instance.Self, gameEvent);
    }
}

public class DTO_PlayerFOVHandler : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new PlayerFOVHandler();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(PlayerFOVHandler);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFOVHandler : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.FOVRecalculated);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.FOVRecalculated)
        {

        }
    }
}

public class DTO_AIFOVHandler : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new AIFOVHandler();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(AIFOVHandler);
    }
}

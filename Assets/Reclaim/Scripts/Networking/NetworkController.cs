using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : InputControllerBase
{
    public override int Priority => 10;
    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetEnergy);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetEnergy)
            gameEvent.Paramters[EventParameter.Value] = 1f;
    }
}

public class DTO_NetworkController : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new NetworkController();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(NetworkController);
    }
}

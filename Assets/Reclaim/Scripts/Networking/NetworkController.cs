using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : InputControllerBase
{
    public override void Init(IEntity self)
    {
        base.Init(self);

    }

    public override void HandleEvent(GameEvent gameEvent)
    {

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

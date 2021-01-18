using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterWithTimeSystem : Component
{
    public RegisterWithTimeSystem()
    {
        RegisteredEvents.Add(GameEventId.RegisterWithTimeSystem);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        TimeProgression time = (TimeProgression)gameEvent.Paramters[EventParameters.Value];
        time.RegisterEntity(Self);
    }
}

public class DTO_RegisterWithTimeSystem : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new RegisterWithTimeSystem();
    }

    public string CreateSerializableData(IComponent component)
    {
       return nameof(RegisterWithTimeSystem);
    }
}
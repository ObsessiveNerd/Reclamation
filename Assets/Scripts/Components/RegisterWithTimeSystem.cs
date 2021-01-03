using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterWithTimeSystem : Component
{
    public RegisterWithTimeSystem(IEntity self)
    {
        Init(self);
        RegisteredEvents.Add(GameEventId.RegisterWithTimeSystem);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        TimeProgression time = (TimeProgression)gameEvent.Paramters[EventParameters.Value];
        time.RegisterEntity(Self);
    }
}

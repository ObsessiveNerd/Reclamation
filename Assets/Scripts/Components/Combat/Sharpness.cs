using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sharpness : Component
{
    public Sharpness(IEntity self)
    {
        Init(self);
        RegisteredEvents.Add(GameEventId.AmAttacking);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        GameEvent sharpness = new GameEvent(GameEventId.Sharpness, gameEvent.Paramters);
        ((List<GameEvent>)gameEvent.Paramters[EventParameters.AdditionalGameEvents]).Add(sharpness);
    }
}

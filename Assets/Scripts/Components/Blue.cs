using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blue : Component
{
    public Blue()
    {
        RegisteredEvents.Add(GameEventId.AlterSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        ((SpriteRenderer)gameEvent.Paramters[EventParameters.Renderer]).color = Color.blue;
    }
}

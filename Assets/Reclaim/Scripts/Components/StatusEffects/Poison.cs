using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : EntityComponent
{
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.AlterSprite, AlterSprite);
    }

    void AlterSprite(GameEvent gameEvent)
    {
        var renderer = gameEvent.GetValue<SpriteRenderer>(EventParameter.Renderer);
        renderer.color = Color.green;
    }
}

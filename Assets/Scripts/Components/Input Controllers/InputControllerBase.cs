using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputControllerBase : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.UpdateEntity);
    }

    protected void EndSelection(GameEvent gameEvent, Point tileSelection)
    {
        Self.RemoveComponent(this);
        Self.AddComponent(new PlayerInputController());
        FireEvent(World.Instance.Self, new GameEvent(GameEventId.EndSelection, new KeyValuePair<string, object>(EventParameters.TilePosition, tileSelection)));
        gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
        //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
    }
}

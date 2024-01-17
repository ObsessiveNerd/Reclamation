using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksVision : EntityComponent
{
    public override void WakeUp(IComponentData data = null)
    {
        RegisteredEvents.Add(GameEventId.CalculateTileFlags, CalculateTileFlags);
    }

    void CalculateTileFlags(GameEvent gameEvent)
    {
        var flags = gameEvent.GetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags);
        
        flags |= MovementBlockFlag.All;
        gameEvent.SetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags, flags);

        gameEvent.SetValue(EventParameter.BlocksVision, true);
    }
}
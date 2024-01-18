using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlocksVisionData : EntityComponent
{
    public override void WakeUp()
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

public class BlocksVision : EntityComponentBehavior
{
    BlocksVisionData Data = new BlocksVisionData();

    public override IComponent GetData()
    {
        return Data;
    }
}
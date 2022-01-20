using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StairDirection
{
    Up,
    Down
}

public class Stairs : Component
{
    public StairDirection Direction;

    public Stairs(StairDirection direction)
    {
        Direction = direction;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.Pickup);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Pickup)
        {
            string e = gameEvent.GetValue<string>(EventParameters.Entity);
            if (WorldUtility.IsActivePlayer(e))
            {
                GameEvent move = GameEventPool.Get(GameEventId.MoveUp);

                if (Direction == StairDirection.Down)
                    move = GameEventPool.Get(GameEventId.MoveDown);

                FireEvent(World.Services.Self, move).Release();
            }
        }
    }
}

public class DTO_Stairs : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        StairDirection dir = (StairDirection)Enum.Parse(typeof(StairDirection), data.Split('=')[1]);
        Component = new Stairs(dir);
    }

    public string CreateSerializableData(IComponent component)
    {
        Stairs stairs = (Stairs)component;
        return $"{nameof(Stairs)}: {nameof(stairs.Direction)}={stairs.Direction.ToString()}";
    }
}
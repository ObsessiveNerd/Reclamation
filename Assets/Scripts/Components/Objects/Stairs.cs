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
        RegisteredEvents.Add(GameEventId.Interact);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Interact)
        {
            EventBuilder move = new EventBuilder(GameEventId.MoveUp);

            if (Direction == StairDirection.Down)
                move = new EventBuilder(GameEventId.MoveDown);

            FireEvent(World.Instance.Self, move.CreateEvent());
        }
    }
}

public class DTO_Stairs : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        StairDirection dir = (StairDirection)Enum.Parse(typeof(StairDirection), data);
        Component = new Stairs(dir);
    }

    public string CreateSerializableData(IComponent component)
    {
        Stairs stairs = (Stairs)component;
        return $"{nameof(Stairs)}: {stairs.Direction.ToString()}";
    }
}
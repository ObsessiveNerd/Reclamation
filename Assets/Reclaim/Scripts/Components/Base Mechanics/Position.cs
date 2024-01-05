using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : EntityComponent
{
    public Point Point;

    void Start()
    {
        RegisteredEvents.Add(GameEventId.MoveEntity, MoveEntity);
        transform.position = Services.Map.GetTile(Point).transform.position;
    }

    void MoveEntity(GameEvent gameEvent)
    {
        Point = gameEvent.GetValue<Point>(EventParameter.TilePosition);
        transform.position = Services.Map.GetTile(Point).transform.position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Spawner
{
    public static void Spawn(IEntity e, Point point)
    {
        Spawn(e, point.x, point.y);
    }

    public static void Spawn(IEntity e, int x, int y)
    {
        if (x == -1 && y == -1)
            return;

        EntityType entityType = (EntityType)e.FireEvent(e, new GameEvent(GameEventId.GetEntityType, new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.None))).Paramters[EventParameters.EntityType];


        World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.Spawn, new KeyValuePair<string, object>(EventParameters.Entity, e.ID),
                                                                            new KeyValuePair<string, object>(EventParameters.EntityType, entityType),
                                                                            new KeyValuePair<string, object>(EventParameters.Point, new Point(x, y))));
    }

    public static void Despawn(IEntity e)
    {
        if (e == null)
            return;

        EntityType entityType = (EntityType)e.FireEvent(e, new GameEvent(GameEventId.GetEntityType, new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.None))).Paramters[EventParameters.EntityType];


        World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.Despawn, new KeyValuePair<string, object>(EventParameters.Entity, e.ID),
                                                                            new KeyValuePair<string, object>(EventParameters.EntityType, entityType)));
    }

    public static void Restore(IEntity e)
    {
        Point spawnPoint = (Point)e.FireEvent(e, new GameEvent(GameEventId.GetPoint, new KeyValuePair<string, object>(EventParameters.Value, null))).Paramters[EventParameters.Value];
        Spawn(e, spawnPoint);
    }
}

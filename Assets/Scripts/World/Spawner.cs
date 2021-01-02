using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Spawner
{
    public static void Spawn(IEntity e, EntityType entityType, int x, int y)
    {
        World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.Spawn, new KeyValuePair<string, object>(EventParameters.Entity, e),
                                                                            new KeyValuePair<string, object>(EventParameters.EntityType, entityType),
                                                                            new KeyValuePair<string, object>(EventParameters.Point, new Point(x, y))));
    }
}

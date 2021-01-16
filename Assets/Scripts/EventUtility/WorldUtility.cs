using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldUtility
{
    public static IEntity GetEntityAtPosition(IEntity source, Point position)
    {
        GameEvent result = source.FireEvent(World.Instance.Self, new GameEvent(GameEventId.GetEntityOnTile, new KeyValuePair<string, object>(EventParameters.TilePosition, position),
                                                                                            new KeyValuePair<string, object>(EventParameters.Entity, null)));

        return (IEntity)result.Paramters[EventParameters.Entity];
    }

    public static IEntity GetClosestEnemyTo(IEntity e)
    {
        return null;
    }
}

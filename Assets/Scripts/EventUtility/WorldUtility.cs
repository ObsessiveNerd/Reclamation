using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldUtility
{
    public static IEntity GetEntityAtPosition(Point position)
    {
        GameEvent result = World.Instance.Self.FireEvent(new GameEvent(GameEventId.GetEntityOnTile, new KeyValuePair<string, object>(EventParameters.TilePosition, position),
                                                                                            new KeyValuePair<string, object>(EventParameters.Entity, null)));

        return EntityQuery.GetEntity((string)result.Paramters[EventParameters.Entity]);
    }

    public static IEntity GetClosestEnemyTo(IEntity e)
    {
        EventBuilder eventBuilder = new EventBuilder(GameEventId.GetClosestEnemy)
                                    .With(EventParameters.Entity, e.ID)
                                    .With(EventParameters.Value, null);

        string id = World.Instance.Self.FireEvent(eventBuilder.CreateEvent()).GetValue<string>(EventParameters.Value);
        return EntityQuery.GetEntity(id);
    }
}

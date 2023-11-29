using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldUtility
{
    public static IEntity GetEntityAtPosition(Point position, bool includeTile = true)
    {
        return Services.WorldDataQuery.GetEntityOnTile(position);
    }

    public static Point GetEntityPosition(IEntity entity)
    {
        GameEvent result = entity.FireEvent(GameEventPool.Get(GameEventId.GetPoint)
            .With(EventParameter.Value, null));

        var ret = result.GetValue<Point>(EventParameter.Value);
        result.Release();
        return ret;
    }

    public static IEntity GetClosestEnemyTo(IEntity e)
    {
        return Services.WorldDataQuery.GetClosestEnemy(e);
    }

    public static GameObject GetGameObject(IEntity e)
    {
        return Services.WorldDataQuery.GetGameObject(Services.EntityMapService.GetPointWhereEntityIs(e));
    }

    public static bool IsActivePlayer(string entityId)
    {
        return Services.WorldDataQuery.GetActivePlayerId() == entityId;
    }

    public static void RegisterUI(IUpdatableUI ui)
    {
        Services.WorldUIService.RegisterUpdatableUI(ui);
    }

    public static void UnRegisterUI(IUpdatableUI ui)
    {
        Services.WorldUIService.UnregisterUpdatableUI(ui);
    }
}

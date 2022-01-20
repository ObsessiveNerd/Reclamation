using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldUtility
{
    public static IEntity GetEntityAtPosition(Point position, bool includeTile = true)
    {
        GameEvent result = World.Instance.Self.FireEvent(GameEventPool.Get(GameEventId.GetEntityOnTile)
                .With(EventParameters.TilePosition, position)
                .With(EventParameters.Entity, null)
                .With(EventParameters.IncludeSelf, includeTile));

        var ret = EntityQuery.GetEntity((string)result.Paramters[EventParameters.Entity]);
        result.Release();
        return ret;
    }

    public static Point GetEntityPosition(IEntity entity)
    {
        GameEvent result = entity.FireEvent(GameEventPool.Get(GameEventId.GetPoint)
            .With(EventParameters.Value, null));

        var ret = result.GetValue<Point>(EventParameters.Value);
        result.Release();
        return ret;
    }

    public static bool IsPlayableCharacter(string id)
    {
        GameEvent isPlayableCharacter = GameEventPool.Get(GameEventId.IsPlayableCharacter)
                                            .With(EventParameters.Entity, id)
                                            .With(EventParameters.Value, false);
        var ret = World.Instance.Self.FireEvent(isPlayableCharacter).GetValue<bool>(EventParameters.Value);
        isPlayableCharacter.Release();
        return ret;
    }

    public static IEntity GetClosestEnemyTo(IEntity e)
    {
        GameEvent getClosestEnemy = GameEventPool.Get(GameEventId.GetClosestEnemy)
                                    .With(EventParameters.Entity, e.ID)
                                    .With(EventParameters.Value, null);

        string id = World.Instance.Self.FireEvent(getClosestEnemy).GetValue<string>(EventParameters.Value);
        var entity = EntityQuery.GetEntity(id);
        getClosestEnemy.Release();
        return entity;
    }

    public static GameObject GetGameObject(IEntity e)
    {
        GameEvent getGameObjectLocation = GameEventPool.Get(GameEventId.GameObject)
                                                .With(EventParameters.Point, GetEntityPosition(e))
                                                .With(EventParameters.Value, null);
        var ret = World.Instance.Self.FireEvent(getGameObjectLocation).GetValue<GameObject>(EventParameters.Value);
        getGameObjectLocation.Release();
        return ret;
    }

    public static bool IsActivePlayer(string entityId)
    {
        GameEvent isActivePlayer = GameEventPool.Get(GameEventId.GetActivePlayerId)
                                        .With(EventParameters.Value, null);
        var ret = entityId == World.Instance.Self.FireEvent(isActivePlayer).GetValue<string>(EventParameters.Value);
        isActivePlayer.Release();
        return ret;
    }

    public static string GetActivePlayerId()
    {
        GameEvent isActivePlayer = GameEventPool.Get(GameEventId.GetActivePlayerId)
                                        .With(EventParameters.Value, null);
        var ret = World.Instance.Self.FireEvent(isActivePlayer).GetValue<string>(EventParameters.Value);
        isActivePlayer.Release();
        return ret;
    }

    public static void RegisterUI(IUpdatableUI ui)
    {
        GameEvent e = GameEventPool.Get(GameEventId.RegisterUI)
                            .With(EventParameters.GameObject, ui);

        World.Instance.Self.FireEvent(e).Release();
    }

    public static void UnRegisterUI(IUpdatableUI ui)
    {
        GameEvent e = GameEventPool.Get(GameEventId.UnRegisterUI)
                            .With(EventParameters.GameObject, ui);

        World.Instance.Self.FireEvent(e).Release();
    }
}

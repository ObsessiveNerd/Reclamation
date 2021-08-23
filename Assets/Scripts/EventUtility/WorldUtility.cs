﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldUtility
{
    public static IEntity GetEntityAtPosition(Point position, bool includeTile = true)
    {
        GameEvent result = World.Instance.Self.FireEvent(new GameEvent(GameEventId.GetEntityOnTile, new KeyValuePair<string, object>(EventParameters.TilePosition, position),
                                                                                            new KeyValuePair<string, object>(EventParameters.Entity, null),
                                                                                            new KeyValuePair<string, object>(EventParameters.IncludeSelf, includeTile)));

        return EntityQuery.GetEntity((string)result.Paramters[EventParameters.Entity]);
    }

    public static Point GetEntityPosition(IEntity entity)
    {
        GameEvent result = entity.FireEvent(new GameEvent(GameEventId.GetPoint, new KeyValuePair<string, object>(EventParameters.Value, null)));

        return result.GetValue<Point>(EventParameters.Value);
    }

    public static bool IsPlayableCharacter(string id)
    {
        EventBuilder isPlayableCharacter = new EventBuilder(GameEventId.IsPlayableCharacter)
                                            .With(EventParameters.Entity, id)
                                            .With(EventParameters.Value, false);
        return World.Instance.Self.FireEvent(isPlayableCharacter.CreateEvent()).GetValue<bool>(EventParameters.Value);
    }

    public static IEntity GetClosestEnemyTo(IEntity e)
    {
        EventBuilder eventBuilder = new EventBuilder(GameEventId.GetClosestEnemy)
                                    .With(EventParameters.Entity, e.ID)
                                    .With(EventParameters.Value, null);

        string id = World.Instance.Self.FireEvent(eventBuilder.CreateEvent()).GetValue<string>(EventParameters.Value);
        var entity = EntityQuery.GetEntity(id);
        return entity;
    }

    public static GameObject GetGameObject(IEntity e)
    {
        EventBuilder getGameObjectLocation = new EventBuilder(GameEventId.GameObject)
                                                .With(EventParameters.Point, GetEntityPosition(e))
                                                .With(EventParameters.Value, null);
        return World.Instance.Self.FireEvent(getGameObjectLocation.CreateEvent()).GetValue<GameObject>(EventParameters.Value);
    }

    public static bool IsActivePlayer(string entityId)
    {
        EventBuilder isActivePlayer = new EventBuilder(GameEventId.GetActivePlayerId)
                                        .With(EventParameters.Value, null);
        return entityId == World.Instance.Self.FireEvent(isActivePlayer.CreateEvent()).GetValue<string>(EventParameters.Value);
    }

    public static string GetActivePlayerId()
    {
        EventBuilder isActivePlayer = new EventBuilder(GameEventId.GetActivePlayerId)
                                        .With(EventParameters.Value, null);
        return World.Instance.Self.FireEvent(isActivePlayer.CreateEvent()).GetValue<string>(EventParameters.Value);
    }

    public static void RegisterUI(IUpdatableUI ui)
    {
        EventBuilder e = new EventBuilder(GameEventId.RegisterUI)
                            .With(EventParameters.GameObject, ui);

        World.Instance.Self.FireEvent(e.CreateEvent());
    }

    public static void UnRegisterUI(IUpdatableUI ui)
    {
        EventBuilder e = new EventBuilder(GameEventId.UnRegisterUI)
                            .With(EventParameters.GameObject, ui);

        World.Instance.Self.FireEvent(e.CreateEvent());
    }
}

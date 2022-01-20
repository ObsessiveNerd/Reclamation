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

        GameEvent getEntityTypeEvent = GameEventPool.Get(GameEventId.GetEntityType)
            .With(EventParameters.EntityType, EntityType.None);

        EntityType entityType = (EntityType)e.FireEvent(e, getEntityTypeEvent)
            .Paramters[EventParameters.EntityType];


        World.Services.Self.FireEvent(World.Services.Self, GameEventPool.Get(GameEventId.Spawn)
                .With(EventParameters.Entity, e.ID)
                .With(EventParameters.EntityType, entityType)
                .With(EventParameters.Point, new Point(x, y))).Release();

        foreach (var comp in e.GetComponents())
            comp.Start();

        e.FireEvent(GameEventPool.Get(GameEventId.InitFOV)).Release();
        getEntityTypeEvent.Release();
    }

    public static void Despawn(IEntity e)
    {
        if (e == null)
            return;

        GameEvent getEntityTypeEvent = GameEventPool.Get(GameEventId.GetEntityType)
            .With(EventParameters.EntityType, EntityType.None);
        EntityType entityType = (EntityType)e.FireEvent(e, getEntityTypeEvent).Paramters[EventParameters.EntityType];


        World.Services.Self.FireEvent(World.Services.Self, GameEventPool.Get(GameEventId.Despawn)
                .With(EventParameters.Entity, e.ID)
                .With(EventParameters.EntityType, entityType)).Release();
        getEntityTypeEvent.Release();
    }

    public static void Swap(IEntity lhs, IEntity rhs)
    {
        var lhsPos = WorldUtility.GetEntityPosition(lhs);
        var rhsPos = WorldUtility.GetEntityPosition(rhs);

        Despawn(lhs);
        Despawn(rhs);

        Spawn(lhs, rhsPos);
        Spawn(rhs, lhsPos);

        //lhsPos = WorldUtility.GetEntityPosition(lhs);
        //rhsPos = WorldUtility.GetEntityPosition(rhs);
    }

    public static void Move(IEntity e, Point newPoint)
    {
        if (e == null)
            return;

        GameEvent getEntityTypeEvent = GameEventPool.Get(GameEventId.GetEntityType)
            .With(EventParameters.EntityType, EntityType.None);
        EntityType entityType = (EntityType)e.FireEvent(e, getEntityTypeEvent).Paramters[EventParameters.EntityType];


        World.Services.Self.FireEvent(World.Services.Self, GameEventPool.Get(GameEventId.SetEntityPosition)
                .With(EventParameters.Entity, e.ID)
                .With(EventParameters.EntityType, entityType)
                .With(EventParameters.TilePosition, newPoint)).Release();
        getEntityTypeEvent.Release();
    }

    public static void Restore(IEntity e)
    {
        GameEvent getPoint = GameEventPool.Get(GameEventId.GetPoint).With(EventParameters.Value, null);
        Point spawnPoint = (Point)e.FireEvent(e,getPoint).Paramters[EventParameters.Value];
        Spawn(e, spawnPoint);
        getPoint.Release();
    }
}

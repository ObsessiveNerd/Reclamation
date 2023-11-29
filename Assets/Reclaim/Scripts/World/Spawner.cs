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

        Services.SpawnerService.Spawn(e, new Point(x, y));

        foreach (var comp in e.GetComponents())
            comp.Start();
        e.FireEvent(GameEventPool.Get(GameEventId.InitFOV)).Release();
    }

    public static void Despawn(IEntity e)
    {
        if (e == null)
            return;

        Services.SpawnerService.Despawn(e);
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

    public static void Restore(IEntity e)
    {
        GameEvent getPoint = GameEventPool.Get(GameEventId.GetPoint).With(EventParameter.Value, null);
        Point spawnPoint = (Point)e.FireEvent(e,getPoint).Paramters[EventParameter.Value];
        Spawn(e, spawnPoint);
        getPoint.Release();
    }
}

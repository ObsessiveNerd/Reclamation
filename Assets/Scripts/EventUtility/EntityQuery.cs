using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityQuery
{
    public static IEntity GetEntity(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        GameEvent builder = GameEventPool.Get(GameEventId.GetEntity)
                                .With(EventParameters.Value, id)
                                .With(EventParameters.Entity, null);
        var entity =  World.Instance?.Self.FireEvent(World.Instance.Self, builder).GetValue<IEntity>(EventParameters.Entity);
        builder.Release();
        if (entity != null)
            return entity;
        return EntityFactory.CreateEntity(id);
    }

    public static string GetEntityName(string id)
    {
        if (!string.IsNullOrEmpty(id) && EntityMap.IDToNameMap.ContainsKey(id))
            return EntityMap.IDToNameMap[id];
        return "";
    }
}

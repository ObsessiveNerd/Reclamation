using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityQuery
{
    public static IEntity GetEntity(string id)
    {
        return EntityMap.GetEntity(id);

        //if (string.IsNullOrEmpty(id))
        //    return null;

        //GameEvent builder = GameEventPool.Get(GameEventId.GetEntity)
        //                        .With(EventParameters.Value, id)
        //                        .With(EventParameters.Entity, null);
        //var entity =  World.Instance?.Self.FireEvent(World.Instance.Self, builder).GetValue<IEntity>(EventParameters.Entity);

        //IEntity returnValue = null;
        //if (entity != null)
        //    returnValue = entity;
        //else
        //    returnValue = EntityFactory.CreateEntity(id);
        ////builder.Release();
        //return returnValue;
    }

    public static string GetEntityName(string id)
    {
        if (!string.IsNullOrEmpty(id) && EntityMap.IDToNameMap.ContainsKey(id))
            return EntityMap.IDToNameMap[id];
        return "";
    }
}

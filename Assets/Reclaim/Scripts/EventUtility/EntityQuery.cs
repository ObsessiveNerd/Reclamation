using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityQuery
{
    public static GameObject GetEntity(string id)
    {
        return Services.EntityMapService.GetEntity(id);

        //if (string.IsNullOrEmpty(id))
        //    return null;

        //GameEvent builder = GameEventPool.Get(GameEventId.GetEntity)
        //                        .With(EventParameters.Value, id)
        //                        .With(EventParameters.Entity, null);
        //var entity =  World.Instance?.Self.FireEvent(World.Instance.Self, builder).GetValue<GameObject>(EventParameters.Entity);

        //GameObject returnValue = null;
        //if (entity != null)
        //    returnValue = entity;
        //else
        //    returnValue = EntityFactory.CreateEntity(id);
        ////builder.Release();
        //return returnValue;
    }

    public static string GetEntityName(string id)
    {
        return Services.EntityMapService.GetEntity(id)?.Name;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityQuery
{
    public static IEntity GetEntity(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        EventBuilder builder = new EventBuilder(GameEventId.GetEntity)
                                .With(EventParameters.Value, id)
                                .With(EventParameters.Entity, null);
        return World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent()).GetValue<IEntity>(EventParameters.Entity);
    }
}

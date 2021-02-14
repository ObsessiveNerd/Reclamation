using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractOnTileOvertake : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.EntityOvertaking);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.EntityOvertaking)
        {
            IEntity source = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            GameEvent ge = new GameEvent(GameEventId.InteractWithTarget, new KeyValuePair<string, object>(EventParameters.Target, Self.ID));
            FireEvent(source, ge);
        }
    }
}

public class DTO_InteractOnTileOvertake : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new InteractOnTileOvertake();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(InteractOnTileOvertake);
    }
}
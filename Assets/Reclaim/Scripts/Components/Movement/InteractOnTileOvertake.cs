using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractOnTileOvertake : EntityComponent
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
            GameEvent ge = GameEventPool.Get(GameEventId.InteractWithTarget)
                .With(EventParameters.Target, Self.ID);
            FireEvent(source, ge, true).Release();
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
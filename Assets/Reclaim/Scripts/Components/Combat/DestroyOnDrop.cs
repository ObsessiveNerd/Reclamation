using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDrop : EntityComponent
{
    public override void Init(GameObject self)
    {
        RegisteredEvents.Add(GameEventId.Drop);
        base.Init(self);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Drop)
        {
            Spawner.Despawn(Self);
        }
    }
}

public class DTO_DestroyOnDrop : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new DestroyOnDrop();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(DestroyOnDrop);
    }
}

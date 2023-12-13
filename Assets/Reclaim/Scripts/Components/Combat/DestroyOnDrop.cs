using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDrop : EntityComponent
{
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.Drop);
        
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

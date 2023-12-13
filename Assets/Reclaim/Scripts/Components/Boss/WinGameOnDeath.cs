using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGameOnDeath : EntityComponent
{
    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.Died);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Died)
            Services.StateManagerService.GameOver(true);
    }
}

public class DTO_WinGameOnDeath : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new WinGameOnDeath();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(WinGameOnDeath);
    }
}

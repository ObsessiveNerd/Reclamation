using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHanded : EntityComponent
{
    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.GetMultiBodyPartUse);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetMultiBodyPartUse)
        {
            gameEvent.Paramters[EventParameter.Value] = 2;
        }
    }
}

public class DTO_TwoHanded : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new TwoHanded();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(TwoHanded);
    }
}
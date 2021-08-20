using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyLeader : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetPackInformation);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        base.HandleEvent(gameEvent);
        if(gameEvent.ID == GameEventId.GetPackInformation)
        {
            gameEvent.Paramters[EventParameters.IsPartyLeader] = true;
            gameEvent.Paramters[EventParameters.Entity] = Self.ID;
        }
    }
}

public class DTO_PartyLeader : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new PartyLeader();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(PartyLeader);
    }
}

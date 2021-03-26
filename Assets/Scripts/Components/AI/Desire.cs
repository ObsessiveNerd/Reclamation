﻿using System.Collections;
using System.Collections.Generic;

public class Desire : Component
{
    public int Greed;

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetActionToTake);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetActionToTake)
        {

        }
    }
}

public class DTO_Desire : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Desire();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Desire);
    }
}
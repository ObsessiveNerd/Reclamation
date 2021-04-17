using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Name : Component
{
    public string PrettyName;

    public Name(string name)
    {
        PrettyName = name;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetInfo)
        {
            gameEvent.Paramters[EventParameters.Name] = PrettyName;
        }
    }
}

public class DTO_Name : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Name(data);
    }

    public string CreateSerializableData(IComponent component)
    {
        Name gc = (Name)component;
        return $"{nameof(Name)}:{gc.PrettyName}";
    }
}

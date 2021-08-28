﻿using System.Collections;
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
        RegisteredEvents.Add(GameEventId.GetName);
        RegisteredEvents.Add(GameEventId.SetInfo);
        RegisteredEvents.Add(GameEventId.SetName);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetName)
        {
            if(!string.IsNullOrEmpty(PrettyName))
                gameEvent.Paramters[EventParameters.Name] = PrettyName;
            else
                gameEvent.Paramters[EventParameters.Name] = Self.InternalName;
        }

        if(gameEvent.ID == GameEventId.SetName)
            PrettyName = gameEvent.GetValue<string>(EventParameters.Name);
    }
}

public class DTO_Name : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data;
        if (value.Contains("="))
            value = value.Split('=')[1];
        Component = new Name(value);
    }

    public string CreateSerializableData(IComponent component)
    {
        Name gc = (Name)component;
        return $"{nameof(Name)}: {nameof(gc.PrettyName)}={gc.PrettyName}";
    }
}

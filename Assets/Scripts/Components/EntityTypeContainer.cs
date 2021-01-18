﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    None = 0,
    Creature,
    Object,
    Item
}

public class EntityTypeContainer : Component
{
    public EntityType Type;

    public EntityTypeContainer(EntityType type)
    {
        Type = type;
        RegisteredEvents.Add(GameEventId.GetEntityType);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.EntityType] = Type;
    }
}

public class DTO_EntityTypeContainer : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        EntityType t = (EntityType)Enum.Parse(typeof(EntityType), data);
        Component = new EntityTypeContainer(t);
    }

    public string CreateSerializableData(IComponent component)
    {
        EntityTypeContainer etc = (EntityTypeContainer)component;
        return $"{nameof(EntityTypeContainer)}:{etc.Type}";
    }
}
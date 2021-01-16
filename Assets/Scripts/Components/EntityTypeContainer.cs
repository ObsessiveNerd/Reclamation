using System;
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
    private EntityType m_Type;

    public EntityTypeContainer(EntityType type)
    {
        m_Type = type;
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
}
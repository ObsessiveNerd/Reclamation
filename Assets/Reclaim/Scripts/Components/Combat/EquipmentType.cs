using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentType : EntityComponent
{
    public BodyPart EquipmentBodyPart;

    public EquipmentType(BodyPart part)
    {
        EquipmentBodyPart = part;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetBodyPartType);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetValidAppendage)
        {
            gameEvent.Paramters[EventParameter.Value] = EquipmentBodyPart;
        }
    }
}

public class DTO_EquipmentType : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        BodyPart bp = (BodyPart)Enum.Parse(typeof(BodyPart), data);
        Component = new EquipmentType(bp);
    }

    public string CreateSerializableData(IComponent component)
    {
        EquipmentType et = (EquipmentType)component;
        return $"{nameof(EquipmentType)}:{et.EquipmentBodyPart}";
    }
}

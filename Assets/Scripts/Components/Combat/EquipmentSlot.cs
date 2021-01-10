using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : Component
{
    IEntity m_Equipment;

    public EquipmentSlot(IEntity self, IEntity equipment = null)
    {
        Init(self);
        m_Equipment = equipment;
        RegisteredEvents.Add(GameEventId.AddArmorValue);
        RegisteredEvents.Add(GameEventId.GetEquipment);
        RegisteredEvents.Add(GameEventId.Equip);
        RegisteredEvents.Add(GameEventId.Unequip);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.AddArmorValue)
            FireEvent(m_Equipment, gameEvent);

        if (gameEvent.ID == GameEventId.GetEquipment)
            gameEvent.Paramters[EventParameters.Entity] = m_Equipment;

        if (gameEvent.ID == GameEventId.Equip)
        {
            if (m_Equipment != null)
                FireEvent(Self, new GameEvent(GameEventId.Unequip));
            m_Equipment = (IEntity)gameEvent.Paramters[EventParameters.Entity];
        }

        if(gameEvent.ID == GameEventId.Unequip)
        {
            FireEvent(Self, new GameEvent(GameEventId.AddToInventory, new KeyValuePair<string, object>(EventParameters.Entity, m_Equipment)));
            m_Equipment = null;
        }
    }
}

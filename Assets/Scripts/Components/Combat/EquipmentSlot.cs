using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : Component
{
    IEntity m_Equipment;

    public EquipmentSlot()
    {
        RegisteredEvents.Add(GameEventId.AddArmorValue);
        RegisteredEvents.Add(GameEventId.GetEquipment);
        RegisteredEvents.Add(GameEventId.Equip);
        RegisteredEvents.Add(GameEventId.Unequip);
        RegisteredEvents.Add(GameEventId.CheckEquipment);
    }

    void EquipmentDestroyed()
    {
        if(m_Equipment != null)
        {
            m_Equipment.Destroyed -= EquipmentDestroyed;
            m_Equipment = null;
        }
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.AddArmorValue)
            FireEvent(m_Equipment, gameEvent);

        if (gameEvent.ID == GameEventId.GetEquipment)
            gameEvent.Paramters[EventParameters.Equipment] = m_Equipment;

        if (gameEvent.ID == GameEventId.Equip)
        {
            if (m_Equipment != null)
                FireEvent(Self, new GameEvent(GameEventId.Unequip));
            m_Equipment = (IEntity)gameEvent.Paramters[EventParameters.Equipment];
            m_Equipment.Destroyed += EquipmentDestroyed;
        }

        if(gameEvent.ID == GameEventId.Unequip)
        {
            FireEvent(Self, new GameEvent(GameEventId.AddToInventory, new KeyValuePair<string, object>(EventParameters.Entity, m_Equipment)));
            m_Equipment = null;
        }

        if(gameEvent.ID == GameEventId.CheckEquipment)
        {
            GameEvent ge = (GameEvent)gameEvent.Paramters[EventParameters.GameEvent];
            FireEvent(m_Equipment, ge);
        }
    }
}

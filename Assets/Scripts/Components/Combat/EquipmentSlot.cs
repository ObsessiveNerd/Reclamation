using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : Component
{
    string m_EquipmentId;

    public EquipmentSlot()
    {
        RegisteredEvents.Add(GameEventId.AddArmorValue);
        RegisteredEvents.Add(GameEventId.GetEquipment);
        RegisteredEvents.Add(GameEventId.Equip);
        RegisteredEvents.Add(GameEventId.Unequip);
        RegisteredEvents.Add(GameEventId.CheckEquipment);
    }

    //void EquipmentDestroyed(IEntity e)
    //{
    //    if(m_Equipment != null && m_Equipment == e)
    //    {
    //        m_Equipment.Destroyed -= EquipmentDestroyed;
    //        m_Equipment = null;
    //    }
    //}

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.AddArmorValue)
            FireEvent(EntityQuery.GetEntity(m_EquipmentId), gameEvent);

        if (gameEvent.ID == GameEventId.GetEquipment)
            gameEvent.Paramters[EventParameters.Equipment] = m_EquipmentId;

        if (gameEvent.ID == GameEventId.Equip)
        {
            if (m_EquipmentId != null)
                FireEvent(Self, new GameEvent(GameEventId.Unequip));

            FireEvent(EntityQuery.GetEntity(m_EquipmentId), new GameEvent(GameEventId.ItemEquipped));
            //m_EquipmentId = (string)gameEvent.Paramters[EventParameters.Equipment];
            //m_Equipment.Destroyed += EquipmentDestroyed;
        }

        if(gameEvent.ID == GameEventId.Unequip)
        {
            if (m_EquipmentId != null)
            {
                FireEvent(Self, new GameEvent(GameEventId.AddToInventory, new KeyValuePair<string, object>(EventParameters.Entity, m_EquipmentId)));
                FireEvent(EntityQuery.GetEntity(m_EquipmentId), new GameEvent(GameEventId.ItemUnequipped));
                m_EquipmentId = null;
            }
        }

        if(gameEvent.ID == GameEventId.CheckEquipment)
        {
            GameEvent ge = (GameEvent)gameEvent.Paramters[EventParameters.GameEvent];
            FireEvent(EntityQuery.GetEntity(m_EquipmentId), ge);
        }
    }
}

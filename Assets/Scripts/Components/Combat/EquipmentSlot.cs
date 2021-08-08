using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : Component
{
    public string EquipmentId;
    public string EquipmentName;

    public EquipmentSlot()
    {
        RegisteredEvents.Add(GameEventId.AddArmorValue);
        RegisteredEvents.Add(GameEventId.GetEquipment);
        RegisteredEvents.Add(GameEventId.Equip);
        RegisteredEvents.Add(GameEventId.Unequip);
        RegisteredEvents.Add(GameEventId.CheckEquipment);
        RegisteredEvents.Add(GameEventId.GetCombatRating);
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
            FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);

        if (gameEvent.ID == GameEventId.GetEquipment)
            gameEvent.Paramters[EventParameters.Equipment] = EquipmentId;

        if (gameEvent.ID == GameEventId.Equip)
        {
            if (!string.IsNullOrEmpty(EquipmentId))
                FireEvent(Self, new GameEvent(GameEventId.Unequip));

            var entity = EntityQuery.GetEntity(EquipmentId);
            FireEvent(entity, new GameEvent(GameEventId.ItemEquipped));
            EquipmentId = (string)gameEvent.Paramters[EventParameters.Equipment];
            EquipmentName = entity?.Name;
            //m_Equipment.Destroyed += EquipmentDestroyed;
        }

        if(gameEvent.ID == GameEventId.Unequip)
        {
            if (EquipmentId != null && gameEvent.GetValue<string>(EventParameters.Item) == EquipmentId)
            {
                IEntity owner = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
                FireEvent(owner, new GameEvent(GameEventId.AddToInventory, new KeyValuePair<string, object>(EventParameters.Entity, EquipmentId)));
                FireEvent(EntityQuery.GetEntity(EquipmentId), new GameEvent(GameEventId.ItemUnequipped));
                EquipmentId = null;
                EquipmentName = "";
            }
        }

        if(gameEvent.ID == GameEventId.CheckEquipment)
        {
            GameEvent ge = (GameEvent)gameEvent.Paramters[EventParameters.GameEvent];
            FireEvent(EntityQuery.GetEntity(EquipmentId), ge);
        }

        if(gameEvent.ID == GameEventId.GetCombatRating)
        {
            if(!string.IsNullOrEmpty(EquipmentId))
                FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);
        }
    }
}

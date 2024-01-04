using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : EntityComponent
{
    public GameObject Equipment;

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.GetImmunity, PassEventToEquipment);
        RegisteredEvents.Add(GameEventId.GetResistances, PassEventToEquipment);
        RegisteredEvents.Add(GameEventId.AddArmorValue, PassEventToEquipment);
        RegisteredEvents.Add(GameEventId.GetSpells, PassEventToEquipment);
        RegisteredEvents.Add(GameEventId.GetWeapon, GetEquipment);
        RegisteredEvents.Add(GameEventId.GetEquipment, GetEquipment);
        RegisteredEvents.Add(GameEventId.ItemEquipped, ItemEquipped);
        //RegisteredEvents.Add(GameEventId.Equip);
        RegisteredEvents.Add(GameEventId.Unequip, Unequip);
        //RegisteredEvents.Add(GameEventId.CheckEquipment);
        //RegisteredEvents.Add(GameEventId.GetCombatRating);
        RegisteredEvents.Add(GameEventId.PerformAttack, PerformAttack);
        RegisteredEvents.Add(GameEventId.Drop, Drop);
        RegisteredEvents.Add(GameEventId.Died, Died);
        RegisteredEvents.Add(GameEventId.GetBodyPartType, GetBodyPartType);
        RegisteredEvents.Add(GameEventId.CheckItemEquiped, CheckItemEquiped);
        RegisteredEvents.Add(GameEventId.EndTurn, EndTurn);
    }

    void PassEventToEquipment(GameEvent gameEvent)
    {
        if (Equipment != null)
            Equipment.FireEvent(gameEvent);
    }

    public void GetImmunity(GameEvent gameEvent)
    {
    
    }
    public void GetResistances(GameEvent gameEvent)
    {
    
    }
    public void AddArmorValue(GameEvent gameEvent)
    {
    
    }
    public void GetSpells(GameEvent gameEvent)
    {
    
    }
    public void GetWeapon(GameEvent gameEvent)
    {
        
    }
    public void GetEquipment(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameter.Equipment] = Equipment;
    }
    public void ItemEquipped(GameEvent gameEvent)
    {
        var entity = gameEvent.GetValue<GameObject>(EventParameter.Equipment);
        if (entity == null)
        {
            Debug.LogError($"Attempting to equip a null item, {gameEvent.GetValue<GameObject>(EventParameter.Equipment)}");
            return;
        }
        entity.FireEvent(gameEvent);
        Equipment = entity;

        GameEvent playOnEquipSound = GameEventPool.Get(GameEventId.Playsound)
                                        .With(EventParameter.SoundSource, gameObject)
                                        .With(EventParameter.Key, SoundKey.Activate);
        entity.FireEvent(playOnEquipSound).Release();
    }
    public void Unequip(GameEvent gameEvent)
    {
        if (Equipment != null && gameEvent.GetValue<GameObject>(EventParameter.Item) == Equipment)
        {
            GameObject owner = gameEvent.GetValue<GameObject>(EventParameter.Entity);
            owner.FireEvent(GameEventPool.Get(GameEventId.AddToInventory).With(EventParameter.Entity, Equipment), true).Release();
            GameEvent itemUnequiped = GameEventPool.Get(GameEventId.ItemUnequipped)
                                            .With(EventParameter.Owner, owner);
            Equipment.FireEvent(itemUnequiped, true).Release();
            Equipment = null;
        }
    }
    public void PerformAttack(GameEvent gameEvent)
    {
    
    }
    public void Drop(GameEvent gameEvent)
    {
    
    }
    public void Died(GameEvent gameEvent)
    {
    
    }
    public void GetBodyPartType(GameEvent gameEvent)
    {
    
    }
    public void CheckItemEquiped(GameEvent gameEvent)
    {
    
    }
    public void EndTurn(GameEvent gameEvent)
    {
    
    }
}
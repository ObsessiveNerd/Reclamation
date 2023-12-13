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
        RegisteredEvents.Add(GameEventId.ItemEquipped);
        //RegisteredEvents.Add(GameEventId.Equip);
        RegisteredEvents.Add(GameEventId.Unequip);
        //RegisteredEvents.Add(GameEventId.CheckEquipment);
        //RegisteredEvents.Add(GameEventId.GetCombatRating);
        RegisteredEvents.Add(GameEventId.PerformAttack);
        RegisteredEvents.Add(GameEventId.Drop);
        RegisteredEvents.Add(GameEventId.Died);
        RegisteredEvents.Add(GameEventId.GetBodyPartType);
        RegisteredEvents.Add(GameEventId.CheckItemEquiped);
        RegisteredEvents.Add(GameEventId.EndTurn);
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
            GameObject owner = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
            FireEvent(owner, GameEventPool.Get(GameEventId.AddToInventory).With(EventParameter.Entity, EquipmentId), true).Release();
            GameEvent itemUnequiped = GameEventPool.Get(GameEventId.ItemUnequipped)
                                            .With(EventParameter.Owner, owner.ID);
            FireEvent(EntityQuery.GetEntity(EquipmentId), itemUnequiped, true).Release();
            EquipmentId = null;
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

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.AddArmorValue || gameEvent.ID == GameEventId.GetResistances || gameEvent.ID == GameEventId.GetImmunity)
        { 
           
        }
        else if (gameEvent.ID == GameEventId.GetEquipment)
           
        else if (gameEvent.ID == GameEventId.GetWeapon)
        {
            //if (!string.IsNullOrEmpty(EquipmentId))
                //FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);
        }
        else if (gameEvent.ID == GameEventId.ItemEquipped)
        {
            
        }

        else if(gameEvent.ID == GameEventId.GetSpells)
        {
        }

        else if (gameEvent.ID == GameEventId.Unequip)
        {
            
        }

        else if (gameEvent.ID == GameEventId.PerformAttack)
        {
            if (!string.IsNullOrEmpty(EquipmentId))
            {
                GameObject equipmentEntity = EntityQuery.GetEntity(EquipmentId);
                if (equipmentEntity == null)
                    return;

                bool melee = gameEvent.GetValue<bool>(EventParameter.Melee);

                if (equipmentEntity.HasComponent(typeof(TwoHanded)))
                    gameEvent.ContinueProcessing = false;

                CombatUtility.Attack(Self,
                    EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Target]),
                    equipmentEntity,
                    AttackType.Melee);
            }
        }

        else if (gameEvent.ID == GameEventId.Drop)
        {
            if (EquipmentId != null && gameEvent.GetValue<string>(EventParameter.Item) == EquipmentId)
                FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent, true);
        }

        else if (gameEvent.ID == GameEventId.Died)
        {
            if (string.IsNullOrEmpty(EquipmentId))
                return;

            GameEvent builder = GameEventPool.Get(GameEventId.Drop)
                                    .With(EventParameter.Entity, Self.ID)
                                    .With(EventParameter.Item, EquipmentId);

            FireEvent(EntityQuery.GetEntity(EquipmentId), builder, true).Release();
        }

        else if(gameEvent.ID == GameEventId.CheckItemEquiped)
        {
            if (!string.IsNullOrEmpty(EquipmentId) && gameEvent.GetValue<string>(EventParameter.Item) == EquipmentId)
                gameEvent.Paramters[EventParameter.Value] = true;
        }

        else if(gameEvent.ID == GameEventId.EndTurn)
        {
            if(!string.IsNullOrEmpty(EquipmentId))
            {
                GameEvent endTurn = GameEventPool.Get(GameEventId.EndTurn)
                                    .With(EventParameter.Entity, Self.ID);
                var equipment = Services.EntityMapService.GetEntity(EquipmentId);
                equipment.FireEvent(endTurn).Release();
            }
        }

        //if(gameEvent.ID == GameEventId.GetSpells)
        //{
        //    if (!string.IsNullOrEmpty(EquipmentId))
        //        FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);
        //}

        //if(gameEvent.ID == GameEventId.CheckEquipment)
        //{
        //    GameEvent ge = (GameEvent)gameEvent.Paramters[EventParameters.GameEvent];
        //    FireEvent(EntityQuery.GetEntity(EquipmentId), ge);
        //}

        //if(gameEvent.ID == GameEventId.GetCombatRating)
        //{
        //    if(!string.IsNullOrEmpty(EquipmentId))
        //        FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);
        //}

        else if (gameEvent.ID == GameEventId.GetBodyPartType)
        {
            List<BodyPart> desiredBodyParts = gameEvent.GetValue<List<BodyPart>>(EventParameter.DesiredBodyPartTypes);
            foreach (var desiredBodyPart in desiredBodyParts)
            {
                if (BodyPartType == desiredBodyPart)
                {
                    if (!gameEvent.GetValue<Dictionary<BodyPart, List<EntityComponent>>>(EventParameter.BodyParts).ContainsKey(BodyPartType))
                        gameEvent.GetValue<Dictionary<BodyPart, List<EntityComponent>>>(EventParameter.BodyParts).Add(BodyPartType, new List<EntityComponent>());

                    gameEvent.GetValue<Dictionary<BodyPart, List<EntityComponent>>>(EventParameter.BodyParts)[BodyPartType].Add(this);
                }
            }
        }
    }
}
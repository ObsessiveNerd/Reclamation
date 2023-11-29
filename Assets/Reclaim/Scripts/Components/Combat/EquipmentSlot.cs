using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : EntityComponent
{
    private string m_EquipmentId;
    private string m_EquipmentName;

    public string EquipmentId;

    //public string EquipmentInstanceId { get; set; }

    //public string EquipmentName;
    public BodyPart BodyPartType;

    public EquipmentSlot(string eID, BodyPart bp)
    {
        BodyPartType = bp;
        EquipmentId = eID;
        
        //EquipmentInstanceId = entity.ID;

        //if (!string.IsNullOrEmpty(EquipmentId) && !int.TryParse(EquipmentId, out int res))
        //    EquipmentId = EntityFactory.CreateEntity(EquipmentId).ID;
    }

    public override void Start()
    {
        base.Start();
         if (string.IsNullOrEmpty(EquipmentId))
            return;

        if (Services.EntityMapService.ContainsId(EquipmentId))
            return;

        var entity = EntityQuery.GetEntity(EquipmentId);
        if (entity == null)
        {
            entity = EntityFactory.CreateEntity(EquipmentId);
            if (entity == null)
                Debug.LogError(EquipmentId);
            Services.EntityMapService.AddEntity(entity);

            //GameEvent register = GameEventPool.Get(GameEventId.RegisterEntity)
            //                        .With(EventParameters.Entity, entity);
            //World.Instance.Self.FireEvent(register.CreateEvent());
        }

        if (entity != null)
        {
            //EquipmentId = entity.ID;
            EquipmentId = "";
            GameEvent equip = GameEventPool.Get(GameEventId.Equip)
                                .With(EventParameter.Equipment, entity.ID)
                                .With(EventParameter.EntityType, BodyPartType);
            FireEvent(Self, equip, true).Release();
        }
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetImmunity);
        RegisteredEvents.Add(GameEventId.GetResistances);
        RegisteredEvents.Add(GameEventId.AddArmorValue);
        RegisteredEvents.Add(GameEventId.GetSpells);
        RegisteredEvents.Add(GameEventId.GetWeapon);
        RegisteredEvents.Add(GameEventId.GetEquipment);
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
        if (gameEvent.ID == GameEventId.AddArmorValue || gameEvent.ID == GameEventId.GetResistances || gameEvent.ID == GameEventId.GetImmunity)
        { 
            if (!string.IsNullOrEmpty(EquipmentId))
                FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);
        }
        else if (gameEvent.ID == GameEventId.GetEquipment)
            gameEvent.Paramters[EventParameter.Equipment] = EquipmentId;
        else if (gameEvent.ID == GameEventId.GetWeapon)
        {
            if (!string.IsNullOrEmpty(EquipmentId))
                FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);
        }
        else if (gameEvent.ID == GameEventId.ItemEquipped)
        {
            var entity = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Equipment));
            if (entity == null)
            {
                Debug.LogError($"Attempting to equip a null item, {gameEvent.GetValue<string>(EventParameter.Equipment)}");
                return;
            }
            FireEvent(entity, gameEvent);
            EquipmentId = (string)gameEvent.Paramters[EventParameter.Equipment];

            GameEvent playOnEquipSound = GameEventPool.Get(GameEventId.Playsound)
                                        .With(EventParameter.SoundSource, Self.ID)
                                        .With(EventParameter.Key, SoundKey.Activate);
            entity.FireEvent(playOnEquipSound).Release();
        }

        else if(gameEvent.ID == GameEventId.GetSpells)
        {
            if (!string.IsNullOrEmpty(EquipmentId))
                FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);
        }

        else if (gameEvent.ID == GameEventId.Unequip)
        {
            if (EquipmentId != null && gameEvent.GetValue<string>(EventParameter.Item) == EquipmentId)
            {
                IEntity owner = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
                FireEvent(owner, GameEventPool.Get(GameEventId.AddToInventory).With(EventParameter.Entity, EquipmentId), true).Release();
                GameEvent itemUnequiped = GameEventPool.Get(GameEventId.ItemUnequipped)
                                            .With(EventParameter.Owner, owner.ID);
                FireEvent(EntityQuery.GetEntity(EquipmentId), itemUnequiped, true).Release();
                EquipmentId = null;
                //EquipmentName = "";
            }
        }

        else if (gameEvent.ID == GameEventId.PerformAttack)
        {
            if (!string.IsNullOrEmpty(EquipmentId))
            {
                IEntity equipmentEntity = EntityQuery.GetEntity(EquipmentId);
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

public class DTO_EquipmentSlot : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        var parsedData = data.Split(',');

        string id = "";
        BodyPart bp = BodyPart.Arm;

        foreach(var kvp in parsedData)
        {
            string key = kvp.Split('=')[0];
            string value = kvp.Split('=')[1];

            switch(key)
            {
                case "EquipmentId":
                    id = value;
                    break;
                case "BodyPartType":
                    bp = (BodyPart)Enum.Parse(typeof(BodyPart), value);
                    break;
            }
        }
        Component = new EquipmentSlot(id, bp);
    }

    public string CreateSerializableData(IComponent component)
    {
        EquipmentSlot es = (EquipmentSlot)component;
        string id = es.EquipmentId;
        if(!Guid.TryParse(id, out Guid result))
            es.Start();

        id = es.EquipmentId;
        if(!string.IsNullOrEmpty(id))
            EntityFactory.CreateTemporaryBlueprint(id, EntityQuery.GetEntity(id).Serialize());
        return $"{nameof(EquipmentSlot)}: {nameof(es.BodyPartType)}={es.BodyPartType}, {nameof(es.EquipmentId)}={id}";
    }
}

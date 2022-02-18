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
                                .With(EventParameters.Equipment, entity.ID)
                                .With(EventParameters.EntityType, BodyPartType);
            FireEvent(Self, equip).Release();
        }
    }

    public override void Init(IEntity self)
    {
        RegisteredEvents.Add(GameEventId.GetImmunity);
        RegisteredEvents.Add(GameEventId.GetResistances);
        RegisteredEvents.Add(GameEventId.AddArmorValue);
        RegisteredEvents.Add(GameEventId.GetSpells);
        RegisteredEvents.Add(GameEventId.GetWeapon);
        RegisteredEvents.Add(GameEventId.GetEquipment);
        RegisteredEvents.Add(GameEventId.GetSpells);
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
        base.Init(self);
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
            gameEvent.Paramters[EventParameters.Equipment] = EquipmentId;
        else if (gameEvent.ID == GameEventId.GetWeapon)
        {
            if (!string.IsNullOrEmpty(EquipmentId))
                FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);
        }
        else if (gameEvent.ID == GameEventId.ItemEquipped)
        {
            var entity = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Equipment));
            if (entity == null)
            {
                Debug.LogError("Attempting to equip a null item");
                return;
            }
            FireEvent(entity, gameEvent);
            EquipmentId = (string)gameEvent.Paramters[EventParameters.Equipment];

            GameEvent playOnEquipSound = GameEventPool.Get(GameEventId.Playsound)
                                        .With(EventParameters.SoundSource, Self.ID)
                                        .With(EventParameters.Key, SoundKey.Activate);
            entity.FireEvent(playOnEquipSound).Release();
        }

        else if(gameEvent.ID == GameEventId.GetSpells)
        {
            if (!string.IsNullOrEmpty(EquipmentId))
                FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);
        }

        else if (gameEvent.ID == GameEventId.Unequip)
        {
            if (EquipmentId != null && gameEvent.GetValue<string>(EventParameters.Item) == EquipmentId)
            {
                IEntity owner = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
                FireEvent(owner, GameEventPool.Get(GameEventId.AddToInventory).With(EventParameters.Entity, EquipmentId)).Release();
                FireEvent(EntityQuery.GetEntity(EquipmentId), GameEventPool.Get(GameEventId.ItemUnequipped)).Release();
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

                bool melee = gameEvent.GetValue<bool>(EventParameters.Melee);

                if (equipmentEntity.HasComponent(typeof(TwoHanded)))
                    gameEvent.ContinueProcessing = false;

                CombatUtility.Attack(Self,
                    EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Target]),
                    equipmentEntity,
                    AttackType.Melee);
            }
        }

        else if (gameEvent.ID == GameEventId.Drop)
        {
            if (EquipmentId != null && gameEvent.GetValue<string>(EventParameters.Item) == EquipmentId)
                FireEvent(EntityQuery.GetEntity(EquipmentId), gameEvent);
        }

        else if (gameEvent.ID == GameEventId.Died)
        {
            if (string.IsNullOrEmpty(EquipmentId))
                return;

            GameEvent builder = GameEventPool.Get(GameEventId.Drop)
                                    .With(EventParameters.Entity, Self.ID)
                                    .With(EventParameters.Item, EquipmentId);

            FireEvent(EntityQuery.GetEntity(EquipmentId), builder).Release();
        }

        else if(gameEvent.ID == GameEventId.CheckItemEquiped)
        {
            if (gameEvent.GetValue<string>(EventParameters.Item) == EquipmentId)
                gameEvent.Paramters[EventParameters.Value] = true;
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

        else if(gameEvent.ID == GameEventId.GetBodyPartType)
        {
            List<BodyPart> desiredBodyParts = gameEvent.GetValue<List<BodyPart>>(EventParameters.DesiredBodyPartTypes);
            foreach (var desiredBodyPart in desiredBodyParts)
            {
                if (BodyPartType == desiredBodyPart)
                {
                    if (!gameEvent.GetValue<Dictionary<BodyPart, List<EntityComponent>>>(EventParameters.BodyParts).ContainsKey(BodyPartType))
                        gameEvent.GetValue<Dictionary<BodyPart, List<EntityComponent>>>(EventParameters.BodyParts).Add(BodyPartType, new List<EntityComponent>());

                    gameEvent.GetValue<Dictionary<BodyPart, List<EntityComponent>>>(EventParameters.BodyParts)[BodyPartType].Add(this);
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
        //string id = string.IsNullOrEmpty(es.EquipmentInstanceId) ? es.EquipmentId : es.EquipmentInstanceId;
        string id = es.EquipmentId;
        if(!string.IsNullOrEmpty(id))
            EntityFactory.CreateTemporaryBlueprint(id, EntityQuery.GetEntity(id).Serialize());
        return $"{nameof(EquipmentSlot)}: {nameof(es.BodyPartType)}={es.BodyPartType}, {nameof(es.EquipmentId)}={id}";
    }
}

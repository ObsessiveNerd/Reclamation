using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public enum BodyPart
{
    Head,
    Torso,
    Arm,
    Leg
}

public class Body : Component
{
    public IEntity Torso;
    public List<IEntity> Head = new List<IEntity>();
    public List<IEntity> Arm = new List<IEntity>();
    public List<IEntity> Leg = new List<IEntity>();

    private List<IEntity> m_AllBodyParts
    {
        get
        {
            List<IEntity> list = new List<IEntity>();
            list.Add(Torso);
            list.AddRange(Head);
            list.AddRange(Arm);
            list.AddRange(Leg);
            return list;
        }
    }

    public Body(int numHeads, int numArms = 0, int numLegs = 0)
    {
        Actor body = new Actor("Body");
        body.AddComponent(new EquipmentSlot());
        body.CleanupComponents();
        Torso = body;

        for (int i = 0; i < numHeads; i++)
            GrowBodyPart(BodyPart.Head);

        for (int i = 0; i < numArms; i++)
            GrowBodyPart(BodyPart.Arm);

        for (int i = 0; i < numLegs; i++)
            GrowBodyPart(BodyPart.Leg);

        RegisteredEvents.Add(GameEventId.SeverBodyPart);
        RegisteredEvents.Add(GameEventId.GrowBodyPart);
        RegisteredEvents.Add(GameEventId.AddArmorValue);
        RegisteredEvents.Add(GameEventId.PerformAttack);
        RegisteredEvents.Add(GameEventId.GetRangedWeapon);
        RegisteredEvents.Add(GameEventId.Equip);
        RegisteredEvents.Add(GameEventId.Unequip);
        RegisteredEvents.Add(GameEventId.CheckEquipment);
        RegisteredEvents.Add(GameEventId.EndTurn);
        RegisteredEvents.Add(GameEventId.GetCombatRating);
        RegisteredEvents.Add(GameEventId.GetCurrentEquipment);
        RegisteredEvents.Add(GameEventId.CheckItemEquiped);
    }

    bool HasEquipment(IEntity e)
    {
        GameEvent result = FireEvent(e, new GameEvent(GameEventId.GetEquipment, new KeyValuePair<string, object>(EventParameters.Equipment, null)));
        return result.Paramters[EventParameters.Equipment] != null;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.SeverBodyPart)
        {
            BodyPart bp = (BodyPart)gameEvent.Paramters[EventParameters.EntityType];
            IEntity dropTarget = null;
            switch (bp)
            {
                case BodyPart.Head:
                    if (Head.Count > 0)
                    {
                        dropTarget = Head[0];
                        Head.RemoveAt(0);
                    }
                    break;
                case BodyPart.Arm:
                    if (Arm.Count > 0)
                    {
                        dropTarget = Arm[0];
                        Arm.RemoveAt(0);
                    }
                    break;
                case BodyPart.Leg:
                    if (Leg.Count > 0)
                    {
                        dropTarget = Leg[0];
                        Leg.RemoveAt(0);
                    }
                    break;
            }

            if (dropTarget != null)
            {
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.Drop, new KeyValuePair<string, object>(EventParameters.Entity, dropTarget.ID),
                                                                                new KeyValuePair<string, object>(EventParameters.Creature, Self.ID),
                                                                                new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Item)));
            }
        }

        //TODO this only works for bipedal monsters
        else if(gameEvent.ID == GameEventId.GetCurrentEquipment)
        {
            gameEvent.Paramters[EventParameters.Head] = GetEquipmentIdForBodyPart(Head[0]);
            gameEvent.Paramters[EventParameters.Torso] = GetEquipmentIdForBodyPart(Torso);
            if(Arm.Count > 0)
                gameEvent.Paramters[EventParameters.LeftArm] = GetEquipmentIdForBodyPart(Arm[0]);
            if(Arm.Count > 1)
                gameEvent.Paramters[EventParameters.RightArm] = GetEquipmentIdForBodyPart(Arm[1]);
            gameEvent.Paramters[EventParameters.Legs] = GetEquipmentIdForBodyPart(Leg[0]);
        }

        else if(gameEvent.ID == GameEventId.CheckItemEquiped)
        {
            string equipmentId = gameEvent.GetValue<string>(EventParameters.Item);
            foreach(var bodyPart in m_AllBodyParts)
            {
                string equipedId = GetEquipmentIdForBodyPart(bodyPart);
                if(equipedId == equipmentId)
                {
                    gameEvent.Paramters[EventParameters.Value] = true;
                    break;
                }
            }
        }

        else if(gameEvent.ID == GameEventId.Unequip)
        {
            foreach (var bp in m_AllBodyParts)
                FireEvent(bp, gameEvent);
        }

        else if(gameEvent.ID == GameEventId.Equip)
        {
            BodyPart bp = (BodyPart)gameEvent.Paramters[EventParameters.EntityType];
            List<IEntity> target = new List<IEntity>();
            switch(bp)
            {
                //todo: all of this is temp.  we'll have to iterate through and see what open slots are avalible
                case BodyPart.Head:
                    target = Head;
                    break;
                case BodyPart.Arm:
                    target = Arm;
                    break;
                case BodyPart.Leg:
                    target = Leg;
                    break;
                case BodyPart.Torso:
                    target = new List<IEntity>() { Torso };
                    break;
            }

            bool equipedItem = false;
            foreach (IEntity e in target)
            {
                if (!HasEquipment(e))
                {
                    equipedItem = true;
                    FireEvent(e, gameEvent);
                    FireEvent(Self, new GameEvent(GameEventId.RemoveFromInventory, 
                        new KeyValuePair<string, object>(EventParameters.Entity, gameEvent.Paramters[EventParameters.Equipment])));
                    break;
                }
            }
            if(!equipedItem)
            {
                //Todo: give a prompt that nothing was equiped and something needs to be unequiped
            }
        }

        else if (gameEvent.ID == GameEventId.AddArmorValue || gameEvent.ID == GameEventId.GetCombatRating)
        {
            FireEvent(Torso, gameEvent);
            foreach(var head in Head)
                FireEvent(head, gameEvent);
            foreach (var arm in Arm)
                FireEvent(arm, gameEvent);
            foreach (var leg in Leg)
                FireEvent(leg, gameEvent);
        }

        else if (gameEvent.ID == GameEventId.GetRangedWeapon)
        {
            foreach (IEntity hand in Arm)
            {
                IEntity equipment = (IEntity)FireEvent(hand, new GameEvent(GameEventId.GetEquipment, new KeyValuePair<string, object>(EventParameters.Equipment, null))).Paramters[EventParameters.Equipment];
                if (equipment != null)
                {
                    TypeWeapon weaponType = (TypeWeapon)FireEvent(equipment, new GameEvent(GameEventId.GetWeaponType,
                           new KeyValuePair<string, object>(EventParameters.WeaponType, null))).Paramters[EventParameters.WeaponType];
                    if (weaponType == TypeWeapon.Ranged)
                    {
                        gameEvent.Paramters[EventParameters.Value] = equipment;
                        break;
                    }
                }
            }
        }

        else if (gameEvent.ID == GameEventId.PerformAttack)
        {
            TypeWeapon desiredWeaponToAttack = (TypeWeapon)gameEvent.Paramters[EventParameters.WeaponType];
            foreach (IEntity hand in Arm)
            {
                EventBuilder getEquipment = new EventBuilder(GameEventId.GetEquipment)
                                            .With(EventParameters.Equipment, null);

                string equipment = FireEvent(hand, getEquipment.CreateEvent()).GetValue<string>(EventParameters.Equipment);
                IEntity equipmentEntity = EntityQuery.GetEntity(equipment);

                if (equipmentEntity == null)
                    equipmentEntity = EntityFactory.CreateEntity("UnarmedStrike");

                if (equipmentEntity != null && CombatUtility.GetWeaponType(equipmentEntity).HasFlag(desiredWeaponToAttack))
                    CombatUtility.Attack(Self, EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Target]), equipmentEntity);
            }
        }

        else if (gameEvent.ID == GameEventId.GrowBodyPart)
        {
            BodyPart bodyPartType = (BodyPart)gameEvent.Paramters[EventParameters.EntityType];
            GrowBodyPart(bodyPartType);
        }

        else if (gameEvent.ID == GameEventId.CheckEquipment)
        {
            foreach(IEntity bodyPart in m_AllBodyParts)
                FireEvent(bodyPart, gameEvent);
        }

        else if (gameEvent.ID == GameEventId.EndTurn)
        {
            GameEvent ge = new GameEvent(GameEventId.CheckEquipment, new KeyValuePair<string, object>(EventParameters.GameEvent, gameEvent));
            foreach (IEntity bodyPart in m_AllBodyParts)
                FireEvent(bodyPart, ge);
        }
    }

    void GrowBodyPart(BodyPart bodyPartType)
    {
        switch (bodyPartType)
        {
            case BodyPart.Head:
                Actor head = new Actor("Head");
                head.AddComponent(new EquipmentSlot());
                head.CleanupComponents();
                Head.Add(head);
                break;
            case BodyPart.Arm:
                Actor arm = new Actor("Arm");
                arm.AddComponent(new EquipmentSlot());
                arm.CleanupComponents();
                Arm.Add(arm);
                break;
            case BodyPart.Leg:
                Actor leg = new Actor("Leg");
                leg.AddComponent(new EquipmentSlot());
                leg.CleanupComponents();
                Leg.Add(leg);
                break;
        }
    }

    string GetEquipmentIdForBodyPart(IEntity bodyPart)
    {
        EventBuilder getEquipment = new EventBuilder(GameEventId.GetEquipment)
                                            .With(EventParameters.Equipment, null);

        string equipment = FireEvent(bodyPart, getEquipment.CreateEvent()).GetValue<string>(EventParameters.Equipment);

        return equipment;
    }
}

public class DTO_Body : IDataTransferComponent
{
    public IComponent Component { get; set; }

    int heads = 1;
    int arms = 0;
    int legs = 0;

    public void CreateComponent(string data)
    {
        Dictionary<BodyPart, List<IEntity>> equipment = new Dictionary<BodyPart, List<IEntity>>();
        string[] bodyParts = data.Split(',');
        foreach(string bp in bodyParts)
        {
            string[] bpTypeToCount = bp.Split('=');
            BodyPart bpType = (BodyPart)Enum.Parse(typeof(BodyPart), bpTypeToCount[0]);
            int bpCount = 0;
            if (bpTypeToCount[1].Contains("["))
                bpCount = int.Parse(bpTypeToCount[1].Substring(0, bpTypeToCount[1].IndexOf('[')));
            else
                bpCount = int.Parse(bpTypeToCount[1]);
            string equipmentData = null;
            if (bpTypeToCount[1].Contains("["))
                equipmentData = bpTypeToCount[1].Substring(bpTypeToCount[1].IndexOf('['));
            //else
            //    equipmentData = bpTypeToCount[1];

            if (!equipment.ContainsKey(bpType))
                equipment[bpType] = new List<IEntity>();
            equipment[bpType].AddRange(EntityFactory.GetEntitiesFromArray(equipmentData));

            switch (bpType)
            {
                case BodyPart.Arm:
                    arms = bpCount;
                    break;
                case BodyPart.Head:
                    heads = bpCount;
                    break;
                case BodyPart.Leg:
                    legs = bpCount;
                    break;
            }
        }
        Component = new Body(heads, arms, legs);

        foreach (var key in equipment.Keys)
        {
            foreach (var e in equipment[key])
                if (e != null)
                    Component.HandleEvent(new GameEvent(GameEventId.Equip, new KeyValuePair<string, object>(EventParameters.EntityType, key),
                                                                        new KeyValuePair<string, object>(EventParameters.Equipment, e.ID)));
        }
    }

    public string CreateSerializableData(IComponent component)
    {
        Body body = (Body)component;

        string headEquipment = GetSerializedEquipment(body.Head);
        string torsoEquipment = GetSerializedEquipment(new List<IEntity>() { body.Torso });
        string armEquipment = GetSerializedEquipment(body.Arm);
        string legEquipment = GetSerializedEquipment(body.Leg);

        return $"Body:Head={body.Head.Count}[{headEquipment}], Torso=1[{torsoEquipment}], Arm={body.Arm.Count}[{armEquipment}], Leg={body.Leg.Count}[{legEquipment}]";
    }

    string GetSerializedEquipment(IList<IEntity> bodyParts)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var bp in bodyParts)
        {
            if(bp == null) continue;

            GameEvent getEquipment = new GameEvent(GameEventId.GetEquipment, new KeyValuePair<string, object>(EventParameters.Equipment, null));
            bp.HandleEvent(getEquipment);
            string equipment = (string)getEquipment.Paramters[EventParameters.Equipment];
            if (equipment != null)
            {
                sb.Append($"<{equipment}>&");
                EntityFactory.CreateTemporaryBlueprint(equipment, EntityQuery.GetEntity(equipment).Serialize()); //todo: feed proper seed
            }
        }
        string value = sb.ToString().TrimEnd('&');
        return value;
    }
}

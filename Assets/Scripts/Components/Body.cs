using System;
using System.Collections;
using System.Collections.Generic;
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
    IEntity m_Body;
    List<IEntity> m_Head = new List<IEntity>();
    List<IEntity> m_Arms = new List<IEntity>();
    List<IEntity> m_Legs = new List<IEntity>();

    public Body(int numHeads, int numArms = 0, int numLegs = 0)
    {
        Actor body = new Actor("Body");
        body.AddComponent(new EquipmentSlot());
        body.CleanupComponents();
        m_Body = body;

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
                    if (m_Head.Count > 0)
                    {
                        dropTarget = m_Head[0];
                        m_Head.RemoveAt(0);
                    }
                    break;
                case BodyPart.Arm:
                    if (m_Arms.Count > 0)
                    {
                        dropTarget = m_Arms[0];
                        m_Arms.RemoveAt(0);
                    }
                    break;
                case BodyPart.Leg:
                    if (m_Legs.Count > 0)
                    {
                        dropTarget = m_Legs[0];
                        m_Legs.RemoveAt(0);
                    }
                    break;
            }

            if(dropTarget != null)
                Spawner.Spawn(dropTarget, EntityType.Item, World.Instance.GetPointWhereEntityIs(Self));
        }

        if(gameEvent.ID == GameEventId.Equip)
        {
            BodyPart bp = (BodyPart)gameEvent.Paramters[EventParameters.EntityType];
            List<IEntity> target = new List<IEntity>();
            switch(bp)
            {
                //todo: all of this is temp.  we'll have to iterate through and see what open slots are avalible
                case BodyPart.Head:
                    target = m_Head;
                    break;
                case BodyPart.Arm:
                    target = m_Arms;
                    break;
                case BodyPart.Leg:
                    target = m_Legs;
                    break;
                case BodyPart.Torso:
                    target = new List<IEntity>() { m_Body };
                    break;
            }

            bool equipedItem = false;
            foreach (IEntity e in target)
            {
                if (!HasEquipment(e))
                {
                    equipedItem = true;
                    FireEvent(e, gameEvent);
                    break;
                }
            }
            if(!equipedItem)
            {
                //Todo: give a prompt that nothing was equiped and something needs to be unequiped
            }
        }

        if(gameEvent.ID == GameEventId.AddArmorValue)
        {
            FireEvent(m_Body, gameEvent);
            foreach(var head in m_Head)
                FireEvent(head, gameEvent);
            foreach (var arm in m_Arms)
                FireEvent(arm, gameEvent);
            foreach (var leg in m_Legs)
                FireEvent(leg, gameEvent);
        }

        if (gameEvent.ID == GameEventId.GetRangedWeapon)
        {
            foreach (IEntity hand in m_Arms)
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

        if (gameEvent.ID == GameEventId.PerformAttack)
        {
            TypeWeapon desiredWeaponToAttack = (TypeWeapon)gameEvent.Paramters[EventParameters.WeaponType];
            foreach (IEntity hand in m_Arms)
            {
                IEntity equipment = (IEntity)FireEvent(hand, new GameEvent(GameEventId.GetEquipment, new KeyValuePair<string, object>(EventParameters.Equipment, null))).Paramters[EventParameters.Equipment];
                if (equipment != null && CombatUtility.GetWeaponType(equipment).HasFlag(desiredWeaponToAttack))
                    CombatUtility.Attack(Self, (IEntity)gameEvent.Paramters[EventParameters.Target], equipment);
            }
        }

        if(gameEvent.ID == GameEventId.GrowBodyPart)
        {
            BodyPart bodyPartType = (BodyPart)gameEvent.Paramters[EventParameters.EntityType];
            GrowBodyPart(bodyPartType);
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
                m_Head.Add(head);
                break;
            case BodyPart.Arm:
                Actor arm = new Actor("Arm");
                arm.AddComponent(new EquipmentSlot());
                arm.CleanupComponents();
                m_Arms.Add(arm);
                break;
            case BodyPart.Leg:
                Actor leg = new Actor("Leg");
                leg.AddComponent(new EquipmentSlot());
                leg.CleanupComponents();
                m_Legs.Add(leg);
                break;
        }
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
            int bpCount = int.Parse(bpTypeToCount[1].Substring(0, bpTypeToCount[1].IndexOf('[')));
            string equipmentData = bpTypeToCount[1].Substring(bpTypeToCount[1].IndexOf('['));
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

        foreach(var key in equipment.Keys)
        {
            foreach (var e in equipment[key])
                Component.HandleEvent(new GameEvent(GameEventId.Equip, new KeyValuePair<string, object>(EventParameters.EntityType, key),
                                                                        new KeyValuePair<string, object>(EventParameters.Equipment, e)));
        }
    }
}

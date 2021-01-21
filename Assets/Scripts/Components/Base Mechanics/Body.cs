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
    public List<IEntity> Heads = new List<IEntity>();
    public List<IEntity> Arms = new List<IEntity>();
    public List<IEntity> Legs = new List<IEntity>();

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
                    if (Heads.Count > 0)
                    {
                        dropTarget = Heads[0];
                        Heads.RemoveAt(0);
                    }
                    break;
                case BodyPart.Arm:
                    if (Arms.Count > 0)
                    {
                        dropTarget = Arms[0];
                        Arms.RemoveAt(0);
                    }
                    break;
                case BodyPart.Leg:
                    if (Legs.Count > 0)
                    {
                        dropTarget = Legs[0];
                        Legs.RemoveAt(0);
                    }
                    break;
            }

            if (dropTarget != null)
            {
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.Drop, new KeyValuePair<string, object>(EventParameters.Entity, dropTarget),
                                                                                new KeyValuePair<string, object>(EventParameters.Creature, Self),
                                                                                new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Item)));
            }
        }

        if(gameEvent.ID == GameEventId.Equip)
        {
            BodyPart bp = (BodyPart)gameEvent.Paramters[EventParameters.EntityType];
            List<IEntity> target = new List<IEntity>();
            switch(bp)
            {
                //todo: all of this is temp.  we'll have to iterate through and see what open slots are avalible
                case BodyPart.Head:
                    target = Heads;
                    break;
                case BodyPart.Arm:
                    target = Arms;
                    break;
                case BodyPart.Leg:
                    target = Legs;
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
            FireEvent(Torso, gameEvent);
            foreach(var head in Heads)
                FireEvent(head, gameEvent);
            foreach (var arm in Arms)
                FireEvent(arm, gameEvent);
            foreach (var leg in Legs)
                FireEvent(leg, gameEvent);
        }

        if (gameEvent.ID == GameEventId.GetRangedWeapon)
        {
            foreach (IEntity hand in Arms)
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
            foreach (IEntity hand in Arms)
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
                Heads.Add(head);
                break;
            case BodyPart.Arm:
                Actor arm = new Actor("Arm");
                arm.AddComponent(new EquipmentSlot());
                arm.CleanupComponents();
                Arms.Add(arm);
                break;
            case BodyPart.Leg:
                Actor leg = new Actor("Leg");
                leg.AddComponent(new EquipmentSlot());
                leg.CleanupComponents();
                Legs.Add(leg);
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

    public string CreateSerializableData(IComponent component)
    {
        Body body = (Body)component;

        string headEquipment = GetSerializedEquipment(body.Heads);
        string torsoEquipment = GetSerializedEquipment(new List<IEntity>() { body.Torso });
        string armEquipment = GetSerializedEquipment(body.Arms);
        string legEquipment = GetSerializedEquipment(body.Legs);

        return $"Body:Head={body.Heads.Count}[{headEquipment}], Torso=1[{torsoEquipment}], Arm={body.Arms.Count}[{armEquipment}], Leg={body.Legs.Count}[{legEquipment}]";
    }

    string GetSerializedEquipment(IList<IEntity> bodyParts)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var bp in bodyParts)
        {
            GameEvent getEquipment = new GameEvent(GameEventId.GetEquipment, new KeyValuePair<string, object>(EventParameters.Equipment, null));
            bp.HandleEvent(getEquipment);
            IEntity equipment = (IEntity)getEquipment.Paramters[EventParameters.Equipment];
            if (equipment != null)
            {
                sb.Append($"<{equipment.ID}>&");
                EntityFactory.CreateTemporaryBlueprint($"{SaveSystem.kSaveDataPath}/{World.Instance.Seed}", equipment.ID, equipment.Serialize()); //todo: feed proper seed
            }
        }
        string value = sb.ToString().TrimEnd('&');
        return value;
    }
}

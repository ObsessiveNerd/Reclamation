using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public enum BodyPart
{
    Head,
    Torso,
    Arm,
    Leg,
    Finger,
    Back,
    Neck
}

public class Body : Component
{
    public Body()
    {
        //RegisteredEvents.Add(GameEventId.SeverBodyPart);
        //    //RegisteredEvents.Add(GameEventId.GrowBodyPart);
        RegisteredEvents.Add(GameEventId.AddArmorValue);
        RegisteredEvents.Add(GameEventId.PerformAttack);
        RegisteredEvents.Add(GameEventId.GetRangedWeapon);
        RegisteredEvents.Add(GameEventId.GetWeapon);
        RegisteredEvents.Add(GameEventId.Equip);
        RegisteredEvents.Add(GameEventId.Unequip);
        //RegisteredEvents.Add(GameEventId.Equip2Hand);
        RegisteredEvents.Add(GameEventId.CheckEquipment);
        RegisteredEvents.Add(GameEventId.EndTurn);
        RegisteredEvents.Add(GameEventId.GetCombatRating);
        RegisteredEvents.Add(GameEventId.GetCurrentEquipment);
        RegisteredEvents.Add(GameEventId.GetSpells);
        RegisteredEvents.Add(GameEventId.CheckItemEquiped);
        RegisteredEvents.Add(GameEventId.Died);
    }

    //public Body(int numHeads, int numArms = 0, int numLegs = 0)
    //{
    //    //RegisteredEvents.Add(GameEventId.SeverBodyPart);
    //    //RegisteredEvents.Add(GameEventId.GrowBodyPart);
    //    RegisteredEvents.Add(GameEventId.AddArmorValue);
    //    RegisteredEvents.Add(GameEventId.PerformAttack);
    //    RegisteredEvents.Add(GameEventId.GetRangedWeapon);
    //    RegisteredEvents.Add(GameEventId.GetWeapon);
    //    RegisteredEvents.Add(GameEventId.Equip);
    //    RegisteredEvents.Add(GameEventId.Unequip);
    //    RegisteredEvents.Add(GameEventId.CheckEquipment);
    //    RegisteredEvents.Add(GameEventId.EndTurn);
    //    RegisteredEvents.Add(GameEventId.GetCombatRating);
    //    RegisteredEvents.Add(GameEventId.GetCurrentEquipment);
    //    RegisteredEvents.Add(GameEventId.GetSpells);
    //    RegisteredEvents.Add(GameEventId.CheckItemEquiped);
    //    RegisteredEvents.Add(GameEventId.Died);
    //}

    //public override void Start()
    //{
    //    foreach (GameEvent ge in OnSpawn)
    //        HandleEvent(ge);
    //    OnSpawn.Clear();
    //}

    //List<GameEvent> OnSpawn = new List<GameEvent>();
    //public void EquipOnSpawn(BodyPart bp, string id)
    //{
    //    OnSpawn.Add(GameEventPool.Get(GameEventId.Equip, new .With(EventParameters.EntityType, bp),
    //                                                                    new .With(EventParameters.Equipment, id)));
    //}

    bool HasEquipment(Component e)
    {
        GameEvent ge = GameEventPool.Get(GameEventId.GetEquipment).With(EventParameters.Equipment, null);
        //GameEvent result = FireEvent(e, );
        e.HandleEvent(ge);
        var ret = !string.IsNullOrEmpty((string)ge.Paramters[EventParameters.Equipment]);
        ge.Release();
        return ret;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Equip)
        {
            BodyPart bp = (BodyPart)gameEvent.Paramters[EventParameters.EntityType];

            GameEvent getBpCount = GameEventPool.Get(GameEventId.GetMultiBodyPartUse)
                .With(EventParameters.Value, 1);
            int numberOfBodyPartsRequired = FireEvent(
                EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Equipment)),
                getBpCount).GetValue<int>(EventParameters.Value);
            getBpCount.Release();

            GameEvent builder = GameEventPool.Get(GameEventId.GetBodyPartType)
                                    .With(EventParameters.BodyParts, new Dictionary<BodyPart, List<Component>>())
                                    .With(EventParameters.DesiredBodyPartTypes, new List<BodyPart>(){ bp });

            Dictionary<BodyPart, List<Component>> target = FireEvent(Self, builder).GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts);
            if (!target.ContainsKey(bp))
            {
                builder.Release();
                return;
            }

            if (target[bp].Count < numberOfBodyPartsRequired)
            {
                builder.Release();
                return;
            }

            int bodyPartsTaken = 0;
            bool unequipThroughIteration = false;
            for (int i = 0; i < target[bp].Count && bodyPartsTaken < numberOfBodyPartsRequired; i++)
            {
                if (GetEquipmentIdForBodyPart(target[bp][i]) == gameEvent.GetValue<string>(EventParameters.Equipment))
                    break;

                if (unequipThroughIteration && GetEquipmentIdForBodyPart(target[bp][i]) != gameEvent.GetValue<string>(EventParameters.Equipment))
                {
                    GameEvent unequip = GameEventPool.Get(GameEventId.Unequip)
                                            .With(EventParameters.Entity, Self.ID)
                                            .With(EventParameters.Item, GetEquipmentIdForBodyPart(target[bp][i]));
                    FireEvent(Self, unequip).Release();
                }

                if (!HasEquipment(target[bp][i]))
                {
                    bodyPartsTaken++;

                    GameEvent itemEquiped = GameEventPool.Get(GameEventId.ItemEquipped)
                                                .With(EventParameters.Equipment, gameEvent.GetValue<string>(EventParameters.Equipment))
                                                .With(EventParameters.Owner, Self?.ID);

                    target[bp][i].HandleEvent(itemEquiped);
                    FireEvent(Self, GameEventPool.Get(GameEventId.RemoveFromInventory)
                            .With(EventParameters.Entity, gameEvent.Paramters[EventParameters.Equipment])).Release();
                    itemEquiped.Release();
                }

                if(i == target[bp].Count - 1 && bodyPartsTaken < numberOfBodyPartsRequired)
                {
                    i = -1;
                    if(!unequipThroughIteration)
                        unequipThroughIteration = true;
                    else
                        throw new Exception($"Attempted to equip item {EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Equipment)).Name}");
                }
            }

            builder.Release();
        }
        else if (gameEvent.ID == GameEventId.PerformAttack)
        {
            GameEvent builder = GameEventPool.Get(GameEventId.GetBodyPartType)
                                    .With(EventParameters.BodyParts, new Dictionary<BodyPart, List<Component>>())
                                    .With(EventParameters.DesiredBodyPartTypes, new List<BodyPart>(){ BodyPart.Arm, BodyPart.Leg, BodyPart.Torso, BodyPart.Head });

            var result = FireEvent(Self, builder);
            if (!result.HasParameter(EventParameters.BodyParts))
            {
                builder.Release();
                return;
            }
            if (!result.GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts).ContainsKey(BodyPart.Arm))
            {
                builder.Release();
                return;
            }

            var arms = result.GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts)[BodyPart.Arm];

            foreach(var arm in arms.Where(a => a.GetType() == typeof(EquipmentSlot)))
            {
                if (!string.IsNullOrEmpty(((EquipmentSlot)arm).EquipmentId))
                {
                    builder.Release();
                    return;
                }
            }

            var equipmentEntity = EntityFactory.CreateEntity("UnarmedStrike");
            CombatUtility.Attack(Self, EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Target]), equipmentEntity, true);
            builder.Release();
        }
        else if(gameEvent.ID == GameEventId.GetCurrentEquipment)
        {
            GameEvent builder = GameEventPool.Get(GameEventId.GetBodyPartType)
                                    .With(EventParameters.BodyParts, new Dictionary<BodyPart, List<Component>>())
                                    .With(EventParameters.DesiredBodyPartTypes, new List<BodyPart>(){ BodyPart.Arm, BodyPart.Leg, BodyPart.Torso, BodyPart.Head });

            var result = FireEvent(Self, builder);

            if(result.GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts).ContainsKey(BodyPart.Head))
                gameEvent.Paramters[EventParameters.Head] = result.GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts)[BodyPart.Head];

            if(result.GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts).ContainsKey(BodyPart.Torso))
                gameEvent.Paramters[EventParameters.Torso] = result.GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts)[BodyPart.Torso];

            if(result.GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts).ContainsKey(BodyPart.Arm))
                gameEvent.Paramters[EventParameters.Arms] = result.GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts)[BodyPart.Arm];

            if(result.GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts).ContainsKey(BodyPart.Leg))
                gameEvent.Paramters[EventParameters.Legs] = result.GetValue<Dictionary<BodyPart, List<Component>>>(EventParameters.BodyParts)[BodyPart.Leg];

            builder.Release();
        }
    }

    //void GrowBodyPart(BodyPart bodyPartType)
    //{
    //    switch (bodyPartType)
    //    {
    //        case BodyPart.Head:
    //            Actor head = new Actor("Head");
    //            head.AddComponent(new EquipmentSlot());
    //            head.CleanupComponents();
    //            Head.Add(head);
    //            break;
    //        case BodyPart.Arm:
    //            Actor arm = new Actor("Arm");
    //            arm.AddComponent(new EquipmentSlot());
    //            arm.CleanupComponents();
    //            Arm.Add(arm);
    //            break;
    //        case BodyPart.Leg:
    //            Actor leg = new Actor("Leg");
    //            leg.AddComponent(new EquipmentSlot());
    //            leg.CleanupComponents();
    //            Leg.Add(leg);
    //            break;
    //    }
    //}

    string GetEquipmentIdForBodyPart(Component bodyPart)
    {
        GameEvent getEquipment = GameEventPool.Get(GameEventId.GetEquipment)
                                            .With(EventParameters.Equipment, null);

        bodyPart.HandleEvent(getEquipment);
        //string equipment = FireEvent(bodyPart, getEquipment.CreateEvent()).GetValue<string>(EventParameters.Equipment);
        var ret = getEquipment.GetValue<string>(EventParameters.Equipment);
        getEquipment.Release();
        return ret;
    }

    //public override string ToString()
    //{
    //    if (Head == null)
    //        return $"Head=1[], Torso=1[], Arm=2[], Leg=2[]";

    //    string headEquipment = GetSerializedEquipment(Head);
    //    string torsoEquipment = GetSerializedEquipment(new List<IEntity>() { Torso });
    //    string armEquipment = GetSerializedEquipment(Arm);
    //    string legEquipment = GetSerializedEquipment(Leg);

    //    return $"Head={Head.Count}[{headEquipment}], Torso=1[{torsoEquipment}], Arm={Arm.Count}[{armEquipment}], Leg={Leg.Count}[{legEquipment}]";
    //}

    private string GetSerializedEquipment(List<IEntity> bodyPart)
    {
        if (bodyPart == null)
            return "";

        StringBuilder sb = new StringBuilder();
        foreach (var bp in bodyPart)
        {
            if(bp == null) continue;

            EquipmentSlot slot = bp.GetComponent<EquipmentSlot>();
            string equipment = EntityQuery.GetEntityName(slot.EquipmentId);
            if (!string.IsNullOrEmpty(equipment))
                sb.Append($"<{equipment}>&");
        }
        string value = sb.ToString().TrimEnd('&');
        return value;
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
        Component = new Body();
        //Dictionary<BodyPart, List<IEntity>> equipment = new Dictionary<BodyPart, List<IEntity>>();
        //string[] bodyParts = data.Split(',');
        //foreach(string bp in bodyParts)
        //{
        //    string[] bpTypeToCount = bp.Split('=');
        //    BodyPart bpType = (BodyPart)Enum.Parse(typeof(BodyPart), bpTypeToCount[0]);
        //    int bpCount = 0;
        //    if (bpTypeToCount[1].Contains("["))
        //        bpCount = int.Parse(bpTypeToCount[1].Substring(0, bpTypeToCount[1].IndexOf('[')));
        //    else
        //        bpCount = int.Parse(bpTypeToCount[1]);
        //    string equipmentData = null;
        //    if (bpTypeToCount[1].Contains("["))
        //        equipmentData = bpTypeToCount[1].Substring(bpTypeToCount[1].IndexOf('['));
        //    //else
        //    //    equipmentData = bpTypeToCount[1];

        //    if (!equipment.ContainsKey(bpType))
        //        equipment[bpType] = new List<IEntity>();
        //    equipment[bpType].AddRange(EntityFactory.GetEntitiesFromArray(equipmentData));

        //    switch (bpType)
        //    {
        //        case BodyPart.Arm:
        //            arms = bpCount;
        //            break;
        //        case BodyPart.Head:
        //            heads = bpCount;
        //            break;
        //        case BodyPart.Leg:
        //            legs = bpCount;
        //            break;
        //    }
        //}
        //Component = new Body(heads, arms, legs);

        //foreach (var key in equipment.Keys)
        //{
        //    foreach (var e in equipment[key])
        //        if (e != null)
        //            ((Body)Component).EquipOnSpawn(key, e.ID);
        //}
    }

    public string CreateSerializableData(IComponent component)
    {
        return $"{nameof(Body)}";

        //Body body = (Body)component;

        //string headEquipment = GetSerializedEquipment(body.Head);
        //string torsoEquipment = GetSerializedEquipment(new List<IEntity>() { body.Torso });
        //string armEquipment = GetSerializedEquipment(body.Arm);
        //string legEquipment = GetSerializedEquipment(body.Leg);

        //return $"Body:Head={body.Head.Count}[{headEquipment}], Torso=1[{torsoEquipment}], Arm={body.Arm.Count}[{armEquipment}], Leg={body.Leg.Count}[{legEquipment}]";
    }

    string GetSerializedEquipment(IList<IEntity> bodyParts)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var bp in bodyParts)
        {
            if(bp == null) continue;

            GameEvent getEquipment = GameEventPool.Get(GameEventId.GetEquipment)
                .With(EventParameters.Equipment, null);
            bp.HandleEvent(getEquipment);
            string equipment = (string)getEquipment.Paramters[EventParameters.Equipment];
            getEquipment.Release();
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

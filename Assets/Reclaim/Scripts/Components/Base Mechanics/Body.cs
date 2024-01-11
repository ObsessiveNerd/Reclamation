using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public enum BodyPart
{
    None,
    Head,
    Torso,
    Arm,
    Leg,
    Finger,
    Back,
    Neck
}

public class Body : EntityComponent
{
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.PerformMeleeAttack, PerformMeleeAttack);
        RegisteredEvents.Add(GameEventId.DamageTaken, DamageTaken);
    }

    void PerformMeleeAttack(GameEvent gameEvent)
    {
        var target = gameEvent.GetValue<GameObject>(EventParameter.Target);

        var equipmentSlots = GetComponents<EquipmentSlot>();
        var meleeAttack = GameEventPool.Get(GameEventId.PerformMeleeAttack)
            .With(EventParameter.DamageList, new List<Damage>());

        foreach (var equipmentSlot in equipmentSlots)
            equipmentSlot.PerformMeleeAttack(meleeAttack);

        var takeDamage = GameEventPool.Get(GameEventId.DamageTaken)
            .With(meleeAttack.Paramters)
            .With(EventParameter.Source, gameObject);

        target.FireEvent(takeDamage);

        meleeAttack.Release();
        takeDamage.Release();
    }

    void DamageTaken(GameEvent gameEvent)
    {
        var damageList = gameEvent.GetValue<List<Damage>>(EventParameter.DamageList);
        var source = gameEvent.GetValue<GameObject>(EventParameter.Source);

        int total = 0;
        foreach(var damage in damageList)
        {
            Debug.LogError($"{source.name} did {damage.DamageAmount} of type {damage.Type}");
            total += damage.DamageAmount;
        }

        int armor = 0;
        var getArmor = GameEventPool.Get(GameEventId.GetArmor)
            .With(EventParameter.Armor, armor);

        total = Math.Max(1, total -  armor);
        getArmor.Release();

        if (total > 0)
        {
            var applyDamage = GameEventPool.Get(GameEventId.ApplyDamage)
                .With(EventParameter.DamageAmount, total);
            gameObject.FireEvent(applyDamage).Release();
        }
    }
}

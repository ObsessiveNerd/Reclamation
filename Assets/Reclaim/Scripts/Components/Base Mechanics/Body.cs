using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Body : EntityComponent
{
    public List<BodyPart> BodyParts = new List<BodyPart>();

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.HostileInteraction, HostileInteraction);
        RegisteredEvents.Add(GameEventId.DamageTaken, DamageTaken);

        foreach(var part in BodyParts)
            part.Activate();
    }

    void HostileInteraction(GameEvent gameEvent)
    {
        Attack(gameEvent);
    }

    void Attack(GameEvent gameEvent)
    {
        var target = gameEvent.GetValue<GameObject>(EventParameter.Target);

        var attack = GameEventPool.Get(GameEventId.PerformAttack)
            .With(EventParameter.DamageList, new List<Damage>())
            .With(EventParameter.Target, target)
            .With(EventParameter.Source, gameObject);

        foreach (var equipmentSlot in BodyParts)
            equipmentSlot.PassEventToEquipment(attack);

        //var takeDamage = GameEventPool.Get(GameEventId.DamageTaken)
        //    .With(attack.Paramters)
        //    .With(EventParameter.Source, gameObject);

        //target.FireEvent(takeDamage);

        attack.Release();
        
        //takeDamage.Release();
    }

    void DamageTaken(GameEvent gameEvent)
    {
        var damageList = gameEvent.GetValue<List<Damage>>(EventParameter.DamageList);
        var source = gameEvent.GetValue<GameObject>(EventParameter.Source);

        int total = 0;
        foreach(var damage in damageList)
        {
            Debug.LogError($"{source.name} did {damage.DamageAmount} of type {damage.Type}");
            //total += damage.DamageAmount;
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

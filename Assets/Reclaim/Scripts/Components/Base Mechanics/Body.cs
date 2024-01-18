using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[Serializable]
public class BodyData : EntityComponent
{
    public List<BodyPart> BodyParts = new List<BodyPart>();

    public override Type MonobehaviorType => typeof(Body);
    public override void WakeUp()
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
            .With(EventParameter.Source, Entity.GameObject);

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
            Debug.LogError($"{source.name} did {damage.component.DamageAmount} of type {damage.component.Type}");
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
            Entity.FireEvent(applyDamage).Release();
        }
    }
}


public class Body : ComponentBehavior<BodyData>
{

}

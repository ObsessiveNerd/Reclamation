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
    public BodyPart MainHand;
    public BodyPart Offhand;

    //List<BodyPart> BodyParts = new List<BodyPart>();

    Type MonobehaviorType = typeof(Body);
    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.PrimaryAttack, HostileInteraction);
        RegisteredEvents.Add(GameEventId.DamageTaken, DamageTaken);

        //temp
        MainHand.Activate();
        Offhand.Activate();

        //foreach(var part in BodyParts)
        //    part.Activate();
    }

    void HostileInteraction(GameEvent gameEvent)
    {
        Attack(gameEvent);
    }

    void Attack(GameEvent gameEvent)
    {
        MainHand.PassEventToEquipment(gameEvent);
        //Offhand.PassEventToEquipment(attack);
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

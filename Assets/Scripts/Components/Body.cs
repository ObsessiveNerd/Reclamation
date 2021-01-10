using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : Component
{
    IEntity m_Body;
    List<IEntity> m_Head = new List<IEntity>();
    List<IEntity> m_Arms = new List<IEntity>();
    List<IEntity> m_Legs = new List<IEntity>();

    public Body(IEntity self, IEntity body, List<IEntity> head = null, List<IEntity> arms = null, List<IEntity> legs = null)
    {
        Init(self);
        m_Body = body;
        if(head != null)
            m_Head = head;
        if(arms != null)
            m_Arms = arms;
        if(legs != null)
            m_Legs = legs;
        RegisteredEvents.Add(GameEventId.SeverBodyPart);
        RegisteredEvents.Add(GameEventId.AddArmorValue);
        RegisteredEvents.Add(GameEventId.PerformAttack);
        RegisteredEvents.Add(GameEventId.GetRangedWeapon);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.SeverBodyPart)
        {
            //todo
            RecLog.Log("Body part severed");
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
                IEntity equipment = (IEntity)FireEvent(hand, new GameEvent(GameEventId.GetEquipment, new KeyValuePair<string, object>(EventParameters.Entity, null))).Paramters[EventParameters.Entity];
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
                IEntity equipment = (IEntity)FireEvent(hand, new GameEvent(GameEventId.GetEquipment, new KeyValuePair<string, object>(EventParameters.Entity, null))).Paramters[EventParameters.Entity];
                if (equipment != null && CombatUtility.GetWeaponType(equipment).HasFlag(desiredWeaponToAttack))
                    CombatUtility.Attack(Self, (IEntity)gameEvent.Paramters[EventParameters.Target], equipment);
            }
        }
    }
}

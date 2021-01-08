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
        RegisteredEvents.Add(GameEventId.GetArmor);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.SeverBodyPart)
        {
            //todo
            Debug.Log("Body part severed");
        }

        if(gameEvent.ID == GameEventId.GetArmor)
        {
            FireEvent(m_Body, gameEvent);
            foreach(var head in m_Head)
                FireEvent(head, gameEvent);
            foreach (var arm in m_Arms)
                FireEvent(arm, gameEvent);
            foreach (var leg in m_Legs)
                FireEvent(leg, gameEvent);
        }
    }
}

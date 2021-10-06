using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party
{
    private List<string> m_Members = new List<string>();
    private string m_Leader;

    public int Size
    {
        get
        {
            return string.IsNullOrEmpty(m_Leader) ? 0 : 1 + m_Members.Count;
        }
    }
    public int SizeLimit = 4;
    public void AssignLeader(string entity)
    {
        EventBuilder setLeader = EventBuilderPool.Get(GameEventId.SetLeader)
                                    .With(EventParameters.Entity, m_Leader);

        EntityQuery.GetEntity(entity).FireEvent(setLeader.CreateEvent());
    }

    public void ReplaceLeader(string newLeader)
    {
        m_Members.Add(m_Leader);
        m_Leader = newLeader;
        foreach (var member in m_Members)
            AssignLeader(member);
        m_Members.Remove(newLeader);
    }

    public void AddMember(string entity)
    {
        if (string.IsNullOrEmpty(m_Leader))
            m_Leader = entity;
        else
            m_Members.Add(entity);

        AssignLeader(entity);
    }

    public void RemoveMember(string entity)
    {
        if (entity == m_Leader)
        {
            if (m_Members.Count > 0)
            {
                m_Leader = m_Members[0];
                foreach (string id in m_Members)
                    AssignLeader(id);
                m_Members.RemoveAt(0);
            }
            else
                m_Leader = null;
        }
        else
            m_Members.Remove(entity);

        EventBuilder setLeader = EventBuilderPool.Get(GameEventId.SetLeader)
                                    .With(EventParameters.Entity, null);

        EntityQuery.GetEntity(entity).FireEvent(setLeader.CreateEvent());
    }

    public bool HasMember(string entity)
    {
        return m_Members.Contains(entity) || m_Leader == entity;
    }
}

public class PartyController : WorldComponent
{
    Dictionary<FactionId, List<Party>> m_FactionParties = new Dictionary<FactionId, List<Party>>();

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.LookingForGroup);
        RegisteredEvents.Add(GameEventId.MakePartyLeader);
        RegisteredEvents.Add(GameEventId.RemoveFromParty);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.LookingForGroup)
        {
            string id = gameEvent.GetValue<string>(EventParameters.Entity);
            IEntity entity = EntityQuery.GetEntity(id);

            GameEvent getFaction = new GameEvent(GameEventId.GetFaction, new KeyValuePair<string, object>(EventParameters.Value, null));
            FactionId factionId = FireEvent(entity, getFaction).GetValue<Faction>(EventParameters.Value).ID;

            if (!m_FactionParties.ContainsKey(factionId))
                m_FactionParties.Add(factionId, new List<Party>(){ new Party() });

            bool entityAddedToParty = false;
            foreach(var party in m_FactionParties[factionId])
            {
                if(party.Size <= party.SizeLimit)
                {
                    party.AddMember(id);
                    entityAddedToParty = true;
                }
            }
            if(!entityAddedToParty)
            {
                Party p = new Party();
                p.AddMember(id);
                m_FactionParties[factionId].Add(p);
            }
        }

        else if(gameEvent.ID == GameEventId.MakePartyLeader)
        {
            string newLeader = gameEvent.GetValue<string>(EventParameters.Entity);
            IEntity entity = EntityQuery.GetEntity(newLeader);

            GameEvent getFaction = new GameEvent(GameEventId.GetFaction, new KeyValuePair<string, object>(EventParameters.Value, null));
            FactionId factionId = FireEvent(entity, getFaction).GetValue<Faction>(EventParameters.Value).ID;

            if (!m_FactionParties.ContainsKey(factionId)) return;

            foreach(var party in m_FactionParties[factionId])
            {
                if (party.HasMember(newLeader))
                    party.ReplaceLeader(newLeader);
            }
        }

        else if(gameEvent.ID == GameEventId.RemoveFromParty)
        {
            string idToRemove = gameEvent.GetValue<string>(EventParameters.Entity);
            IEntity entity = EntityQuery.GetEntity(idToRemove);

            GameEvent getFaction = new GameEvent(GameEventId.GetFaction, new KeyValuePair<string, object>(EventParameters.Value, null));
            FactionId factionId = FireEvent(entity, getFaction).GetValue<Faction>(EventParameters.Value).ID;

            if (!m_FactionParties.ContainsKey(factionId)) return;

            foreach(var party in m_FactionParties[factionId])
            {
                if (party.HasMember(idToRemove))
                    party.RemoveMember(idToRemove);
            }
        }
    }
}

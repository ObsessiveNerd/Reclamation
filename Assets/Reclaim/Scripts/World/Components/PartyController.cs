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
        GameEvent setLeader = GameEventPool.Get(GameEventId.SetLeader)
                                    .With(EventParameter.Entity, m_Leader);

        EntityQuery.GetEntity(entity).FireEvent(setLeader).Release();
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

        GameEvent setLeader = GameEventPool.Get(GameEventId.SetLeader)
                                    .With(EventParameter.Entity, null);

        EntityQuery.GetEntity(entity).FireEvent(setLeader).Release();
    }

    public bool HasMember(string entity)
    {
        return m_Members.Contains(entity) || m_Leader == entity;
    }
}

public class PartyController : GameService
{
    Dictionary<FactionId, List<Party>> m_FactionParties = new Dictionary<FactionId, List<Party>>();

    public void MakePartyLeader(GameObject entity)
    {
        GameEvent getFaction = GameEventPool.Get(GameEventId.GetFaction).With(EventParameter.Value, null);
        FireEvent(entity, getFaction);
        Faction f = getFaction.GetValue<Faction>(EventParameter.Value);
        FactionId factionId = f.ID;
        getFaction.Release();

        if (!m_FactionParties.ContainsKey(factionId))
            return;

        foreach (var party in m_FactionParties[factionId])
        {
            if (party.HasMember(entity.ID))
                party.ReplaceLeader(entity.ID);
        }
    }

    public void LookingForGroup(GameObject entity)
    {
        if (entity == null)
            return;

        GameEvent getFaction = GameEventPool.Get(GameEventId.GetFaction).With(EventParameter.Value, null);
        FactionId factionId = FireEvent(entity, getFaction).GetValue<Faction>(EventParameter.Value).ID;
        getFaction.Release();

        if (!m_FactionParties.ContainsKey(factionId))
            m_FactionParties.Add(factionId, new List<Party>() { new Party() });

        bool entityAddedToParty = false;
        foreach (var party in m_FactionParties[factionId])
        {
            if (party.Size <= party.SizeLimit)
            {
                party.AddMember(entity.ID);
                entityAddedToParty = true;
            }
        }
        if (!entityAddedToParty)
        {
            Party p = new Party();
            p.AddMember(entity.ID);
            m_FactionParties[factionId].Add(p);
        }
    }

    public void RemoveFromParty(GameObject entity)
    {
        GameEvent getFaction = GameEventPool.Get(GameEventId.GetFaction).With(EventParameter.Value, null);
        FactionId factionId = FireEvent(entity, getFaction).GetValue<Faction>(EventParameter.Value).ID;
        getFaction.Release();

        if (!m_FactionParties.ContainsKey(factionId))
            return;

        foreach (var party in m_FactionParties[factionId])
        {
            if (party.HasMember(entity.ID))
                party.RemoveMember(entity.ID);
        }
    }
}

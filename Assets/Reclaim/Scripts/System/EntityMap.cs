using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMap : GameService
{
    public Dictionary<string, string> IDToNameMap = new Dictionary<string, string>();

    public void AddEntity(IEntity e)
    {
        m_EntityIdToEntityMap[e.ID] = e;
        AddEntityToNameMap(e);
    }

    public void UpdateEntity(IEntity e)
    {
        if (m_EntityIdToEntityMap.ContainsKey(e.ID))
            m_EntityIdToEntityMap[e.ID] = e;
    }

    public void AddEntityToNameMap(IEntity e)
    {
        IDToNameMap[e.ID] = e.Name;
    }

    public bool ContainsId(string id)
    {
        return m_EntityIdToEntityMap.ContainsKey(id);
    }

    public void DestroyEntity(string id)
    {
        if (m_EntityIdToEntityMap.ContainsKey(id))
            m_EntityIdToEntityMap[id] = null;
    }

    public IEntity GetEntity(string id)
    {
        if (!m_EntityIdToEntityMap.ContainsKey(id))
            return EntityFactory.CreateEntity(id);

        if (m_EntityIdToEntityMap.ContainsKey(id))
            return m_EntityIdToEntityMap[id];
        return null;
    }

    public void RegisterEntity(IEntity entity)
    {
        m_EntityIdToEntityMap[entity.ID] = entity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMap : GameService
{
    public Dictionary<string, string> IDToNameMap = new Dictionary<string, string>();

    public void AddEntity(GameObject e)
    {
        m_EntityIdToEntityMap[e.ID] = e;
        AddEntityToNameMap(e);
    }

    public void UpdateEntity(GameObject e)
    {
        if (m_EntityIdToEntityMap.ContainsKey(e.ID))
            m_EntityIdToEntityMap[e.ID] = e;
    }

    public void AddEntityToNameMap(GameObject e)
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

    public GameObject GetEntity(string id)
    {
        if (!m_EntityIdToEntityMap.ContainsKey(id))
            return EntityFactory.CreateEntity(id);

        if (m_EntityIdToEntityMap.ContainsKey(id))
            return m_EntityIdToEntityMap[id];
        return null;
    }

    public void RegisterEntity(GameObject entity)
    {
        m_EntityIdToEntityMap[entity.ID] = entity;
    }
}

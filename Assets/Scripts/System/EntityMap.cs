using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMap : WorldComponent
{
    public static Dictionary<string, string> IDToNameMap = new Dictionary<string, string>();
    public static void AddEntityToNameMap(IEntity e)
    {
        IDToNameMap[e.ID] = e.Name;
    }


    private Dictionary<string, IEntity> m_EntityIdToEntityMap = new Dictionary<string, IEntity>();

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.RegisterEntity);
        RegisteredEvents.Add(GameEventId.DestroyEntity);
        RegisteredEvents.Add(GameEventId.GetEntity);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.RegisterEntity)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            RegisterEntity(entity);
        }

        else if (gameEvent.ID == GameEventId.DestroyEntity)
        {
            string id = (string)gameEvent.Paramters[EventParameters.Value];
            if (m_EntityIdToEntityMap.ContainsKey(id))
                m_EntityIdToEntityMap[id] = null;
        }

        else if (gameEvent.ID == GameEventId.GetEntity)
        {
            string id = (string)gameEvent.Paramters[EventParameters.Value];
            gameEvent.Paramters[EventParameters.Entity] = GetEntity(id);
        }
    }

    public IEntity GetEntity(string id)
    {
        if (m_EntityIdToEntityMap.ContainsKey(id))
            return m_EntityIdToEntityMap[id];
        return null;
    }

    public void RegisterEntity(IEntity entity)
    {
        m_EntityIdToEntityMap[entity.ID] = entity;
    }
}

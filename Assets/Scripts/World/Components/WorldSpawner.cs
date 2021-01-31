using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpawner : WorldComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.Spawn);
        RegisteredEvents.Add(GameEventId.Despawn);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Spawn)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            Point spawnPoint = (Point)gameEvent.Paramters[EventParameters.Point];
            FireEvent(entity, new GameEvent(GameEventId.SetPoint, new KeyValuePair<string, object>(EventParameters.TilePosition, spawnPoint)));
            FireEvent(m_Tiles[spawnPoint], gameEvent);
            FireEvent(Self, new GameEvent(GameEventId.UpdateWorldView));
            m_EntityToPointMap[entity] = spawnPoint;

            //Todo: we'll need to make sure we find which player is closest before picking their time system
            FireEvent(entity, new GameEvent(GameEventId.RegisterPlayableCharacter));
            FireEvent(entity, new GameEvent(GameEventId.RegisterWithTimeSystem, new KeyValuePair<string, object>(EventParameters.Value, m_TimeProgression /*m_PlayerToTimeProgressionMap[m_ActivePlayer.Value]*/)));
        }

        if(gameEvent.ID == GameEventId.Despawn)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];
            if (!m_EntityToPointMap.ContainsKey(entity)) return;

            Point currentPoint = m_EntityToPointMap[entity];
            GameEvent despawn = new GameEvent(GameEventId.Despawn, new KeyValuePair<string, object>(EventParameters.Entity, entity),
                                                                   new KeyValuePair<string, object>(EventParameters.EntityType, entityType));
            FireEvent(m_Tiles[currentPoint], despawn);
            m_EntityToPointMap.Remove(entity);
            m_TimeProgression.RemoveEntity(entity);
            //foreach(var timeProgression in m_PlayerToTimeProgressionMap.Values)
            //{
            //    if(timeProgression.ContainsEntity(entity))
            //    {
            //        timeProgression.RemoveEntity(entity);
            //        break;
            //    }
            //}
        }
    }
}

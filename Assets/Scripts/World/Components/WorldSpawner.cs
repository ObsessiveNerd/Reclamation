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
            
            IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            Point spawnPoint = (Point)gameEvent.Paramters[EventParameters.Point];
            if (!m_Tiles.ContainsKey(spawnPoint)) return;

            FireEvent(entity, GameEventPool.Get(GameEventId.SetPoint).With(EventParameters.TilePosition, spawnPoint)).Release();
            m_Tiles[spawnPoint].GetComponent<Tile>().Spawn(gameEvent);

            //FireEvent(m_Tiles[spawnPoint], gameEvent);
            FireEvent(Self, GameEventPool.Get(GameEventId.UpdateWorldView)).Release();
            m_EntityToPointMap[entity] = spawnPoint;

            //Todo: we'll need to make sure we find which player is closest before picking their time system
            FireEvent(entity, GameEventPool.Get(GameEventId.RegisterPlayableCharacter)).Release();
            FireEvent(entity, GameEventPool.Get(GameEventId.RegisterWithTimeSystem)
                .With(EventParameters.Value, m_TimeProgression /*m_PlayerToTimeProgressionMap[m_ActivePlayer.Value]*/)).Release();
        }

        if(gameEvent.ID == GameEventId.Despawn)
        {
            IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];
            if (!m_EntityToPointMap.ContainsKey(entity)) return;

            Point currentPoint = m_EntityToPointMap[entity];
            GameEvent despawn = GameEventPool.Get(GameEventId.Despawn).With(EventParameters.Entity, entity.ID)
                                                                   .With(EventParameters.EntityType, entityType);

            m_Tiles[currentPoint].GetComponent<Tile>().Despawn(despawn);
            //FireEvent(m_Tiles[currentPoint], despawn);
            m_EntityToPointMap.Remove(entity);
            m_TimeProgression.RemoveEntity(entity);
            despawn.Release();
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

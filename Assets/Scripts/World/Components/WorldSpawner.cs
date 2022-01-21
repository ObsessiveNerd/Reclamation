using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpawner : GameService
{
    public void Spawn(IEntity entity, Point spawnPoint)
    {
        if (!m_Tiles.ContainsKey(spawnPoint))
        {
            Debug.LogWarning($"Spawn point {spawnPoint} does not exist for {entity.Name}");
            return;
        }

        FireEvent(entity, GameEventPool.Get(GameEventId.SetPoint).With(EventParameters.TilePosition, spawnPoint)).Release();
        m_Tiles[spawnPoint].Spawn(entity);

        Services.WorldUpdateService.UpdateWorldView();
        m_EntityToPointMap[entity] = spawnPoint;

        //Todo: we'll need to make sure we find which player is closest before picking their time system
        FireEvent(entity, GameEventPool.Get(GameEventId.RegisterPlayableCharacter)).Release();
        FireEvent(entity, GameEventPool.Get(GameEventId.RegisterWithTimeSystem)
            .With(EventParameters.Value, m_TimeProgression /*m_PlayerToTimeProgressionMap[m_ActivePlayer.Value]*/)).Release();
    }

    public void Despawn(IEntity entity)
    {
        GameEvent getType = GameEventPool.Get(GameEventId.GetEntityType)
                            .With(EventParameters.EntityType, EntityType.None);

        EntityType entityType = entity.FireEvent(getType).GetValue<EntityType>(EventParameters.EntityType);
        getType.Release();

        if (!m_EntityToPointMap.ContainsKey(entity))
            return;

        Point currentPoint = m_EntityToPointMap[entity];
        GameEvent despawn = GameEventPool.Get(GameEventId.Despawn).With(EventParameters.Entity, entity.ID)
                                                               .With(EventParameters.EntityType, entityType);

        m_Tiles[currentPoint].Despawn(despawn);
        m_EntityToPointMap.Remove(entity);
        m_TimeProgression.RemoveEntity(entity);
        despawn.Release();
    }
}

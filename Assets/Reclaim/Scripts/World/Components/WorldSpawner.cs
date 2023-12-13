using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpawner : GameService
{
    HashSet<GameObject> m_DespawnAtEndOfFrame = new HashSet<GameObject>();
    public void Spawn(GameObject entity, Point spawnPoint)
    {
        if (!m_Tiles.ContainsKey(spawnPoint))
        {
            Debug.LogWarning($"Spawn point {spawnPoint} does not exist for {entity.Name}");
            return;
        }

        FireEvent(entity, GameEventPool.Get(GameEventId.SetPoint).With(EventParameter.TilePosition, spawnPoint)).Release();
        m_Tiles[spawnPoint].Spawn(entity);

        m_EntityToPointMap[entity.ID] = spawnPoint;
        Services.WorldUpdateService.UpdateWorldView();

        //Todo: we'll need to make sure we find which player is closest before picking their time system
        FireEvent(entity, GameEventPool.Get(GameEventId.RegisterPlayableCharacter)).Release();
        FireEvent(entity, GameEventPool.Get(GameEventId.RegisterWithTimeSystem)
            .With(EventParameter.Value, m_TimeProgression /*m_PlayerToTimeProgressionMap[m_ActivePlayer.Value]*/)).Release();
    }

    public void RegisterForDespawn(GameObject entity)
    {
        m_DespawnAtEndOfFrame.Add(entity);
    }

    public void DespawnAllRegistered()
    {
        foreach(var e in m_DespawnAtEndOfFrame)
            Despawn(e);

        m_DespawnAtEndOfFrame.Clear();
    }

    public void Despawn(GameObject entity)
    {
        GameEvent getType = GameEventPool.Get(GameEventId.GetEntityType)
                            .With(EventParameter.EntityType, EntityType.None);

        EntityType entityType = entity.FireEvent(getType).GetValue<EntityType>(EventParameter.EntityType);
        getType.Release();

        if (!m_EntityToPointMap.ContainsKey(entity.ID))
            return;

        Point currentPoint = m_EntityToPointMap[entity.ID];
        GameEvent despawn = GameEventPool.Get(GameEventId.Despawn).With(EventParameter.Entity, entity.ID)
                                                               .With(EventParameter.EntityType, entityType);

        m_Tiles[currentPoint].Despawn(despawn);

        if (Services.NetworkService.IsConnected)
            Services.NetworkService.EmitEvent(new GameEventSerializable(currentPoint.ToString(), despawn));

        m_EntityToPointMap.Remove(entity.ID);
        m_EntityToPreviousPointMap[entity.ID] = currentPoint;
        m_TimeProgression.RemoveEntity(entity);
        despawn.Release();
    }
}

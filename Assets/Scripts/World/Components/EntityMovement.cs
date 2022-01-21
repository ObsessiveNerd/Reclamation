using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : GameService
{
    public MoveDirection BeforeMoving(IEntity entity, MoveDirection moveDirection, ref int energyRequired)
    {
        if (!m_EntityToPointMap.ContainsKey(entity))
            return MoveDirection.None;

        GameEvent entityBeforeMoveCheck = GameEventPool.Get(GameEventId.BeforeMoving)
                                            .With(EventParameters.Entity, entity.ID)
                                            .With(EventParameters.InputDirection, moveDirection)
                                            .With(EventParameters.RequiredEnergy, energyRequired);

        Point currentPoint = m_EntityToPointMap[entity];
        Point newPoint = GetTilePointInDirection(currentPoint, moveDirection);
        if (m_Tiles.TryGetValue(newPoint, out Actor tile))
            tile.GetComponent<Tile>().BeforeMoving(entityBeforeMoveCheck);

        var ret = entityBeforeMoveCheck.GetValue<MoveDirection>(EventParameters.InputDirection);
        entityBeforeMoveCheck.Release();
        return ret;
    }

    public void SetEntityPosition(IEntity entity, Point newPoint)
    {
        if (!m_EntityToPointMap.ContainsKey(entity))
            return;
        Point currentPoint = m_EntityToPointMap[entity];
        GameEvent getEntityType = GameEventPool.Get(GameEventId.GetEntityType)
                                    .With(EventParameters.EntityType, EntityType.None);

        EntityType entityType = entity.FireEvent(getEntityType).GetValue<EntityType>(EventParameters.EntityType);
        getEntityType.Release();

        GameEvent removeEntityFromTile = GameEventPool.Get(GameEventId.Despawn)
                                            .With(EventParameters.Entity, entity.ID)
                                            .With(EventParameters.EntityType, entityType);
        FireEvent(m_Tiles[currentPoint], removeEntityFromTile).Release();

        m_EntityToPointMap[entity] = newPoint;
        GameEvent addEntityToTile = GameEventPool.Get(GameEventId.Spawn)
                                        .With(EventParameters.Entity, entity.ID)
                                        .With(EventParameters.EntityType, entityType);
        FireEvent(m_Tiles[newPoint], addEntityToTile).Release();
        Services.WorldUpdateService.UpdateWorldView();
    }

    public void Move(IEntity entity, MoveDirection moveDirection)
    {
        if (!m_EntityToPointMap.ContainsKey(entity))
            return;
        Point currentPoint = m_EntityToPointMap[entity];
        Point newPoint = GetTilePointInDirection(currentPoint, moveDirection);

        if (m_Tiles.ContainsKey(newPoint))
        {
            var pointEvent = FireEvent(entity, GameEventPool.Get(GameEventId.SetPoint).With(EventParameters.TilePosition, newPoint));
            Spawner.Move(entity, newPoint);
            pointEvent.Release();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : GameService
{
    //public MoveDirection BeforeMoving(GameObject entity, MoveDirection moveDirection, out float energyRequired)
    //{
    //    if (!m_EntityToPointMap.ContainsKey(entity))
    //    {
    //        energyRequired = 0f;
    //        return MoveDirection.None;
    //    }

    //    float energy = 1f;
    //    GameEvent entityBeforeMoveCheck = GameEventPool.Get(GameEventId.BeforeMoving)
    //                                        .With(EventParameters.Entity, entity.ID)
    //                                        .With(EventParameters.InputDirection, moveDirection)
    //                                        .With(EventParameters.RequiredEnergy, energy);

    //    Point currentPoint = m_EntityToPointMap[entity];
    //    Point newPoint = GetTilePointInDirection(currentPoint, moveDirection);
    //    if (m_Tiles.TryGetValue(newPoint, out Actor tile))
    //        tile.BeforeMoving(entityBeforeMoveCheck);

    //    var ret = entityBeforeMoveCheck.GetValue<MoveDirection>(EventParameters.InputDirection);
    //    entityBeforeMoveCheck.Release();
    //    energyRequired = energy;
    //    return ret;
    //}

    //public void SetEntityPosition(GameObject entity, Point newPoint)
    //{
    //    if (!m_EntityToPointMap.ContainsKey(entity.ID))
    //        return;
    //    Point currentPoint = m_EntityToPointMap[entity.ID];
    //    GameEvent getEntityType = GameEventPool.Get(GameEventId.GetEntityType)
    //                                .With(EventParameter.EntityType, EntityType.None);

    //    EntityType entityType = entity.FireEvent(getEntityType).GetValue<EntityType>(EventParameter.EntityType);
    //    getEntityType.Release();

    //    GameEvent removeEntityFromTile = GameEventPool.Get(GameEventId.Despawn)
    //                                        .With(EventParameter.Entity, entity.ID)
    //                                        .With(EventParameter.EntityType, entityType);
    //    m_Tiles[currentPoint].Despawn(removeEntityFromTile);
    //    removeEntityFromTile.Release();

    //    m_EntityToPointMap[entity.ID] = newPoint;
    //    GameEvent addEntityToTile = GameEventPool.Get(GameEventId.Spawn)
    //                                    .With(EventParameter.Entity, entity.ID)
    //                                    .With(EventParameter.EntityType, entityType);
    //    m_Tiles[newPoint].Spawn(entity);
    //    addEntityToTile.Release();
    //    Services.WorldUpdateService.UpdateWorldView();
    //}

    //public void Move(GameObject entity, MoveDirection moveDirection)
    //{
    //    if (!m_EntityToPointMap.ContainsKey(entity.ID))
    //        return;
    //    Point currentPoint = m_EntityToPointMap[entity.ID];
    //    Point newPoint = GetTilePointInDirection(currentPoint, moveDirection);

    //    if (m_Tiles.ContainsKey(newPoint))
    //    {
    //        m_ChangedTiles.Add(m_Tiles[currentPoint]);
    //        m_ChangedTiles.Add(m_Tiles[newPoint]);

    //        var pointEvent = FireEvent(entity, GameEventPool.Get(GameEventId.SetPoint).With(EventParameter.TilePosition, newPoint));
    //        SetEntityPosition(entity, newPoint);
    //        pointEvent.Release();
    //    }
    //}
}

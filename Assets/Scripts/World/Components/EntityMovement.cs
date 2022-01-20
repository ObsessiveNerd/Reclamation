using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : WorldComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.BeforeMoving);
        RegisteredEvents.Add(GameEventId.MoveEntity);
        RegisteredEvents.Add(GameEventId.SetEntityPosition);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.BeforeMoving)
        {
            IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            MoveDirection moveDirection = (MoveDirection)gameEvent.Paramters[EventParameters.InputDirection];
            if (!m_EntityToPointMap.ContainsKey(entity)) return;

            Point currentPoint = m_EntityToPointMap[entity];
            Point newPoint = GetTilePointInDirection(currentPoint, moveDirection);
            if (m_Tiles.TryGetValue(newPoint, out Actor tile))
            {
                tile.GetComponent<Tile>().BeforeMoving(gameEvent);
                //FireEvent(tile, gameEvent);
            }
            else
            {
                //Move to a new part of the map.  For now do nothing
            }
        }

        if(gameEvent.ID == GameEventId.SetEntityPosition)
        {
            IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            if (!m_EntityToPointMap.ContainsKey(entity)) return;
            Point currentPoint = m_EntityToPointMap[entity];
            Point newPoint = gameEvent.GetValue<Point>(EventParameters.TilePosition);
            EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];

            GameEvent removeEntityFromTile = GameEventPool.Get(GameEventId.Despawn)
                                                .With(EventParameters.Entity, entity.ID)
                                                .With(EventParameters.EntityType, entityType);
            FireEvent(m_Tiles[currentPoint], removeEntityFromTile).Release();
            m_EntityToPointMap[entity] = newPoint;
            GameEvent addEntityToTile = GameEventPool.Get(GameEventId.Spawn)
                                            .With(EventParameters.Entity, entity.ID)
                                            .With(EventParameters.EntityType, entityType);
            FireEvent(m_Tiles[newPoint], addEntityToTile).Release();
            FireEvent(Self, GameEventPool.Get(GameEventId.UpdateWorldView)).Release();
        }

        if (gameEvent.ID == GameEventId.MoveEntity)
        {
            IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            if (!m_EntityToPointMap.ContainsKey(entity)) return;
            Point currentPoint = m_EntityToPointMap[entity];
            EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];
            MoveDirection moveDirection = (MoveDirection)gameEvent.Paramters[EventParameters.InputDirection];
            Point newPoint = GetTilePointInDirection(currentPoint, moveDirection);

            if (m_Tiles.ContainsKey(newPoint))
            {
                FireEvent(entity, GameEventPool.Get(GameEventId.SetPoint).With(EventParameters.TilePosition, newPoint)).Release();
                Spawner.Move(entity, newPoint);


                //Just as a note, doing it this way isn't terribly efficient since despawn is going to remove things from the time progression
                //This also means turn order is going to get fucked, so really we should do this proper and just event to the correct tiles here.
                //I mean I guess things are despawning and spawning in order so maybe turn order won't get fucked.
                //GameEvent spawn = GameEventPool.Get(GameEventId.Spawn, new .With(EventParameters.Entity, entity.ID),
                //                                                       new .With(EventParameters.EntityType, entityType));

                //FireEvent(m_Tiles[newPoint], spawn);

                //GameEvent despawn = GameEventPool.Get(GameEventId.Despawn, new .With(EventParameters.Entity, entity.ID),
                //                                                   new .With(EventParameters.EntityType, entityType));
                //FireEvent(m_Tiles[currentPoint], despawn);
                //FireEvent(Self, GameEventPool.Get(GameEventId.UpdateWorldView));
                //m_EntityToPointMap[entity] = newPoint;
            }
            else
            {
                //Todo move to a new section of the map
            }
        }
    }
}

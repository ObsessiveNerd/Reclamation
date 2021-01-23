﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInteractions : WorldComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.ShowTileInfo);
        RegisteredEvents.Add(GameEventId.AddComponentToTile);
        //RegisteredEvents.Add(GameEventId.Interact);
        RegisteredEvents.Add(GameEventId.Pickup);
        RegisteredEvents.Add(GameEventId.Drop);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        //if (gameEvent.ID == GameEventId.Interact)
        //{
        //    IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
        //    Point currentPoint = m_EntityToPointMap[entity];
        //    FireEvent(m_Tiles[currentPoint], new GameEvent(GameEventId.Interact, new KeyValuePair<string, object>(EventParameters.Entity, entity)));
        //}

        if (gameEvent.ID == GameEventId.Pickup)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            Point p = m_EntityToPointMap[entity];
            FireEvent(m_Tiles[p], gameEvent);
        }

        if (gameEvent.ID == GameEventId.Drop)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            IEntity droppingEntity = (IEntity)gameEvent.Paramters[EventParameters.Creature];
            EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];

            Point p = m_EntityToPointMap[droppingEntity];
            FireEvent(Self, new GameEvent(GameEventId.Spawn, new KeyValuePair<string, object>(EventParameters.Entity, entity),
                                                                    new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Item),
                                                                    new KeyValuePair<string, object>(EventParameters.Point, p)));
        }

        if (gameEvent.ID == GameEventId.ShowTileInfo)
        {
            Point currentTilePos = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            FireEvent(m_Tiles[currentTilePos], gameEvent);
        }

        if (gameEvent.ID == GameEventId.AddComponentToTile)
        {
            //Todo
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelection : WorldComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.SelectTile);
        RegisteredEvents.Add(GameEventId.SelectNewTileInDirection);
        RegisteredEvents.Add(GameEventId.EndSelection);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.SelectTile)
        {
            EventBuilder getEntity = new EventBuilder(GameEventId.GetEntity)
                                        .With(EventParameters.Entity, null)
                                        .With(EventParameters.Value, gameEvent.Paramters[EventParameters.Entity]);
            
            IEntity entity = FireEvent(Self, getEntity.CreateEvent()).GetValue<IEntity>(EventParameters.Entity);

            EventBuilder getTarget = new EventBuilder(GameEventId.GetEntity)
                                        .With(EventParameters.Entity, null)
                                        .With(EventParameters.Value, gameEvent.Paramters[EventParameters.Target]);
            IEntity target = FireEvent(Self, getTarget.CreateEvent()).GetValue<IEntity>(EventParameters.Entity);

            Point p = m_EntityToPointMap[target == null ? entity : target];

            m_Tiles[p].AddComponent(new SelectedTile());
            m_Tiles[p].CleanupComponents();

            gameEvent.Paramters[EventParameters.TilePosition] = p;
        }

        if (gameEvent.ID == GameEventId.SelectNewTileInDirection)
        {
            Point currentTilePos = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            MoveDirection moveDirection = (MoveDirection)gameEvent.Paramters[EventParameters.InputDirection];

            m_Tiles[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_Tiles[currentTilePos].CleanupComponents();

            Point newPoint = GetTilePointInDirection(currentTilePos, moveDirection);

            m_Tiles[newPoint].AddComponent(new SelectedTile());
            m_Tiles[newPoint].CleanupComponents();

            gameEvent.Paramters[EventParameters.TilePosition] = newPoint;
        }

        if (gameEvent.ID == GameEventId.EndSelection)
        {
            Point currentTilePos = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            if(!m_Tiles.ContainsKey(currentTilePos)) return;

            m_Tiles[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_Tiles[currentTilePos].CleanupComponents();
            gameEvent.Paramters[EventParameters.TilePosition] = null;
        }
    }
}

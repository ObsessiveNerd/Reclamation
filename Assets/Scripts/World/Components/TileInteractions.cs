using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInteractions : GameService
{
    public Tile GetTile(Point p)
    {
        if (!m_Tiles.ContainsKey(p))
            return null;
        return m_Tiles[p].GetComponent<Tile>();
    }

    public void TileChanged(Tile t)
    {
        m_ChangedTiles.Add(t);
    }


    public override void HandleEvent(GameEvent gameEvent)
    {
        //if (gameEvent.ID == GameEventId.Interact)
        //{
        //    IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
        //    Point currentPoint = m_EntityToPointMap[entity];
        //    FireEvent(m_Tiles[currentPoint], GameEventPool.Get(GameEventId.Interact, new .With(EventParameters.Entity, entity)));
        //}

        if (gameEvent.ID == GameEventId.Pickup)
        {
            IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            Point p = m_EntityToPointMap[entity];
            FireEvent(m_Tiles[p], gameEvent);
        }

        if (gameEvent.ID == GameEventId.Drop)
        {
            IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            IEntity droppingEntity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Creature]);
            EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];

            if(!m_EntityToPointMap.ContainsKey(droppingEntity)) return;

            Point p = m_EntityToPointMap[droppingEntity];
            FireEvent(Self, GameEventPool.Get(GameEventId.Spawn)
                    .With(EventParameters.Entity, entity.ID)
                    .With(EventParameters.EntityType, EntityType.Item)
                    .With(EventParameters.Point, p)).Release();
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

        if(gameEvent.ID == GameEventId.DestroyObject)
        {
            Point p = gameEvent.GetValue<Point>(EventParameters.Point);
            if (!m_Tiles.ContainsKey(p))
                Debug.Log($"P isn't here. {p}");
            else
                FireEvent(m_Tiles[p], gameEvent);
        }
    }
}

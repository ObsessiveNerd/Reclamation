using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldFov : WorldComponent
{
    Dictionary<IEntity, List<Point>> m_PlayerToVisibleTiles = new Dictionary<IEntity, List<Point>>();

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.FOVRecalculated);
        RegisteredEvents.Add(GameEventId.IsTileBlocking);
        RegisteredEvents.Add(GameEventId.RevealAllTiles);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.RevealAllTiles)
        {
            foreach (var tile in m_Tiles.Values)
                FireEvent(tile, new GameEvent(GameEventId.SetVisibility, new KeyValuePair<string, object>(EventParameters.TileInSight, true)));
            FireEvent(Self, new GameEvent(GameEventId.UpdateWorldView));
        }

        if(gameEvent.ID == GameEventId.FOVRecalculated)
        {
            IEntity source = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            List<Point> newVisibleTiles = (List<Point>)gameEvent.Paramters[EventParameters.VisibleTiles];

            if (!m_PlayerToVisibleTiles.ContainsKey(source))
                m_PlayerToVisibleTiles.Add(source, newVisibleTiles);

            List<Point> oldVisibleTiles = m_PlayerToVisibleTiles[source];
            m_PlayerToVisibleTiles[source] = newVisibleTiles;
            UpdateTiles(oldVisibleTiles);
        }

        if(gameEvent.ID == GameEventId.IsTileBlocking)
        {
            Point p = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            if(m_Tiles.ContainsKey(p))
                FireEvent(m_Tiles[p], gameEvent);
        }
    }

    void ClearTiles(List<Point> oldTiles, List<Point> newTiles)
    {
        foreach (Point tile in oldTiles)
        {
            if(!newTiles.Contains(tile))
                FireEvent(m_Tiles[tile], new GameEvent(GameEventId.SetVisibility, new KeyValuePair<string, object>(EventParameters.TileInSight, false)));
        }
    }

    void UpdateTiles(List<Point> oldTiles)
    {
        List<Point> allVisibleTiles = new List<Point>();
        foreach (var key in m_PlayerToVisibleTiles.Keys)
            allVisibleTiles.AddRange(m_PlayerToVisibleTiles[key]);

        foreach (Point tile in oldTiles)
        {
            if(allVisibleTiles.Contains(tile))
                FireEvent(m_Tiles[tile], new GameEvent(GameEventId.SetVisibility, new KeyValuePair<string, object>(EventParameters.TileInSight, true)));
            else
                FireEvent(m_Tiles[tile], new GameEvent(GameEventId.SetVisibility, new KeyValuePair<string, object>(EventParameters.TileInSight, false)));
        }
    }
}

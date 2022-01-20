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
        RegisteredEvents.Add(GameEventId.CleanFoVData);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.RevealAllTiles)
        {
            foreach (var tile in m_Tiles.Values)
                FireEvent(tile, GameEventPool.Get(GameEventId.SetVisibility)
                    .With(EventParameters.TileInSight, true)).Release();
            FireEvent(Self, GameEventPool.Get(GameEventId.UpdateWorldView)).Release();
        }

        if (gameEvent.ID == GameEventId.UnRegisterPlayer)
        {
            IEntity player = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            if (m_PlayerToVisibleTiles.ContainsKey(player))
                m_PlayerToVisibleTiles.Remove(player);
        }

        if (gameEvent.ID == GameEventId.CleanFoVData)
        {
            m_PlayerToVisibleTiles.Clear();
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
            {
                m_Tiles[p].GetComponent<Tile>().IsTileBlocking(gameEvent);
                //FireEvent(m_Tiles[p], gameEvent);
            }
        }
    }

    void UpdateTiles(List<Point> oldTiles)
    {
        List<Point> allVisibleTiles = new List<Point>();
        foreach (var key in m_PlayerToVisibleTiles.Keys)
            allVisibleTiles.AddRange(m_PlayerToVisibleTiles[key]);

        foreach(Point tile in allVisibleTiles)
                FireEvent(m_Tiles[tile], GameEventPool.Get(GameEventId.SetVisibility)
                    .With(EventParameters.TileInSight, true)).Release();

        foreach (Point tile in oldTiles)
            if(!allVisibleTiles.Contains(tile))
                FireEvent(m_Tiles[tile], GameEventPool.Get(GameEventId.SetVisibility)
                    .With(EventParameters.TileInSight, false)).Release();
    }
}

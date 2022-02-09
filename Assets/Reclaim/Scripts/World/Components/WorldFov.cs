using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldFov : GameService
{
    Dictionary<IEntity, List<Point>> m_PlayerToVisibleTiles = new Dictionary<IEntity, List<Point>>();

    public void UnRegisterPlayer(IEntity player)
    {
        if (m_PlayerToVisibleTiles.ContainsKey(player))
            m_PlayerToVisibleTiles.Remove(player);
    }

    public void CleanFoVData()
    {
        m_PlayerToVisibleTiles.Clear();
    }

    public void FoVRecalculated(IEntity source, List<Point> newVisibleTiles)
    {
        if (!m_PlayerToVisibleTiles.ContainsKey(source))
            m_PlayerToVisibleTiles.Add(source, newVisibleTiles);

        List<Point> oldVisibleTiles = m_PlayerToVisibleTiles[source];
        m_PlayerToVisibleTiles[source] = newVisibleTiles;
        UpdateTiles(oldVisibleTiles);
    }

    public void RevealAllTiles()
    {
        foreach (var tile in m_TileEntity.Values)
            FireEvent(tile, GameEventPool.Get(GameEventId.SetVisibility)
                .With(EventParameters.TileInSight, true)).Release();
        Services.WorldUpdateService.UpdateWorldView();
    }

    void UpdateTiles(List<Point> oldTiles)
    {
        List<Point> allVisibleTiles = new List<Point>();
        foreach (var key in m_PlayerToVisibleTiles.Keys)
            allVisibleTiles.AddRange(m_PlayerToVisibleTiles[key]);

        foreach(Point tile in allVisibleTiles)
                FireEvent(m_TileEntity[tile], GameEventPool.Get(GameEventId.SetVisibility)
                    .With(EventParameters.TileInSight, true)).Release();

        foreach (Point tile in oldTiles)
            if(!allVisibleTiles.Contains(tile))
                FireEvent(m_TileEntity[tile], GameEventPool.Get(GameEventId.SetVisibility)
                    .With(EventParameters.TileInSight, false)).Release();
    }
}

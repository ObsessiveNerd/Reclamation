using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldFov : GameService
{
    Dictionary<string, List<Point>> m_PlayerToVisibleTiles = new Dictionary<string, List<Point>>();

    public void UnRegisterPlayer(GameObject player)
    {
        //if (m_PlayerToVisibleTiles.ContainsKey(player.ID))
        //    m_PlayerToVisibleTiles.Remove(player.ID);
    }

    public void CleanFoVData()
    {
        m_PlayerToVisibleTiles.Clear();
    }

    public void FoVRecalculated(GameObject source, List<Point> newVisibleTiles)
    {
        //if (!m_PlayerToVisibleTiles.ContainsKey(source.ID))
        //    m_PlayerToVisibleTiles.Add(source.ID, newVisibleTiles);

        //List<Point> oldVisibleTiles = m_PlayerToVisibleTiles[source.ID];
        //m_PlayerToVisibleTiles[source.ID] = newVisibleTiles;
        //UpdateTiles(oldVisibleTiles);
    }

    public void RevealAllTiles()
    {
        //foreach (var tile in m_TileEntity.Values)
        //    FireEvent(tile, GameEventPool.Get(GameEventId.SetVisibility)
        //        .With(EventParameter.TileInSight, true)).Release();
        //Services.WorldUpdateService.UpdateWorldView();
    }

    //void UpdateTiles(List<Point> oldTiles)
    //{
    //    List<Point> allVisibleTiles = new List<Point>();
    //    foreach (var key in m_PlayerToVisibleTiles.Keys)
    //        allVisibleTiles.AddRange(m_PlayerToVisibleTiles[key]);

    //    foreach(Point tile in allVisibleTiles)
    //    {
    //        if(!m_TileEntity.ContainsKey(tile)) continue;

    //            FireEvent(m_TileEntity[tile], GameEventPool.Get(GameEventId.SetVisibility)
    //                .With(EventParameter.TileInSight, true)).Release();
    //    }

    //    foreach (Point tile in oldTiles)
    //    { 
    //        if(!allVisibleTiles.Contains(tile) && m_TileEntity.ContainsKey(tile))
    //            FireEvent(m_TileEntity[tile], GameEventPool.Get(GameEventId.SetVisibility)
    //                .With(EventParameter.TileInSight, false)).Release();
    //    }
    //}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerFOVHandler : EntityComponent
{
    private List<Point> m_VisiblePoints = new List<Point>();

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.FOVRecalculated, FOVRecalculated);
    }

    void FOVRecalculated(GameEvent gameEvent)
    {
        if(!IsOwner) 
            return;

        var newVisibleTiles = gameEvent.GetValue<List<Point>>(EventParameter.VisibleTiles);

        var visibleTileDifference = m_VisiblePoints.Except(newVisibleTiles);
        foreach (var tilePoint in visibleTileDifference)
            Services.Map.GetTile(tilePoint).SetVisibility(false);

        foreach(var tilePoint in newVisibleTiles)
            Services.Map.GetTile(tilePoint).SetVisibility(true);

        m_VisiblePoints = newVisibleTiles;
    }
}
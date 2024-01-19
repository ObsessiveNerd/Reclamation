using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class PlayerFOVHandlerData : EntityComponent
{
    public Action OnFOVRecalculated;
    public List<Point> VisiblePoints = new List<Point>();
    
    Type MonobehaviorType = typeof(PlayerFOVHandler);

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.FOVRecalculated, FOVRecalculated);
    }

    void FOVRecalculated(GameEvent gameEvent)
    {
        VisiblePoints = gameEvent.GetValue<List<Point>>(EventParameter.VisibleTiles);
        if(OnFOVRecalculated != null)
            OnFOVRecalculated();
    }
}

public class PlayerFOVHandler : ComponentBehavior<PlayerFOVHandlerData>
{
    private List<Point> m_VisiblePoints = new List<Point>();

    void Start()
    {
        component.OnFOVRecalculated += FOVRecalculated;
        FOVRecalculated();
    }

    public override void OnDestroy()
    {
        component.OnFOVRecalculated -= FOVRecalculated;
    }

    void FOVRecalculated()
    {
        if(!IsOwner) 
            return;

        var newVisibleTiles = component.VisiblePoints;
        var visibleTileDifference = m_VisiblePoints.Except(newVisibleTiles);
        foreach (var tilePoint in visibleTileDifference)
            Services.Map.GetTile(tilePoint).SetVisibility(false);

        foreach(var tilePoint in newVisibleTiles)
            Services.Map.GetTile(tilePoint).SetVisibility(true);

        m_VisiblePoints = newVisibleTiles;
    }
}
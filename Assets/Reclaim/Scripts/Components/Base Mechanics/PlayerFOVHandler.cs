using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class PlayerFOVHandlerData : EntityComponent
{
    public Action<GameEvent> OnFOVRecalculated;

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.FOVRecalculated, FOVRecalculated);
    }

    void FOVRecalculated(GameEvent gameEvent)
    {
        OnFOVRecalculated(gameEvent);
    }
}

public class PlayerFOVHandler : EntityComponentBehavior
{
    public PlayerFOVHandlerData Data = new PlayerFOVHandlerData();

    private List<Point> m_VisiblePoints = new List<Point>();

    void Start()
    {
        Data.OnFOVRecalculated += FOVRecalculated;
    }

    public override void OnDestroy()
    {
        Data.OnFOVRecalculated -= FOVRecalculated;
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

    public override IComponent GetData()
    {
        return Data;
    }
}
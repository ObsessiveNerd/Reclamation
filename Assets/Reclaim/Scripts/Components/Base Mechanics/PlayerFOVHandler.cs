//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerFOVHandler : EntityComponent
//{
//    private List<Point> m_VisiblePoints = new List<Point>();

//    public void Start()
//    {
//        RegisteredEvents.Add(GameEventId.FOVRecalculated, FOVRecalculated);
//        RegisteredEvents.Add(GameEventId.GetVisibleTiles, GetVisibleTiles);
//        RegisteredEvents.Add(GameEventId.IsInFOV, IsInFOV);
//    }

//    void FOVRecalculated(GameEvent gameEvent)
//    {
//        m_VisiblePoints = gameEvent.GetValue<List<Point>>(EventParameter.VisibleTiles); //(List<Point>)gameEvent.Paramters[EventParameters.VisibleTiles];
//        Services.FOVService.FoVRecalculated(gameObject, m_VisiblePoints);
//    }

//    void GetVisibleTiles(GameEvent gameEvent)
//    {
//        gameEvent.Paramters[EventParameter.VisibleTiles] = m_VisiblePoints;

//    }

//    void IsInFOV(GameEvent gameEvent)
//    {
//        var target = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
//        gameEvent.Paramters[EventParameter.Value] = m_VisiblePoints.Contains(WorldUtility.GetEntityPosition(target));
//    }
//}
    //using System.Collections;
    //using System.Collections.Generic;
    //using UnityEngine;

    //public class AIFOVHandler : EntityComponent
    //{
    //    private List<Point> m_VisiblePoints = new List<Point>();
    //    public void Start()
    //    {
        
    //        RegisteredEvents.Add(GameEventId.FOVRecalculated, FOVRecalculated);
    //        RegisteredEvents.Add(GameEventId.GetVisibleTiles, GetVisibleTiles);
    //    }

    //    void FOVRecalculated(GameEvent gameEvent)
    //    {
    //        m_VisiblePoints = (List<Point>)gameEvent.Paramters[EventParameter.VisibleTiles];

    //    }

    //    void GetVisibleTiles(GameEvent gameEvent)
    //    {
    //        gameEvent.Paramters[EventParameter.VisibleTiles] = m_VisiblePoints;
    //    }
    //}

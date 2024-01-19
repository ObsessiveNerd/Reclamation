using System;
using System.Collections.Generic;

[Serializable]
public class FOVData : EntityComponent
{ 
    public int FOVRange;
    
    IFovAlgorithm m_Fov;
    List<Point> m_VisibleTiles;

    Type MonobehaviorType = typeof(FOV);

    public override void WakeUp()
    {
        m_Fov = new Shadowcasting();

        UpdateFOV();
        RegisteredEvents.Add(GameEventId.AfterMoving, AfterMoving);
        RegisteredEvents.Add(GameEventId.GetVisibleTiles, GetVisibleTiles);
    }
    void UpdateFOV()
    {
        int baseRange = FOVRange;
        GameEvent beforeFOVCalculated = GameEventPool.Get(GameEventId.BeforeFOVRecalculated)
                .With(EventParameter.FOVRange, FOVRange);
        
        m_VisibleTiles = m_Fov.GetVisibleTiles(Entity.GameObject, FOVRange);

        GameEvent afterFOVCalculated = GameEventPool.Get(GameEventId.FOVRecalculated)
            .With(EventParameter.VisibleTiles, m_VisibleTiles);
        Entity.FireEvent(afterFOVCalculated).Release();

        FOVRange = baseRange;
        beforeFOVCalculated.Release();
    }

    void AfterMoving(GameEvent gameEvent)
    {
        UpdateFOV();
    }

    void GetVisibleTiles(GameEvent gameEvent)
    {
        gameEvent.SetValue<List<Point>>(EventParameter.VisibleTiles, m_VisibleTiles);
    }
}

public class FOV : ComponentBehavior<FOVData>
{
    
}
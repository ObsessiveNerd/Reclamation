using System;
using System.Collections.Generic;

[Serializable]
public class FOVData : ComponentData
{ 
    public int FOVRange;
}

public class FOV : EntityComponent
{
    public FOVData Data = new FOVData();
    IFovAlgorithm m_Fov;
    List<Point> m_VisibleTiles;

    void Start()
    {
        m_Fov = new Shadowcasting();

        UpdateFOV();
        RegisteredEvents.Add(GameEventId.AfterMoving, AfterMoving);
        RegisteredEvents.Add(GameEventId.GetVisibleTiles, GetVisibleTiles);
    }

    public override void WakeUp(IComponentData data = null)
    {
        if(data != null)
            Data = data as FOVData;
    }

    public override IComponentData GetData()
    {
        return Data;
    }

    void UpdateFOV()
    {
        int baseRange = Data.FOVRange;
        GameEvent beforeFOVCalculated = GameEventPool.Get(GameEventId.BeforeFOVRecalculated)
                .With(EventParameter.FOVRange, Data.FOVRange);
        
        m_VisibleTiles = m_Fov.GetVisibleTiles(gameObject, Data.FOVRange);

        GameEvent afterFOVCalculated = GameEventPool.Get(GameEventId.FOVRecalculated)
            .With(EventParameter.VisibleTiles, m_VisibleTiles);
        gameObject.FireEvent(afterFOVCalculated).Release();

        Data.FOVRange = baseRange;
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
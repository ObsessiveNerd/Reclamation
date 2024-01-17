using System.Collections.Generic;

public class FOVData : IComponentData
{ 
    public int FOVRange;
}

public class FOV : EntityComponent
{
    public FOVData Data = new FOVData();
    IFovAlgorithm m_Fov;
    List<Point> m_VisibleTiles;

    public override void WakeUp(IComponentData data = null)
    {
        m_Fov = new Shadowcasting();
        if(data != null)
            Data = data as FOVData;

        UpdateFOV();
        RegisteredEvents.Add(GameEventId.AfterMoving, AfterMoving);
        RegisteredEvents.Add(GameEventId.GetVisibleTiles, GetVisibleTiles);
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
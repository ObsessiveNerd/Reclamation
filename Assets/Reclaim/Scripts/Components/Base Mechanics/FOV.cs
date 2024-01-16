using System.Collections.Generic;

public class FOV : EntityComponent
{
    IFovAlgorithm m_Fov;
    public int FOVRange;

    List<Point> m_VisibleTiles;

    public void Start()
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
        
        m_VisibleTiles = m_Fov.GetVisibleTiles(gameObject, FOVRange);

        GameEvent afterFOVCalculated = GameEventPool.Get(GameEventId.FOVRecalculated)
            .With(EventParameter.VisibleTiles, m_VisibleTiles);
        gameObject.FireEvent(afterFOVCalculated).Release();

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
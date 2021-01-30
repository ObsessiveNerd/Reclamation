using System.Collections.Generic;

public class FOV : Component
{
    IFovAlgorithm m_Fov;
    public int FOVRange;

    public FOV(int visibleRange)
    {
        FOVRange = visibleRange;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.AfterMoving);
        m_Fov = new Shadowcasting();

        //FireEvent(World.Instance.Self, new GameEvent(GameEventId.FOVRecalculated, new KeyValuePair<string, object>(EventParameters.Entity, Self),
        //                                                                                new KeyValuePair<string, object>(EventParameters.VisibleTiles, m_Fov.GetVisibleTiles(Self, FOVRange))));
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.AfterMoving)
            FireEvent(World.Instance.Self, new GameEvent(GameEventId.FOVRecalculated, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                        new KeyValuePair<string, object>(EventParameters.VisibleTiles, m_Fov.GetVisibleTiles(Self, FOVRange))));
    }
}

public class DTO_FOV : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        int visibleRange = int.Parse(data.Split('=')[1]);
        Component = new FOV(visibleRange);
    }

    public string CreateSerializableData(IComponent component)
    {
        FOV fov = (FOV)component;
        return $"{nameof(FOV)}: VisibleRange={fov.FOVRange}";
    }
}
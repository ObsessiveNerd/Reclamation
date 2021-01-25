using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        RegisteredEvents.Add(GameEventId.ExecuteMove);
        m_Fov = new Shadowcasting();

        //TODO: we may want to gather visible tiles and run a self check to make sure we don't have some kind of true sight or anything to alter the visible tiles

        FireEvent(World.Instance.Self, new GameEvent(GameEventId.FOVRecalculated, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                        new KeyValuePair<string, object>(EventParameters.VisibleTiles, m_Fov.GetVisibleTiles(Self, FOVRange))));
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.ExecuteMove)
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
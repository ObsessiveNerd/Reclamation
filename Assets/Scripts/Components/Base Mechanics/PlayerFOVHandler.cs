using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFOVHandler : Component
{
    private List<Point> m_VisiblePoints = new List<Point>();

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.FOVRecalculated);
        RegisteredEvents.Add(GameEventId.GetVisibleTiles);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.FOVRecalculated)
        {
            m_VisiblePoints = (List<Point>)gameEvent.Paramters[EventParameters.VisibleTiles];
            FireEvent(World.Instance.Self, gameEvent);
        }
        else if (gameEvent.ID == GameEventId.GetVisibleTiles)
            gameEvent.Paramters[EventParameters.VisibleTiles] = m_VisiblePoints;
    }
}

public class DTO_PlayerFOVHandler : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new PlayerFOVHandler();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(PlayerFOVHandler);
    }
}

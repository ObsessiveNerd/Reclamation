using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFOVHandler : EntityComponent
{
    private List<Point> m_VisiblePoints = new List<Point>();

    public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.FOVRecalculated);
        RegisteredEvents.Add(GameEventId.GetVisibleTiles);
        RegisteredEvents.Add(GameEventId.IsInFOV);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.FOVRecalculated)
        {
            m_VisiblePoints = gameEvent.GetValue<List<Point>>(EventParameter.VisibleTiles); //(List<Point>)gameEvent.Paramters[EventParameters.VisibleTiles];
            Services.FOVService.FoVRecalculated(Self, m_VisiblePoints);
        }

        else if (gameEvent.ID == GameEventId.GetVisibleTiles)
            gameEvent.Paramters[EventParameter.VisibleTiles] = m_VisiblePoints;

        else if(gameEvent.ID == GameEventId.IsInFOV)
        {
            var target = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
            gameEvent.Paramters[EventParameter.Value] = m_VisiblePoints.Contains(WorldUtility.GetEntityPosition(target));
        }
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

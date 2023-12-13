using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFOVHandler : EntityComponent
{
    private List<Point> m_VisiblePoints = new List<Point>();
    public override void Init(GameObject self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.FOVRecalculated);
        RegisteredEvents.Add(GameEventId.GetVisibleTiles);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.FOVRecalculated)
        {
            m_VisiblePoints = (List<Point>)gameEvent.Paramters[EventParameter.VisibleTiles];
            //foreach(var tile in m_VisiblePoints)
            //{
            //    GameObject helm = EntityFactory.CreateEntity("Helmet");
            //    Spawner.Spawn(helm, tile);
            //}
        }
        else if (gameEvent.ID == GameEventId.GetVisibleTiles)
            gameEvent.Paramters[EventParameter.VisibleTiles] = m_VisiblePoints;
    }
}

public class DTO_AIFOVHandler : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new AIFOVHandler();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(AIFOVHandler);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggression : Component
{
    public int BaseAggression;
    public override int Priority => 1;

    Point m_CurrentLocation;
    Point m_TargetLocation;

    public Aggression(int baseAggression)
    {
        BaseAggression = baseAggression;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetActionToTake);
        RegisteredEvents.Add(GameEventId.GetTileAggression);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetActionToTake)
        {
            m_CurrentLocation = PathfindingUtility.GetEntityLocation(Self);
            EventBuilder getMyAggressionLevel = new EventBuilder(GameEventId.GetTileAggression)
                                                        .With(EventParameters.Value, -1);

            int myAggressionLevel = FireEvent(Self, getMyAggressionLevel.CreateEvent()).GetValue<int>(EventParameters.Value);

            EventBuilder getVisiblePoints = new EventBuilder(GameEventId.GetVisibleTiles)
                                            .With(EventParameters.VisibleTiles, new List<Point>());
            List<Point> visiblePoints = FireEvent(Self, getVisiblePoints.CreateEvent()).GetValue<List<Point>>(EventParameters.VisibleTiles);
            foreach(var point in visiblePoints)
            {
                if (point == m_CurrentLocation) continue;

                EventBuilder getCombatRatingOfTile = new EventBuilder(GameEventId.GetTileAggression)
                                                        .With(EventParameters.TilePosition, point)
                                                        .With(EventParameters.Value, -1);

                int tileAggressionLevel = FireEvent(World.Instance.Self, getCombatRatingOfTile.CreateEvent()).GetValue<int>(EventParameters.Value);

                if(tileAggressionLevel > -1 && tileAggressionLevel <= myAggressionLevel)
                {
                    m_TargetLocation = point;
                    AIAction attackAction = new AIAction()
                    {
                        Priority = 2,
                        ActionToTake = MakeAttack
                    };
                    gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameters.AIActionList).Add(attackAction);
                }
            }
        }
        else if(gameEvent.ID == GameEventId.GetTileAggression)
        {
            gameEvent.Paramters[EventParameters.Value] = BaseAggression;
        }
    }

    //Todo: will need to check for ranged weapons and perform ranged attack if it wants to
    MoveDirection MakeAttack()
    {
        var path = PathfindingUtility.GetPath(m_CurrentLocation, m_TargetLocation);
        return PathfindingUtility.GetDirectionTo(m_CurrentLocation, path[0]);
    }
}

public class DTO_Aggression : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string[] parameters = data.Split('=');
        int baseAggression = int.Parse(parameters[1]);
        Component = new Aggression(baseAggression);
    }

    public string CreateSerializableData(IComponent component)
    {
        Aggression agg = (Aggression)component;
        return $"{nameof(Aggression)}:BaseAggression={agg.BaseAggression}";
    }
}

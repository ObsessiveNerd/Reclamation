using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fear : Component
{
    Point m_CurrentLocation;
    Point m_TargetLocation;
    //int m_FearMargin = 2;

   public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetActionToTake);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetActionToTake)
        {
            m_CurrentLocation = PathfindingUtility.GetEntityLocation(Self);
            EventBuilder getMyAggressionLevel = new EventBuilder(GameEventId.GetCombatRating)
                                                        .With(EventParameters.Value, -1);

            int myCombatLevel = FireEvent(Self, getMyAggressionLevel.CreateEvent()).GetValue<int>(EventParameters.Value);

            EventBuilder getVisiblePoints = new EventBuilder(GameEventId.GetVisibleTiles)
                                            .With(EventParameters.VisibleTiles, new List<Point>());
            List<Point> visiblePoints = FireEvent(Self, getVisiblePoints.CreateEvent()).GetValue<List<Point>>(EventParameters.VisibleTiles);
            foreach(var point in visiblePoints)
            {
                if (point == m_CurrentLocation) continue;

                EventBuilder getEntity = new EventBuilder(GameEventId.GetEntityOnTile)
                                                        .With(EventParameters.TilePosition, point)
                                                        .With(EventParameters.Entity, "");

                IEntity target = EntityQuery.GetEntity(FireEvent(World.Instance.Self, getEntity.CreateEvent()).GetValue<string>(EventParameters.Entity));
                if (target == null) continue;

                if (Factions.GetDemeanorForTarget(Self, target) != Demeanor.Hostile) continue;

                EventBuilder getCombatRatingOfTile = new EventBuilder(GameEventId.GetCombatRating)
                                                        .With(EventParameters.TilePosition, point)
                                                        .With(EventParameters.Value, -1);

                int targetCombatRating = FireEvent(target, getCombatRatingOfTile.CreateEvent()).GetValue<int>(EventParameters.Value);

                if(CombatUtility.AmIAfraid(myCombatLevel, targetCombatRating))
                {
                    m_TargetLocation = point;
                    AIAction attackAction = new AIAction()
                    {
                        Priority = 1,
                        ActionToTake = RunAway
                    };
                    gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameters.AIActionList).Add(attackAction);
                    break;
                }
            }
        }
    }

    MoveDirection RunAway()
    {
        Point randomPoint = PathfindingUtility.GetRandomValidPoint();
        var path = PathfindingUtility.GetPath(m_CurrentLocation, randomPoint);
        FireEvent(Self, new GameEvent(GameEventId.BreakRank));
        if(path.Count >= 1)
            return PathfindingUtility.GetDirectionTo(m_CurrentLocation, path[0]);
        return PathfindingUtility.GetDirectionTo(m_CurrentLocation, m_CurrentLocation);
    }
}

public class DTO_Fear : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Fear();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Fear);
    }
}
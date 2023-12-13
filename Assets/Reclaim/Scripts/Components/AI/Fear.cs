using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fear : EntityComponent
{
    Point m_CurrentLocation;
    Point m_TargetLocation;
    //int m_FearMargin = 2;

   public void Start()
    {
        
        RegisteredEvents.Add(GameEventId.GetActionToTake);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetActionToTake)
        {
            m_CurrentLocation = PathfindingUtility.GetEntityLocation(Self);
            GameEvent getMyAggressionLevel = GameEventPool.Get(GameEventId.GetCombatRating)
                                                        .With(EventParameter.Value, -1);

            int myCombatLevel = FireEvent(Self, getMyAggressionLevel).GetValue<int>(EventParameter.Value);
            getMyAggressionLevel.Release();

            GameEvent getVisiblePoints = GameEventPool.Get(GameEventId.GetVisibleTiles)
                                            .With(EventParameter.VisibleTiles, new List<Point>());
            List<Point> visiblePoints = FireEvent(Self, getVisiblePoints).GetValue<List<Point>>(EventParameter.VisibleTiles);
            getVisiblePoints.Release();
            
            foreach(var point in visiblePoints)
            {
                if (point == m_CurrentLocation) continue;

                GameObject target = Services.WorldDataQuery.GetEntityOnTile(point);
                if (target == null) continue;

                if (Factions.GetDemeanorForTarget(Self, target) != Demeanor.Hostile) continue;

                GameEvent getCombatRatingOfTile = GameEventPool.Get(GameEventId.GetCombatRating)
                                                        .With(EventParameter.TilePosition, point)
                                                        .With(EventParameter.Value, -1);

                int targetCombatRating = FireEvent(target, getCombatRatingOfTile).GetValue<int>(EventParameter.Value);
                getCombatRatingOfTile.Release();

                if(CombatUtility.AmIAfraid(myCombatLevel, targetCombatRating))
                {
                    m_TargetLocation = point;
                    AIAction attackAction = new AIAction()
                    {
                        Priority = 1,
                        ActionToTake = RunAway
                    };
                    gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameter.AIActionList).Add(attackAction);
                    break;
                }
            }
        }
    }

    MoveDirection RunAway()
    {
        //Point randomPoint = PathfindingUtility.GetRandomValidPoint();
        var test = PathfindingUtility.GetDirectionTo(m_CurrentLocation, m_TargetLocation);
        FireEvent(Self, GameEventPool.Get(GameEventId.BreakRank), true);
        return PathfindingUtility.GetDirectionAwayFrom(m_CurrentLocation, m_TargetLocation);
        //var path = PathfindingUtility.GetPath(m_CurrentLocation, randomPoint);
        //if(path.Count >= 1)
        //    return PathfindingUtility.GetDirectionTo(m_CurrentLocation, path[0]);
        //return PathfindingUtility.GetDirectionTo(m_CurrentLocation, m_CurrentLocation);
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
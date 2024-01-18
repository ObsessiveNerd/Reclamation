using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AgressionData : EntityComponent
{
    public Point CurrentLocation;
    public Point TargetLocation;
    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.GetActionToTake, GetActionToTake);
    }

    void GetActionToTake(GameEvent gameEvent)
    {
        CurrentLocation = Entity.GetComponent<PositionData>().Point;
        GameEvent getVisiblePoints = GameEventPool.Get(GameEventId.GetVisibleTiles)
                                            .With(EventParameter.VisibleTiles, new List<Point>());
        List<Point> visiblePoints = Entity.FireEvent(getVisiblePoints).GetValue<List<Point>>(EventParameter.VisibleTiles);
        getVisiblePoints.Release();

        foreach (var point in visiblePoints)
        {
            if (point == CurrentLocation)
                continue;

            HashSet<GameObject> targets = Services.Map.GetTile(point).Objects;

            foreach (var target in targets)
            {
                if (Factions.GetDemeanorForTarget(Entity.GameObject, target) != Demeanor.Hostile)
                    continue;

                TargetLocation = point;
                AIAction attackAction = new AIAction()
                {
                    Priority = 2,
                    ActionToTake = MakeAttack
                };
                gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameter.AIActionList).Add(attackAction);
                break;
            }
        }
    }

    MoveDirection MakeAttack()
    {
        var path = Services.Pathfinder.GetPath(CurrentLocation, TargetLocation);
        if (path.Count == 0)
            return MoveDirection.None;

        return Services.Pathfinder.GetDirectionTo(CurrentLocation, path[0]);
    }
}


public class Aggression : EntityComponentBehavior
{
    public AgressionData Data = new AgressionData();

    public override IComponent GetData()
    {
        return Data;
    }
}

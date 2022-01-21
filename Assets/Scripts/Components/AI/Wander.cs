using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Component
{
    public int Weight => 5;
    private Point m_Destination;
    private List<Point> m_CurrentPath;
    private Point m_PreviousPosition;

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetActionToTake);
    }

    public override void Start()
    {
        m_Destination = PathfindingUtility.GetRandomValidPoint();
        m_CurrentPath = new List<Point>();
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetActionToTake)
        {
            PriorityQueue<AIAction> actionQueue = gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameters.AIActionList);
            AIAction action = new AIAction()
            {
                Priority = Weight,
                ActionToTake = WanderAction
            };
            actionQueue.Add(action);
        }
    }

    MoveDirection WanderAction()
    {
        Point currentPos = PathfindingUtility.GetEntityLocation(Self);

        //if (m_PreviousPosition == currentPos)
        //{
        //    m_CurrentPath.Clear();
        //    m_Destination = PathfindingUtility.GetRandomValidPoint();
        //}

        if (m_Destination == null || currentPos == m_Destination)
            m_Destination = PathfindingUtility.GetRandomValidPoint();

        if (Services.TileInteractionService.GetTile(m_Destination).BlocksMovement)
            return MoveDirection.None;

        if (m_CurrentPath.Count == 0 || !IsNeighbor(currentPos, m_CurrentPath[0]))
            m_CurrentPath = PathfindingUtility.GetPath(currentPos, m_Destination);

        if (m_CurrentPath.Count == 0)
            return MoveDirection.None;

        //m_PreviousPosition = currentPos;

        Point nextNode = m_CurrentPath[0];
        m_CurrentPath.RemoveAt(0);
        return PathfindingUtility.GetDirectionTo(currentPos, nextNode);
    }

    bool IsNeighbor(Point current, Point neighbor)
    {
        for(int i = current.x - 1; i < current.x + 1; i++)
        {
            for(int j = current.y - 1; j < current.y + 1; j++)
            {
                Point p = new Point(i, j);
                if (neighbor == p)
                    return true;
            }
        }
        return false;
    }
}

public class DTO_Wander : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Wander();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Wander);
    }
}
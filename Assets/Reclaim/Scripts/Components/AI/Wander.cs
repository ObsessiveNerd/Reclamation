//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Wander : EntityComponent
//{
//    public int Weight => 5;
//    private Point m_Destination;
//    private List<Point> m_CurrentPath;
//    private Point m_PreviousPosition;

//    public override void WakeUpIComponentData data = null()
//    {
//        RegisteredEvents.Add(GameEventId.GetActionToTake, GetActionToTake);

//        m_Destination = PathfindingUtility.GetRandomValidPoint();
//        m_CurrentPath = new List<Point>();
        
//    }

//    void GetActionToTake(GameEvent gameEvent)
//    {
//        PriorityQueue<AIAction> actionQueue = gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameter.AIActionList);
//            AIAction action = new AIAction()
//            {
//                Priority = Weight,
//                ActionToTake = WanderAction
//            };
//            actionQueue.Add(action);
//    }

//    MoveDirection WanderAction()
//    {
//        Point currentPos = PathfindingUtility.GetEntityLocation(gameObject);

//        if (m_Destination == null || currentPos == m_Destination)
//            m_Destination = PathfindingUtility.GetRandomValidPoint();

//        //TODO, get tile for m_Destination
//        Tile t = new Tile();
        
//        if (t == null || t.BlocksMovement)
//            return MoveDirection.None;

//        if (m_CurrentPath.Count == 0 || !IsNeighbor(currentPos, m_CurrentPath[0]))
//            m_CurrentPath = PathfindingUtility.GetPath(currentPos, m_Destination);

//        if (m_CurrentPath.Count == 0)
//            return MoveDirection.None;

//        Point nextNode = m_CurrentPath[0];
//        m_CurrentPath.RemoveAt(0);
//        return PathfindingUtility.GetDirectionTo(currentPos, nextNode);
//    }

//    bool IsNeighbor(Point current, Point neighbor)
//    {
//        for(int i = current.x - 1; i < current.x + 1; i++)
//        {
//            for(int j = current.y - 1; j < current.y + 1; j++)
//            {
//                Point p = new Point(i, j);
//                if (neighbor == p)
//                    return true;
//            }
//        }
//        return false;
//    }
//}
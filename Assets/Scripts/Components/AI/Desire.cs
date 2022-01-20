using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desire : Component
{
    public int Greed = 0;

    private Point m_CurrentDestination = Point.InvalidPoint;
    private Point m_CurrentPosition = Point.InvalidPoint;
    private int m_DesiredValue = 0;
    private List<IMapNode> m_CurrentPath = new List<IMapNode>();

    public Desire(int greedModifier)
    {
        Greed = greedModifier;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetActionToTake);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetActionToTake)
        {
            m_CurrentPosition = PathfindingUtility.GetEntityLocation(Self);
            if (m_CurrentDestination != Point.InvalidPoint && m_CurrentDestination != m_CurrentPosition && m_CurrentPath.Count > 0)
            {
                AIAction continueOnPath = new AIAction()
                {
                    Priority = GetPriority(),
                    ActionToTake = MoveOrPickup
                };

                gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameters.AIActionList).Add(continueOnPath);
                return;
            }

            GameEvent getVisiblePoints = GameEventPool.Get(GameEventId.GetVisibleTiles)
                                           .With(EventParameters.VisibleTiles, new List<Point>());

            List<Point> visiblePoints = FireEvent(Self, getVisiblePoints).GetValue<List<Point>>(EventParameters.VisibleTiles);
            getVisiblePoints.Release();
            m_CurrentDestination = Point.InvalidPoint;
            m_DesiredValue = 0;
            foreach (var point in visiblePoints)
            {
                GameEvent getTileValue = GameEventPool.Get(GameEventId.GetValueOnTile)
                                            .With(EventParameters.TilePosition, point)
                                            .With(EventParameters.Value, 0);
                int valueOnTile = FireEvent(World.Instance.Self, getTileValue).GetValue<int>(EventParameters.Value);
                getTileValue.Release();
                if(valueOnTile > m_DesiredValue)
                {
                    m_CurrentDestination = point;
                    m_DesiredValue = valueOnTile;
                }
            }

            if (m_DesiredValue > 0 && m_CurrentDestination != Point.InvalidPoint)
            {
                AIAction moveToTreasure = new AIAction()
                {
                    Priority = GetPriority(),
                    ActionToTake = MoveOrPickup
                };
                gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameters.AIActionList).Add(moveToTreasure);
            }
        }
    }

    MoveDirection MoveOrPickup()
    {
        if (m_CurrentPosition == m_CurrentDestination)
        {
            GameEvent pickupItem = GameEventPool.Get(GameEventId.Pickup)
                                        .With(EventParameters.Entity, Self.ID);
            FireEvent(World.Instance.Self, pickupItem).Release();

            GameEvent tryEquip = GameEventPool.Get(GameEventId.TryEquip)
                                        .With(EventParameters.Entity, Self.ID);
            Self.FireEvent(tryEquip).Release();

            return MoveDirection.None;
        }
        else
        {
            m_CurrentPath = PathfindingUtility.GetPath(m_CurrentPosition, m_CurrentDestination);
            if (m_CurrentPath.Count == 0)
                return MoveDirection.None;
            var retValue = PathfindingUtility.GetDirectionTo(m_CurrentPosition, m_CurrentPath.Count == 0 ? m_CurrentPosition : m_CurrentPath[0]);
            m_CurrentPath.RemoveAt(0);
            return retValue;
        }
    }

    int GetPriority()
    {
        return Mathf.Max(0, 5 - ((m_DesiredValue / 100) + Greed));
    }
}

public class DTO_Desire : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string[] kvp = data.Split('=');
        if (kvp.Length == 2)
        {
            int mod = int.Parse(kvp[1]);
            Component = new Desire(mod);
        }
        else
            Component = new Desire(0);
    }

    public string CreateSerializableData(IComponent component)
    {
        Desire d = (Desire)component;
        return $"{nameof(Desire)}:{d.Greed}";
    }
}
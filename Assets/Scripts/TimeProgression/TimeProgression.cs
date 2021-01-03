using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeProgression
{
    LinkedList<IEntity> m_EntityList = new LinkedList<IEntity>();
    LinkedListNode<IEntity> m_Previous;
    LinkedListNode<IEntity> m_Current;

    public void RegisterEntity(IEntity entity)
    {
        m_EntityList.AddLast(entity);
    }

    public void RemoveEntity(IEntity entity)
    {
        m_EntityList.Remove(entity);
    }

    public void Update()
    {
        if (m_EntityList.Count == 0)
            return;
        if (m_Current == null)
        {
            m_Current = m_EntityList.First;
            GameEvent startTurn = new GameEvent(GameEventId.StartTurn);
            m_Current.Value.HandleEvent(startTurn);
        }

        GameEvent update = new GameEvent(GameEventId.UpdateEntity,  new KeyValuePair<string, object>(EventParameters.TakeTurn, false),
                                                                    new KeyValuePair<string, object>(EventParameters.UpdateWorld, false),
                                                                    new KeyValuePair<string, object>(EventParameters.CleanupComponents, false));
        m_Current.Value.HandleEvent(update);

        if ((bool)update.Paramters[EventParameters.CleanupComponents])
            m_Current.Value.CleanupComponents();

        if((bool)update.Paramters[EventParameters.UpdateWorld])
            World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.UpdateWorldView));

        if ((bool)update.Paramters[EventParameters.TakeTurn])
        {
            Debug.Log($"End turn: {m_Current.Value}");
            GameEvent endTurn = new GameEvent(GameEventId.EndTurn);
            m_Current.Value.HandleEvent(endTurn);
            m_Current.Value.CleanupComponents();

            m_Previous = m_Current;
            m_Current = m_Current.Next;

            if (m_Current != null)
            {
                GameEvent startTurn = new GameEvent(GameEventId.StartTurn);
                m_Current.Value.HandleEvent(startTurn);
            }
            World.Instance.Self.FireEvent(World.Instance.Self, new GameEvent(GameEventId.UpdateWorldView));
        }
    }
}

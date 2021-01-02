using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeProgression : MonoBehaviour
{
    LinkedList<IEntity> m_EntityList = new LinkedList<IEntity>();
    LinkedListNode<IEntity> m_Previous;
    LinkedListNode<IEntity> m_Current;
    IEntity m_World;

    public void RegisterEntity(IEntity entity)
    {
        m_EntityList.AddLast(entity);
    }

    public void RemoveEntity(IEntity entity)
    {
        m_EntityList.Remove(entity);
    }

    public void RegisterWorld(IEntity world)
    {
        m_World = world;
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

        GameEvent update = new GameEvent(GameEventId.UpdateEntity, new KeyValuePair<string, object>(EventParameters.TakeTurn, true));
        m_Current.Value.HandleEvent(update);

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
            m_World.FireEvent(m_World, new GameEvent(GameEventId.UpdateWorld));
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeProgression
{
    LinkedList<GameObject> m_EntityList = new LinkedList<GameObject>();
    LinkedListNode<GameObject> m_Previous;
    LinkedListNode<GameObject> m_Current;
    Action m_PostFrameCallback;
    bool m_IsStopped = false;

    public void RegisterEntity(GameObject entity)
    {
        if(!m_EntityList.Contains(entity))
            m_EntityList.AddLast(entity);
    }

    public void RemoveEntity(GameObject entity)
    {
        if(m_EntityList.Contains(entity))
            m_EntityList.Remove(entity);
    }

    public bool ContainsEntity(GameObject entity)
    {
        return m_EntityList.Contains(entity);
    }

    public void SetPostFrameCallback(Action callback)
    {
        m_PostFrameCallback = callback;
    }

    public void Resume()
    {
        m_IsStopped = false;
    }


    public void Stop()
    {
        m_IsStopped = true;
    }

    public void SetActiveEntity(GameObject entity)
    {
        m_PostFrameCallback = () =>
        {
            m_Previous = m_Current;
            m_Current = m_EntityList.Find(entity);

            if (m_Current != null)
            {
                GameEvent characterRotated = GameEventPool.Get(GameEventId.CharacterRotated);
                m_Current.Value.HandleEvent(characterRotated);
                characterRotated.Release();
            }
        };
    }

    public void ProgressTimeUntilEntityHasTakenTurn(string id)
    {
        Update();
        if (m_Current == null)
            Update();

        if (m_Current.Value.ID == id)
        {
            Update();
            if (m_Current == null)
                Update();
        }

        while (m_Current.Value.ID != id)
        {
            Update();
            if (m_Current == null)
                Update();
        }
        Update();
    }
    
    GameEvent update = new GameEvent(GameEventId.UpdateEntity)
        .With(EventParameter.TakeTurn, false)
        .With(EventParameter.UpdateWorldView, false);

    public void Update()
    {
        update.Paramters[EventParameter.TakeTurn] = false;
        update.Paramters[EventParameter.UpdateWorldView] = false;
        update.ContinueProcessing = true;

        if (m_EntityList.Count == 0 || m_IsStopped)
        {
            return;
        }

        if (m_Current == null)
        {
            m_Current = m_EntityList.First;
            GameEvent startTurn = GameEventPool.Get(GameEventId.StartTurn);
            m_Current.Value.HandleEvent(startTurn);
            startTurn.Release();
        }

        using (new DiagnosticsTimer("Update entity"))
            m_Current.Value.HandleEvent(update);

        if (m_Current.Value.NeedsCleanup)
        { 
            m_Current.Value.CleanupComponents();
            update.Paramters[EventParameter.UpdateWorldView] = true;
        }


        using (new DiagnosticsTimer("Update world view"))
        {
            try
            {
                if ((bool)update.Paramters[EventParameter.UpdateWorldView])
                    Services.WorldUpdateService.UpdateWorldView();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        Services.SpawnerService.DespawnAllRegistered();

        if ((bool)update.Paramters[EventParameter.TakeTurn])
        {
            GameEvent endTurn = GameEventPool.Get(GameEventId.EndTurn);
            m_Current.Value.HandleEvent(endTurn);
            m_Current.Value.CleanupComponents();

            m_Current = m_Current.Next;

            if (m_Current != null)
            {
                GameEvent startTurn = GameEventPool.Get(GameEventId.StartTurn);
                m_Current.Value.HandleEvent(startTurn);
                startTurn.Release();
            }
            endTurn.Release();
            Services.WorldUpdateService.UpdateWorldView();
            Services.WorldUIService.UpdateUI();
        }

        //ObjectPool.Return(update);
        if (m_PostFrameCallback != null)
        {
            m_PostFrameCallback.Invoke();
            m_PostFrameCallback = null;
        }
    }
}

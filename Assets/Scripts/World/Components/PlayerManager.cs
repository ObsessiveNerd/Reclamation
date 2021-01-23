using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : WorldComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.RotateActiveCharacter);
        RegisteredEvents.Add(GameEventId.ConvertToPlayableCharacter);
        RegisteredEvents.Add(GameEventId.RegisterPlayableCharacter);
        RegisteredEvents.Add(GameEventId.UnRegisterPlayer);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.RotateActiveCharacter)
        {
            if (m_Players.Count == 1)
                return;

            m_ActivePlayer.Value.RemoveComponent(typeof(PlayerInputController));
            m_ActivePlayer.Value.AddComponent(new AIController());
            //m_ActivePlayer.Value.CleanupComponents();

            m_ActivePlayer = m_ActivePlayer.Next;
            if (m_ActivePlayer == null)
                m_ActivePlayer = m_Players.First;

            m_ActivePlayer.Value.CleanupComponents();
            m_ActivePlayer.Value.RemoveComponent(typeof(AIController));
            m_ActivePlayer.Value.AddComponent(new PlayerInputController());
            m_ActivePlayer.Value.CleanupComponents();

            m_TimeProgression.SetActiveEntity(m_ActivePlayer.Value);
        }

        //if(gameEvent.ID == GameEventId.GetActivePlayer)
        //{
        //    gameEvent.Paramters[EventParameters.Entity] = m_ActivePlayer.Value;
        //}

        if (gameEvent.ID == GameEventId.ConvertToPlayableCharacter)
        {
            ConvertToPlayableEntity((IEntity)gameEvent.Paramters[EventParameters.Entity]);
        }

        if (gameEvent.ID == GameEventId.RegisterPlayableCharacter)
        {
            RegisterPlayer((IEntity)gameEvent.Paramters[EventParameters.Entity]);
        }

        if (gameEvent.ID == GameEventId.UnRegisterPlayer)
        {
            UnregisterPlayer((IEntity)gameEvent.Paramters[EventParameters.Entity]);
        }
    }

    public void RegisterPlayer(IEntity entity)
    {
        if (!entity.GetComponents().Any(c => c.GetType() == typeof(RegisterPlayableCharacter)))
            ConvertToPlayableEntity(entity);

        m_Players.AddLast(entity);
        if (m_ActivePlayer == null)
        {
            m_ActivePlayer = m_Players.First;
            if (!entity.GetComponents().Any(c => c.GetType() == typeof(PlayerInputController)))
                entity.AddComponent(new PlayerInputController());
        }
        else if (!entity.GetComponents().Any(c => c.GetType() == typeof(AIController)))
        {
            entity.AddComponent(new AIController());
        }

        if (m_TimeProgression == null)
            m_TimeProgression = new TimeProgression();
        m_TimeProgression.RegisterEntity(entity);

        //m_PlayerToTimeProgressionMap[entity] = new TimeProgression();
        //m_PlayerToTimeProgressionMap[entity].RegisterEntity(entity);
    }

    public void ConvertToPlayableEntity(IEntity entity)
    {
        entity.RemoveComponent(typeof(InputControllerBase));
        entity.RemoveComponent(typeof(RegisterWithTimeSystem));

        entity.AddComponent(new RegisterPlayableCharacter());

        entity.CleanupComponents();
    }

    public void UnregisterPlayer(IEntity entity)
    {
        //May need to rotate to the next active character first
        m_Players.Remove(entity);
        m_TimeProgression.RemoveEntity(entity);
        //m_PlayerToTimeProgressionMap.Remove(entity);
        FireEvent(Self, new GameEvent(GameEventId.Despawn, new KeyValuePair<string, object>(EventParameters.Entity, entity),
                                                            new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Creature)));
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : WorldComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.StartWorld);
        RegisteredEvents.Add(GameEventId.RotateActiveCharacter);
        RegisteredEvents.Add(GameEventId.SetActiveCharacter);
        RegisteredEvents.Add(GameEventId.ConvertToPlayableCharacter);
        RegisteredEvents.Add(GameEventId.RegisterPlayableCharacter);
        RegisteredEvents.Add(GameEventId.UnRegisterPlayer);
        RegisteredEvents.Add(GameEventId.UpdateCamera);
        RegisteredEvents.Add(GameEventId.IsPlayableCharacter);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.RotateActiveCharacter)
        {
            RotateCharacter();
            FireEvent(Self, new GameEvent(GameEventId.UpdateUI, new KeyValuePair<string, object>(EventParameters.Entity, m_ActivePlayer.Value.ID)));
        }

        if (gameEvent.ID == GameEventId.SetActiveCharacter)
        {
            string id = gameEvent.GetValue<string>(EventParameters.Entity);
            string startId = m_ActivePlayer.Value.ID;

            while (m_ActivePlayer.Value.ID != id)
            {
                RotateCharacter();
                if (m_ActivePlayer.Value.ID == startId)
                    break;
            }

            FireEvent(Self, new GameEvent(GameEventId.UpdateUI, new KeyValuePair<string, object>(EventParameters.Entity, m_ActivePlayer.Value.ID)));
        }

        if(gameEvent.ID == GameEventId.IsPlayableCharacter)
        {
            string id = gameEvent.GetValue<string>(EventParameters.Entity);
            gameEvent.Paramters[EventParameters.Value] = m_Players.Contains(EntityQuery.GetEntity(id));
        }

        //if(gameEvent.ID == GameEventId.GetActivePlayer)
        //{
        //    gameEvent.Paramters[EventParameters.Entity] = m_ActivePlayer.Value;
        //}

        if (gameEvent.ID == GameEventId.ConvertToPlayableCharacter)
        {
            ConvertToPlayableEntity(EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]));
        }

        if (gameEvent.ID == GameEventId.RegisterPlayableCharacter)
        {
            RegisterPlayer(EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]));
        }

        if (gameEvent.ID == GameEventId.UnRegisterPlayer)
        {
            UnregisterPlayer(EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]));
            if (m_Players.Count == 0)
                FireEvent(Self, new GameEvent(GameEventId.GameFailure));
        }

        if (gameEvent.ID == GameEventId.UpdateCamera)
        {
            if (m_ActivePlayer != null)
            {
                EventBuilder setCamera = new EventBuilder(GameEventId.SetCameraPosition)
                                    .With(EventParameters.Point, PathfindingUtility.GetEntityLocation(m_ActivePlayer.Value));
                FireEvent(Self, setCamera.CreateEvent());
            }
        }
    }

    public void RegisterPlayer(IEntity entity)
    {
        if (!m_Players.Any(p => p.ID == entity.ID))
        {
            var addedNode = m_Players.AddLast(entity);

            if (entity.HasComponent(typeof(PlayerInputController)))
                m_ActivePlayer = addedNode;
            else if (m_ActivePlayer == null && !entity.HasComponent(typeof(InputControllerBase)))
            {
                entity.AddComponent(new PlayerInputController());
                m_ActivePlayer = addedNode;
            }
            else if (!entity.HasComponent(typeof(InputControllerBase)))
                entity.AddComponent(new AIController());

        }

        m_TimeProgression.RegisterEntity(entity);

        if (!m_ActivePlayer.Value.HasComponent(typeof(PartyLeader)))
            m_ActivePlayer.Value.AddComponent(new PartyLeader());

        if (m_ActivePlayer != null)
        {
            EventBuilder setCamera = new EventBuilder(GameEventId.SetCameraPosition)
                                        .With(EventParameters.Point, PathfindingUtility.GetEntityLocation(m_ActivePlayer.Value));
            FireEvent(Self, setCamera.CreateEvent());
        }

        //m_PlayerToTimeProgressionMap[entity] = new TimeProgression();
        //m_PlayerToTimeProgressionMap[entity].RegisterEntity(entity);
    }

    public void ConvertToPlayableEntity(IEntity entity)
    {
        if (!entity.HasComponent(typeof(RegisterPlayableCharacter)))
        {
            entity.RemoveComponent(typeof(AIController));
            entity.RemoveComponent(typeof(AIFOVHandler));
            entity.RemoveComponent(typeof(RegisterWithTimeSystem));
            entity.AddComponent(new RegisterPlayableCharacter());
            entity.AddComponent(new PlayerFOVHandler());
            entity.CleanupComponents();
        }
    }

    public void UnregisterPlayer(IEntity entity)
    {
        //May need to rotate to the next active character first
        m_Players.Remove(entity);
        if (entity == m_ActivePlayer.Value)
            RotateCharacter();
        m_TimeProgression.RemoveEntity(entity);
        //m_PlayerToTimeProgressionMap.Remove(entity);
        FireEvent(Self, new GameEvent(GameEventId.Despawn, new KeyValuePair<string, object>(EventParameters.Entity, entity.ID),
                                                            new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Creature)));
    }

    void RotateCharacter()
    {
        //if (m_Players.Count == 1)
        //    return;

        bool hasUIController = m_ActivePlayer.Value.GetComponents().Any(comp => comp.GetType() == typeof(PlayerUIController));
        m_ActivePlayer.Value.RemoveComponent(typeof(InputControllerBase));
        m_ActivePlayer.Value.RemoveComponent(typeof(PartyLeader)); //We're going to need to start tracking the memebers of a party if we want to be able to split parties up intentionally
        m_ActivePlayer.Value.AddComponent(new AIController());
        //m_ActivePlayer.Value.CleanupComponents();

        m_ActivePlayer = m_ActivePlayer.Next;
        if (m_ActivePlayer == null)
            m_ActivePlayer = m_Players.First;

        if (m_ActivePlayer == null)
            return;

        m_ActivePlayer.Value.CleanupComponents();
        m_ActivePlayer.Value.RemoveComponent(typeof(AIController));
        if(hasUIController)
            m_ActivePlayer.Value.AddComponent(new PlayerUIController());
        else
        {
            m_ActivePlayer.Value.AddComponent(new PlayerInputController());
            m_ActivePlayer.Value.AddComponent(new PartyLeader());
        }
        m_ActivePlayer.Value.CleanupComponents();

        m_TimeProgression.SetActiveEntity(m_ActivePlayer.Value);

        EventBuilder setCamera = new EventBuilder(GameEventId.SetCameraPosition)
                                .With(EventParameters.Point, PathfindingUtility.GetEntityLocation(m_ActivePlayer.Value));
        FireEvent(Self, setCamera.CreateEvent());
    }
}

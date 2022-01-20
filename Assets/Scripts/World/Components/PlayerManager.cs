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
            UIManager.RemovePopUntilAllOfTypeRemoved<ContextMenuMono>();
            WorldUIController.UpdateUI(m_ActivePlayer.Value.ID);

            //FireEvent(Self, GameEventPool.Get(GameEventId.UpdateUI)
            //    .With(EventParameters.Entity, m_ActivePlayer.Value.ID)).Release();
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

            WorldUIController.UpdateUI(m_ActivePlayer.Value.ID);
            //FireEvent(Self, GameEventPool.Get(GameEventId.UpdateUI)
            //    .With(EventParameters.Entity, m_ActivePlayer.Value.ID)).Release();
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
            {
                m_TimeProgression.Stop();
                FireEvent(Self, GameEventPool.Get(GameEventId.GameFailure)).Release();
            }
        }

        if (gameEvent.ID == GameEventId.UpdateCamera)
        {
            if (m_ActivePlayer != null)
            {
                GameEvent setCamera = GameEventPool.Get(GameEventId.SetCameraPosition)
                                    .With(EventParameters.Point, PathfindingUtility.GetEntityLocation(m_ActivePlayer.Value));
                FireEvent(Self, setCamera).Release();
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

        if (m_ActivePlayer != null)
        {
            GameEvent setCamera = GameEventPool.Get(GameEventId.SetCameraPosition)
                                        .With(EventParameters.Point, PathfindingUtility.GetEntityLocation(m_ActivePlayer.Value));
            FireEvent(Self, setCamera).Release();

            GameEvent makeActivePlayer = GameEventPool.Get(GameEventId.MakePartyLeader)
                                            .With(EventParameters.Entity, m_ActivePlayer.Value.ID);
            FireEvent(Self, makeActivePlayer).Release();
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
        FireEvent(Self, GameEventPool.Get(GameEventId.Despawn)
            .With(EventParameters.Entity, entity.ID)
            .With(EventParameters.EntityType, EntityType.Creature)).Release();
    }

    void RotateCharacter()
    {
        //if (m_Players.Count == 1)
        //    return;

        bool hasUIController = m_ActivePlayer.Value.GetComponents().Any(comp => comp.GetType() == typeof(PlayerUIController));
        m_ActivePlayer.Value.RemoveComponent(typeof(InputControllerBase));
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
            m_ActivePlayer.Value.AddComponent(new PlayerInputController());

        GameEvent makePartyLeader = GameEventPool.Get(GameEventId.MakePartyLeader)
                                            .With(EventParameters.Entity, m_ActivePlayer.Value.ID);
        FireEvent(Self, makePartyLeader).Release();

        m_ActivePlayer.Value.CleanupComponents();

        m_TimeProgression.SetActiveEntity(m_ActivePlayer.Value);

        GameEvent setCamera = GameEventPool.Get(GameEventId.SetCameraPosition)
                                .With(EventParameters.Point, PathfindingUtility.GetEntityLocation(m_ActivePlayer.Value));
        FireEvent(Self, setCamera).Release();
    }
}

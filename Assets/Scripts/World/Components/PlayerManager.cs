using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : GameService
{
    public void RotateActiveCharacter()
    {
        RotateCharacter();
        UIManager.RemovePopUntilAllOfTypeRemoved<ContextMenuMono>();
        Services.WorldUIService.UpdateUI(m_ActivePlayer.Value.ID);
    }

    public void SetActiveCharacter(string id)
    {
        string startId = m_ActivePlayer.Value.ID;

        while (m_ActivePlayer.Value.ID != id)
        {
            RotateCharacter();
            if (m_ActivePlayer.Value.ID == startId)
                break;
        }

        Services.WorldUIService.UpdateUI(m_ActivePlayer.Value.ID);
    }

    public bool IsPlayableCharacter(string id)
    {
        return m_Players.Contains(EntityQuery.GetEntity(id));
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
            Services.CameraService.SetCameraPosition(m_EntityToPointMap[m_ActivePlayer.Value]);
            Services.PartyService.MakePartyLeader(m_ActivePlayer.Value);
        }
    }

    public void UnRegisterCharacter(string id)
    {
        UnRegisterPlayer(m_EntityIdToEntityMap[id]);
        if (m_Players.Count == 0)
        {
            m_TimeProgression.Stop();
            Services.StateManagerService.GameOver(false);
        }
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

    public void UnRegisterPlayer(IEntity entity)
    {
        m_Players.Remove(entity);
        if (entity == m_ActivePlayer.Value)
            RotateCharacter();
        m_TimeProgression.RemoveEntity(entity);
        Services.SpawnerService.Despawn(entity);
    }

    void RotateCharacter()
    {
        bool hasUIController = m_ActivePlayer.Value.GetComponents().Any(comp => comp.GetType() == typeof(PlayerUIController));
        m_ActivePlayer.Value.RemoveComponent(typeof(InputControllerBase));
        m_ActivePlayer.Value.AddComponent(new AIController());

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

        Services.PartyService.MakePartyLeader(m_ActivePlayer.Value);

        m_ActivePlayer.Value.CleanupComponents();
        m_TimeProgression.SetActiveEntity(m_ActivePlayer.Value);

        Services.CameraService.SetCameraPosition(m_EntityToPointMap[m_ActivePlayer.Value]);
    }
}

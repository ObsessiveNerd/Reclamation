using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerManager : GameService
{
    public void RotateActiveCharacter()
    {
        RotateCharacter();
        UIManager.RemovePopUntilAllOfTypeRemoved<ContextMenuMono>();
        Services.WorldUIService.UpdateUI();
    }

    public List<IEntity> GetPlayerActiveAbilities(string id)
    {
        List<IEntity> activeAbilities = new List<IEntity>();
        GameEvent getAbilities = GameEventPool.Get(GameEventId.GetActiveAbilities)
                                    .With(EventParameters.Abilities, new List<IEntity>());
        m_EntityIdToEntityMap[id].FireEvent(getAbilities);
        activeAbilities = getAbilities.GetValue<List<IEntity>>(EventParameters.Abilities);
        getAbilities.Release();
        return activeAbilities.Distinct().ToList();
    }

    public IEntity GetActivePlayer()
    {
        return m_ActivePlayer?.Value;
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

        Services.WorldUIService.UpdateUI();
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
            try
            {
                Point p = m_EntityToPointMap.ContainsKey(m_ActivePlayer.Value.ID) ? m_EntityToPointMap[m_ActivePlayer.Value.ID] 
                    : m_EntityToPreviousPointMap[m_ActivePlayer.Value.ID];

                Services.CameraService.SetCameraPosition(p);
                Services.PartyService.MakePartyLeader(m_ActivePlayer.Value);
            }
            catch
            {
                Debug.LogError($"{m_ActivePlayer.Value} was not found in entity -> point map.  This has to do with how we" +
                    $"despawn and spawn etities to move them.  Bug presents itself primarily when walking over allies.");
            }
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

    public List<IEntity> GetPlayerEntitiesToSpawn()
    {
        List<IEntity> result = new List<IEntity>();
        {
            string charactersPath = GameSaveSystem.kSaveDataPath + "/" + Services.SaveAndLoadService.CurrentSaveName + "/Blueprints/Characters";
//#if UNITY_EDITOR
            if (!Directory.Exists(charactersPath) && m_Players.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    IEntity player = EntityFactory.CreateEntity("DwarfWarrior");
                    result.Add(player);
                    //Spawner.Spawn(player, DungeonGenerator.Rooms[0].GetValidPoint(null));
                    //player.CleanupComponents();
                    //FireEvent(player, GameEventPool.Get(GameEventId.InitFOV)).Release();
                }
            }

            else
//#endif
            {
                foreach (var bp in Directory.EnumerateFiles(charactersPath, "*.bp"))
                {
                    IEntity player = EntityFactory.CreateEntity(Path.GetFileNameWithoutExtension(bp));
                    result.Add(player);

                    //Services.PlayerManagerService.ConvertToPlayableEntity(player);
                    //Spawner.Spawn(player, Services.DungeonService.DungeonGenerator.Rooms[0].GetValidPoint(null));

                    //player.CleanupComponents();

                    //FireEvent(player, GameEventPool.Get(GameEventId.InitFOV)).Release();
                }
                Directory.Delete(charactersPath, true);
            }
            return result;
        }

        //{
        //    foreach (var player in m_Players)
        //        FireEvent(player, GameEventPool.Get(GameEventId.InitFOV)).Release();
        //    Services.CameraService.UpdateCamera();
        //}
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
        if (m_ActivePlayer.Value == null || entity == m_ActivePlayer.Value)
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

        InputControllerBase inputController;
        if (hasUIController)
            inputController = new PlayerUIController();
        else
            inputController = new PlayerInputController();

        m_ActivePlayer.Value.AddComponent(inputController);
        inputController.Start();
        Services.PartyService.MakePartyLeader(m_ActivePlayer.Value);

        m_ActivePlayer.Value.CleanupComponents();
        m_TimeProgression.SetActiveEntity(m_ActivePlayer.Value);

        Services.CameraService.SetCameraPosition(m_EntityToPointMap[m_ActivePlayer.Value.ID]);
    }
}
